using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using shoppingApi.Configurations;
using shoppingApi.Models;
using shoppingApi.Models.Responces;
using shoppingApi.Models.AuthenticationModels;
using ShoppingApi.Models;
using ShoppingApi.MyDbContext;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using shoppingApi.Models.Constants;

namespace shoppingApi.Services
{
    public class AuthenticationService
    {
        readonly JwtConfig _config;
        readonly ApplicationDbContext _db;
        public AuthenticationService(JwtConfig config, ApplicationDbContext dbContext)
        {
            _config = config;
            _db = dbContext;
        }

        public async Task<User> LoginUser(LoginRequest userCredential)
        {

            //getting user's data from the userName
            //var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userCredential.UserName);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userCredential.UserName && u.Role == UserRoles.USER);
            //if user is null then no user is found of given username

            if (user != null)
                if (VerifyPsswordHashes(user, userCredential.Password))
                {
                    return user;
                }

            return null;


        }

        public async Task<User> AttemptLogin(LoginRequest userCredential)
        {

            //getting user's data from the userName
            //var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userCredential.UserName);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == userCredential.UserName);
            //if user is null then no user is found of given username

            if (user != null)
                if (VerifyPsswordHashes(user, userCredential.Password))
                {
                    return user;
                }

            return null;


        }

        private static bool VerifyPsswordHashes(User user, string password)
        {
            PasswordHasher<string> pw = new PasswordHasher<string>();

            var result = pw.VerifyHashedPassword(user.UserName, user.HashedPassword.Trim(), password.Trim());
            return result == PasswordVerificationResult.Success;

        }

        public async Task<User> LoginAdmin(LoginRequest adminCredentials)
        {

            //getting user's data from the userName
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == adminCredentials.UserName && u.Role == UserRoles.ADMIN);
            //if user is null then no user is found of given username

            if (user != null)
                if (VerifyPsswordHashes(user, adminCredentials.Password))
                {
                    return user;
                }

            return null;

        }

        public SuccessfullAuthenticationResponce GenerateLoginResponce(User user)
        {
            string token = GenerateToken(user);
            string refreshToken = GenerateRefreshToke();

            //get from db
            _db.RefreshTokens.Add(new RefreshToken()
            {
                Token = refreshToken,
                UserName = user.UserName
            });
            //=------

            return new SuccessfullAuthenticationResponce()
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                UserId = user.Id
            };

        }

        private string GenerateRefreshToke()
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.RefreshTokenSecretKey));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: null,
                notBefore: DateTime.UtcNow,
                expires: DateTime.Now.AddMinutes(_config.RefreshTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateToken(User user)
        {

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),

            };

            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.AccessTokenSecretKey));

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.Now.AddMinutes(_config.AccessTokenExpirationMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

        public static string? GetCurrentUserId(ClaimsIdentity identity)
        {

            if (identity != null)
            {
                var id =
                    identity.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier)?.Value;

                return id;
            }
            return null;

        }

    }
}
