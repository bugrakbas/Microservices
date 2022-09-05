using Data.Context;
using Data.Models;
using DataAccess.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Data.Jwt
{
    public class JwtManager
    {
        private readonly IConfiguration _configuration;

        private DBContext _dBContext;
        public JwtManager(DBContext dBContext, IConfiguration configuration)
        {
            _dBContext = dBContext;
            _configuration = configuration;
        }
        public string GenerateToken(string email, int expireMinutes = 20)
        {
            var symmetricKey = Convert.FromBase64String(_configuration["JWT:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, email)
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(_configuration["JWT:Key"]);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }

        public string Get(string email, string password)
        {
            if (CheckUser(email, password))
            {
                return GenerateToken(email);
            }
            return "";
        }

        public bool CheckUser(string email, string password)
        {
            using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
            {
                var customer = unitOf.GetRepository<Customer>().Get(x => x.Email == email && x.Password == password);
                if (customer != null)
                    return true;
                else
                    return false;
            }
        }

        public bool ValidateToken(string token)
        {
            using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
            {
                if (unitOf.GetRepository<Customer>().Get(x => x.CurrentToken == token) != null)
                    return true;
                else
                    return false;
            }
        }
    }
}
