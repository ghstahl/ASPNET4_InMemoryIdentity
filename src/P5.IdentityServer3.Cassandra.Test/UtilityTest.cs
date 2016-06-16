using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using P5.IdentityServer3.Cassandra.DAO;
using P5.IdentityServer3.Common;
using P5.MSTest.Common;

namespace P5.IdentityServer3.Cassandra.Test
{
    [TestClass]
    [DeploymentItem("source", "source")]
    public class UtilityTest : TestBase
    {

        [TestInitialize]
        public void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public async Task TestUpdateAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();

            var insert = await CassandraTestHelper.InsertTestData_Clients(1);
            var result = await dao.FindClientByIdAsync(insert[0].Id);
            Assert.AreEqual(insert[0].Record.ClientName, result.ClientName);

            await dao.TruncateTablesAsync();
            result = await dao.FindClientByIdAsync(insert[0].Id);
            Assert.IsNull(result);

        }
        [TestMethod]
        public async Task Test_CreateTablesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.CreateTablesAsync();
        }
        [TestMethod]
        public async Task Test_TruncateTablesAsync()
        {
            var dao = new IdentityServer3CassandraDao();
            await dao.EstablishConnectionAsync();
            await dao.TruncateTablesAsync();
        }

        [TestMethod]
        public async Task Test_JWT()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var symmetricKey = Guid.NewGuid().ToByteArray();
            var nameIdentifier = "5094df23-78a6-486d-bd39-7923bc2218ed";
            var now = DateTime.UtcNow;


            var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
                            new Claim(ClaimTypes.Role, "Author"),
                            new Claim("norton::action", Guid.NewGuid().ToString()),
                        }),
                    TokenIssuerName = "self",
                    AppliesToAddress = "http://www.example.com",
                    Lifetime = new Lifetime(now, now.AddSeconds(2)),
                    SigningCredentials = new SigningCredentials(
                        new InMemorySymmetricSecurityKey(symmetricKey),
                        "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                        "http://www.w3.org/2001/04/xmlenc#sha256"),


                };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var validationParameters = new TokenValidationParameters()
            {
                ValidAudiences = new[] { "http://www.example.com" },
                IssuerSigningToken = new BinarySecretSecurityToken(symmetricKey),
                ValidIssuer = "self",
                ValidateLifetime = true,

            };
            SecurityToken validatedToken = null;
            // will throw if lifetime outside of cpu clock skew
            var principal = tokenHandler.ValidateToken(tokenString, validationParameters,out validatedToken);


            byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenString);
            var tokenBytesEncoded = HttpServerUtility.UrlTokenEncode(tokenBytes);
            var tokenBytesDecoded = HttpServerUtility.UrlTokenDecode(tokenBytesEncoded);
            string tokenStringBack = System.Text.Encoding.UTF8.GetString(tokenBytesDecoded);

            Assert.AreEqual(tokenString,tokenStringBack);
            principal = tokenHandler.ValidateToken(tokenStringBack, validationParameters, out validatedToken);


            var tokenStringUrlEncoded = HttpUtility.UrlEncode(tokenString);
            var tokenStringUrlDecoded = HttpUtility.UrlDecode(tokenStringUrlEncoded);
            Assert.AreEqual(tokenString,tokenStringUrlDecoded);
            principal = tokenHandler.ValidateToken(tokenStringUrlDecoded, validationParameters, out validatedToken);
            Console.WriteLine(tokenString);


            var nowNow = DateTime.UtcNow;
            Assert.IsNotNull(validatedToken);
            Assert.IsTrue(nowNow > validatedToken.ValidFrom);
            Assert.IsTrue(nowNow <= validatedToken.ValidTo);


       //     Assert.IsTrue();

            Assert.IsTrue(principal.Identities.First().Claims
                .Any(c => c.Type == ClaimTypes.NameIdentifier && c.Value == nameIdentifier));
            Assert.IsTrue(principal.Identities.First().Claims
                .Any(c => c.Type == ClaimTypes.Role && c.Value == "Author"));
        }

    }
}
