using Microsoft.IdentityModel.Tokens;
using shoppingApi.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace shoppingApi.Services
{
    public class TokenValidator
    {

        private readonly JwtConfig _config;

        public TokenValidator(JwtConfig config)
        {
            _config = config;
        }


        public bool ValidateAccessToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.AccessTokenSecretKey)),
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            return validatedToken != null;

        }
        public bool ValidateRefreshToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey)),
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
            };

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            return validatedToken != null;

        }


    }
}
