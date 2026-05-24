using AA2_CS.Model;
using AA2_CS.Database;
using AA2_CS.JWT;
using AA2_CS.Repository;
using AA2_CS.Service;
using AA2_CS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Backend.Tests;

public class AuthServiceTests
{
    [Fact]
    public void HasAccessToResource_CuandoEsPropietario_DevuelveTrue()
    {
        var service = new AuthService(null!, null!, null!, null!, null!);
        var principal = BuildPrincipal(userId: 5, role: Roles.userNormal);

        var canAccess = service.HasAccessToResource(5, principal);

        Assert.True(canAccess);
    }

    [Fact]
    public void HasAccessToResource_CuandoEsAdmin_DevuelveTrue()
    {
        var service = new AuthService(null!, null!, null!, null!, null!);
        var principal = BuildPrincipal(userId: 2, role: Roles.userMaster);

        var canAccess = service.HasAccessToResource(9, principal);

        Assert.True(canAccess);
    }

    [Fact]
    public void HasAccessToResource_CuandoNoEsPropietarioNiAdmin_DevuelveFalse()
    {
        var service = new AuthService(null!, null!, null!, null!, null!);
        var principal = BuildPrincipal(userId: 2, role: Roles.userNormal);

        var canAccess = service.HasAccessToResource(9, principal);

        Assert.False(canAccess);
    }

    [Fact]
    public void Login_CredencialesValidas_DevuelveToken()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);
        var verificationRepository = new EmailVerificationRepository(context);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "clave-super-secreta-con-longitud-suficiente-12345",
                ["JwtSettings:Issuer"] = "TheTrainingHub",
                ["JwtSettings:Audience"] = "TheTrainingHubUsers"
            })
            .Build();

        var jwtConfigurer = new JWTConfigurer(config, userService);
        var authService = new AuthService(userService, jwtConfigurer, null!, verificationRepository, new NotificationPreferenceRepository(context));

        userRepository.Register(new User
        {
            name = "User Login",
            email = "login@test.com",
            passwordhash = "123456",
            role = Roles.userNormal,
            isEmailVerified = true
        });

        var token = authService.Login("login@test.com", "123456");

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void Login_CredencialesInvalidas_DevuelveNull()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "clave-super-secreta-con-longitud-suficiente-12345",
                ["JwtSettings:Issuer"] = "TheTrainingHub",
                ["JwtSettings:Audience"] = "TheTrainingHubUsers"
            })
            .Build();

        var jwtConfigurer = new JWTConfigurer(config, userService);
        var authService = new AuthService(
            userService,
            jwtConfigurer,
            null!,
            new EmailVerificationRepository(context),
            new NotificationPreferenceRepository(context));

        var token = authService.Login("noexiste@test.com", "123456");

        Assert.Null(token);
    }

    private static ClaimsPrincipal BuildPrincipal(int userId, string role)
    {
        var identity = new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            },
            authenticationType: "TestAuth"
        );

        return new ClaimsPrincipal(identity);
    }
}
