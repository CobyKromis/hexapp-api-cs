using hexapp_api_cs.Models.Authentication;
using hexapp_api_cs.Services.Authentication;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading.Tasks;

namespace hexapp_api_cs.Helpers.Authentication
{
    public class AuthenticationHelpers
    {
        private const int PWD_SALT_SIZE = 16;
        private const int PWD_HASH_SIZE = 48;
        private const int PWD_HASH_ITER = 100000;

        public static string EncrpytPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[PWD_SALT_SIZE]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, PWD_HASH_ITER);
            var hash = pbkdf2.GetBytes(PWD_HASH_SIZE);

            var hashBytes = new byte[PWD_SALT_SIZE + PWD_HASH_SIZE];
            Array.Copy(salt, 0, hashBytes, 0, PWD_SALT_SIZE);
            Array.Copy(hash, 0, hashBytes, PWD_SALT_SIZE, PWD_HASH_SIZE);

            var base64Hash = Convert.ToBase64String(hashBytes);

            return string.Format("$HEX$V1${0}${1}", PWD_HASH_ITER, base64Hash);
        }

        public static bool IsPasswordValid(string password, string hashedPassword)
        {
            var splitHashString = hashedPassword.Replace("$HEX$V1$", "").Split('$');
            var iterations = int.Parse(splitHashString[0]);
            var base64Hash = splitHashString[1];

            var hashBytes = Convert.FromBase64String(base64Hash);

            var salt = new byte[PWD_SALT_SIZE];
            Array.Copy(hashBytes, 0, salt, 0, PWD_SALT_SIZE);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(PWD_HASH_SIZE);

            for (var i = 0; i < PWD_HASH_SIZE; i++)
            {
                if (hashBytes[i + PWD_SALT_SIZE] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static string GenerateAuthToken(User user)
        {
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(
                    Convert.FromBase64String(Startup.StaticConfiguration.GetSection("JWTOptions")["Secret"])
                ), SecurityAlgorithms.HmacSha256Signature
            );

            List<Claim> userClaims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: Startup.StaticConfiguration.GetSection("JWTOptions")["Issuer"],
                audience: Startup.StaticConfiguration.GetSection("JWTOptions")["Audience"],
                expires: DateTime.UtcNow.AddMinutes(5),
                notBefore: DateTime.UtcNow.AddSeconds(-5),
                signingCredentials: creds,
                claims: userClaims
            );

            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRefreshToken(User user, int authTokenId)
        {
            var creds = new SigningCredentials(
               new SymmetricSecurityKey(
                   Convert.FromBase64String(Startup.StaticConfiguration.GetSection("JWTOptions")["Secret"])
               ), SecurityAlgorithms.HmacSha256Signature
           );

            List<Claim> claims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim("authTokenId", authTokenId.ToString())
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = new JwtSecurityToken(
                issuer: Startup.StaticConfiguration.GetSection("JWTOptions")["Issuer"],
                audience: Startup.StaticConfiguration.GetSection("JWTOptions")["Audience"],
                expires: DateTime.UtcNow.AddHours(4),
                notBefore: DateTime.UtcNow,
                signingCredentials: creds,
                claims: claims
            );

            return tokenHandler.WriteToken(token);
        }

        public static bool IsTokenValid(string token)
        {
            var creds = new SymmetricSecurityKey(
               Convert.FromBase64String(Startup.StaticConfiguration.GetSection("JWTOptions")["Secret"])
            );

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = Startup.StaticConfiguration.GetSection("JWTOptions")["Issuer"],
                ValidateAudience = true,
                ValidAudience = Startup.StaticConfiguration.GetSection("JWTOptions")["Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = creds
            };

            SecurityToken validatedToken;

            try
            {
                IPrincipal principal = handler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch
            {
                return false;
            }

            return new TokenService().IsTokenValid(token);
        }

        public static JwtSecurityToken ReadToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedToken = handler.ReadJwtToken(token);

            return decodedToken;
        }

        public static string GetUserIdFromToken(string token)
        {
            var decodedToken = ReadToken(token);
            return decodedToken.Claims.First(c => c.Type == "userId").Value;
        }
    }
}
