using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Database;
using AA2_CS.JWT;
using AA2_CS.Repository;
using AA2_CS.Service;
using AA2_CS.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AA2_CS.Model.Common;
using System.Security.Claims;

namespace Backend.Tests;

public class AuthServiceTests
{
    [Fact] // Se marca el método como prueba unitaria 
    public void HasAccessToResource_CuandoEsPropietario_DevuelveTrue()
    {
        // Esta funcion prueba el acceso a los metodos necesarios cuando el usuario es el propietario del propio objeto. Se espera que devuelva true.

        var service = new AuthService(new AuthRepository(null!, null!, null!, null!, null!));
        var principal = BuildPrincipal(userId: 5, role: Roles.userNormal);

        var canAccess = service.HasAccessToResource(5, principal);

        Assert.True(canAccess);
    }

    [Fact]
    public void HasAccessToResource_CuandoEsAdmin_DevuelveTrue()
    {
        // Esta funcion prueba el acceso a los metodos necesarios cuando el usuario es admin. Se espera que devuelva true.

        var service = new AuthService(new AuthRepository(null!, null!, null!, null!, null!));
        var principal = BuildPrincipal(userId: 2, role: Roles.userMaster);

        var canAccess = service.HasAccessToResource(9, principal);

        Assert.True(canAccess);
    }

    [Fact]
    public void HasAccessToResource_CuandoNoEsPropietarioNiAdmin_DevuelveFalse()
    {
        // Esta funcion prueba el acceso a los metodos necesarios cuando el usuario no es el propietario ni admin. Se espera que devuelva false.

        var service = new AuthService(new AuthRepository(null!, null!, null!, null!, null!));
        var principal = BuildPrincipal(userId: 2, role: Roles.userNormal);

        var canAccess = service.HasAccessToResource(9, principal);

        Assert.False(canAccess);
    }

    [Fact]
    public void Login_CredencialesValidas_DevuelveToken()
    {
        // Comprobación del proceso de login con credenciales válidas. Se espera que devuelva un token no nulo ni vacío.
        using var context = TestDbContextFactory.CreateContext();
        var purchaseRepository = new PurchaseRepository(context);
        var userRepository = new UserRepository(context, purchaseRepository);
        var verificationRepository = new EmailVerificationRepository(context);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "clave-super-secreta-con-longitud-suficiente-12345",
                ["JwtSettings:Issuer"] = "TheTrainingHub",
                ["JwtSettings:Audience"] = "TheTrainingHubUsers"
            })
            .Build();

        var jwtConfigurer = new JWTConfigurer(config, new UserService(userRepository));
        var authService = new AuthService(new AuthRepository(userRepository, jwtConfigurer, null!, verificationRepository, new NotificationPreferenceRepository(context)));

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
        // Comprobación del proceso de login con credenciales inválidas. Se espera que devuelva null.
        using var context = TestDbContextFactory.CreateContext();
        var purchaseRepository = new PurchaseRepository(context);
        var userRepository = new UserRepository(context, purchaseRepository);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "clave-super-secreta-con-longitud-suficiente-12345",
                ["JwtSettings:Issuer"] = "TheTrainingHub",
                ["JwtSettings:Audience"] = "TheTrainingHubUsers"
            })
            .Build();

        var jwtConfigurer = new JWTConfigurer(config, null!);
        var authService = new AuthService(new AuthRepository(
            userRepository,
            jwtConfigurer,
            null!,
            new EmailVerificationRepository(context),
            new NotificationPreferenceRepository(context)));

        var token = authService.Login("noexiste@test.com", "123456");

        Assert.Null(token);
    }

    private static ClaimsPrincipal BuildPrincipal(int userId, string role)
    {
        // Construye un ClaimsPrincipal con los claims necesarios para las pruebas de acceso. Incluye el ID del usuario y su rol.
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
