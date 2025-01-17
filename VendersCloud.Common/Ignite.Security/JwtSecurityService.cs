    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.IdentityModel.Tokens;

    namespace IgniteSecurityLib
{
        public class JwtSecurityService
        {
            public static string BuildJwtToken(string secretKey, string userId, string issuer, string audience, int expirationMinutes)
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Audience = audience,
                    Issuer = issuer,
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    SigningCredentials = signingCredentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }

            public static JwtSecurityToken ValidateToken(string secretKey, string authToken, string issuer, string audience)
            {
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudiences = new[] { audience },
                    ValidIssuers = new[] { issuer },
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    tokenHandler.ValidateToken(authToken, tokenValidationParameters, out var validatedToken);
                    return validatedToken as JwtSecurityToken;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public static string Decode(JwtSecurityToken userPayloadToken)
            {
                return userPayloadToken.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier)?.Value;
            }

            public static string Decode(string token)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var userPayloadToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                return Decode(userPayloadToken);
            }

            // Encrypt the given string using AES
            public static string Encrypt(string secretKey, string plainText)
            {
                if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException(nameof(plainText));

                using var aes = Aes.Create();
                aes.Key = GenerateKey(secretKey, aes.KeySize / 8);
                aes.GenerateIV();

                using var ms = new MemoryStream();
                ms.Write(BitConverter.GetBytes(aes.IV.Length));
                ms.Write(aes.IV);

                using (var cryptoStream = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                }

                return System.Convert.ToBase64String(ms.ToArray());
            }

            // Decrypt the given string using AES
            public static string Decrypt(string secretKey, string cipherText)
            {
                if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException(nameof(cipherText));

                using var aes = Aes.Create();
                var cipherBytes = System.Convert.FromBase64String(cipherText);

                using var ms = new MemoryStream(cipherBytes);
                var ivLength = BitConverter.ToInt32(ms.ReadBytes(sizeof(int)));
                aes.IV = ms.ReadBytes(ivLength);
                aes.Key = GenerateKey(secretKey, aes.KeySize / 8);

                using var cryptoStream = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);

                return reader.ReadToEnd();
            }

            private static byte[] GenerateKey(string secretKey, int size)
            {
                using var sha256 = SHA256.Create();
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey)).Take(size).ToArray();
            }
        }

    }


