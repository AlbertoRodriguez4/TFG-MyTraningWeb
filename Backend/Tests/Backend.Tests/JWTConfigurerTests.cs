using AA2_CS.Database;
using AA2_CS.JWT;
using AA2_CS.Model.Entities;
using AA2_CS.Model.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend.Tests;

public class JWTConfigurerTests
{
    [Fact]
    public void GenerateToken_IncluyeClaimsCalculadas_ConBonosYExperiencia()
    {
        // Prueba que el método GenerateToken de JWTConfigurer incluye correctamente los claims calculados para fuerza, resistencia, experiencia requerida y experiencia restante. Se espera que los valores de los claims sean correctos según las propiedades del usuario y sus items equipados.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        var user = new User
        {
            id = 1,
            name = "Alberto",
            email = "alberto@test.com",
            passwordhash = "hash",
            role = Roles.userNormal,
            level = 2,
            strength = 10,
            endurance = 8,
            experience = 50,
            gold = 120,
            consistencystreak = 3,
            avatarUrl = "https://avatar.test/alberto.png",
            equippedStrengthId = 100,
            equippedEnduranceId = 200
        };

        var strengthItem = new Item { id = 100, name = "Mancuerna", type = "Strength", bonus = 5, price = 10 };
        var enduranceItem = new Item { id = 200, name = "Zapatillas", type = "Endurance", bonus = 3, price = 10 };

        context.Users.Add(user);
        context.Items.AddRange(strengthItem, enduranceItem);
        context.Purchases.AddRange(
            new Purchase { userid = 1, itemid = 100, Item = strengthItem },
            new Purchase { userid = 1, itemid = 200, Item = enduranceItem }
        );
        context.SaveChanges();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Key"] = "clave-super-secreta-con-longitud-suficiente-12345",
                ["JwtSettings:Issuer"] = "TheTrainingHub",
                ["JwtSettings:Audience"] = "TheTrainingHubUsers"
            })
            .Build();

        var userRepository = new AA2_CS.Repository.UserRepository(context);
        var purchaseRepository = new AA2_CS.Repository.PurchaseRepository(context);
        var userService = new AA2_CS.Service.UserService(userRepository, purchaseRepository);
        var jwtConfigurer = new JWTConfigurer(config, userService);

        var token = jwtConfigurer.GenerateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var claims = jwt.Claims.ToDictionary(c => c.Type, c => c.Value);

        Assert.Equal("1", claims[ClaimTypes.NameIdentifier]);
        Assert.Equal(Roles.userNormal, claims[ClaimTypes.Role]);
        Assert.Equal("15", claims["strength"]);
        Assert.Equal("11", claims["endurance"]);
        Assert.Equal("282", claims["xpRequired"]);
        Assert.Equal("232", claims["xpRemaining"]);
        Assert.Equal("https://avatar.test/alberto.png", claims["avatarUrl"]);
        Assert.Equal("100", claims["equippedStrengthItemId"]);
        Assert.Equal("200", claims["equippedEnduranceItemId"]);
    }
}
