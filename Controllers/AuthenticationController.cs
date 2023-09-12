using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shoppingApi.Configurations;
using shoppingApi.Models;
using shoppingApi.Models.Responces;
using shoppingApi.Models.AuthenticationModels;
using shoppingApi.Services;
using ShoppingApi.Models;
using ShoppingApi.MyDbContext;
using System.Security.Claims;
using shoppingApi.Models.Constants;
using ShoppingApi.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace shoppingApi.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {


        private AuthenticationService _auth;
        private TokenValidator _tokenValidator;
        private ApplicationDbContext _db;
        private ILogger<AuthenticationController> _logger;

        public AuthenticationController(JwtConfig config, ApplicationDbContext db, ILogger<AuthenticationController> logger)
        {
            _auth = new AuthenticationService(config, db);
            _tokenValidator = new TokenValidator(config);
            _db = db;
            _logger = logger;
        }

        //[HttpGet("Test")]
        //[Authorize]

        //public async Task<IActionResult> test()
        //{
        //    return Ok();
        //}

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserDto user)
        {

            if (ModelState.IsValid)
            {
                var exsistingUser = _db.Users.FirstOrDefault(x => x.UserName == user.UserName);
                if (exsistingUser == null)
                {


                    PasswordHasher<string> pw = new PasswordHasher<string>();

                    _db.Users.Add(new User()
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Name = user.Name,
                        Role = user.Role,
                        PhoneNumber = user.PhoneNumber,
                        HashedPassword = user.HashedPassword,
                    });

                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"New user created with username {user.UserName}");
                    return Ok("User Created Successfully");
                }
                else
                {
                    _logger.LogError($"Tried to create new user with existing username present ");
                    return BadRequest("User Already Exists with same username");
                }

            }

            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            _logger.LogError(messages);
            return BadRequest("Bad registration request " + messages);

        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {

            if (loginRequest == null)
            {
                _logger.LogError("Empty login request data provided");
                return BadRequest("empty login credentials");
            }
            User user;

            //if (loginRequest.UserRole == UserRoles.ADMIN)
            //{
            //    _logger.LogInformation($"Attempting admin login");
            //    user = await _auth.LoginAdmin(loginRequest);
            //}
            //else
            //{
            //    _logger.LogInformation($"Attempting User login");
            //    user = await _auth.LoginUser(loginRequest);
            //}

            user = await _auth.AttemptLogin(loginRequest);

            if (user == null)
            {
                _logger.LogInformation($"Login Failed for {loginRequest.UserName}");
                return Unauthorized("User Credentials Invalid");
            }

            _logger.LogInformation($"Login Sucessfull for {loginRequest.UserName}");
            SuccessfullAuthenticationResponce responce = _auth.GenerateLoginResponce(user);
            responce.Role = user.Role;
            return Ok(responce);

        }

        //validate and refresh

        //[HttpPost("validate")]
        //public IActionResult Validate(Token token)
        //{
        //    _logger.LogInformation("Validating token");
        //    return Ok(_tokenValidator.ValidateAccessToken(token.token));
        //}


        //[HttpPost("refresh")]
        //public async Task<IActionResult> Refresh([FromBody] Token refreshToken)
        //{

        //    //validate refreshToken

        //    string tokenString = refreshToken.token;
        //    bool IsValidToken = _tokenValidator.ValidateRefreshToken(tokenString);
        //    if (!IsValidToken)
        //    {
        //        _logger.LogInformation("Tried to refresh access token with invalid refresh token");
        //        return BadRequest("Invalid Refresh token");
        //    }

        //    // get user by refresh token

        //    RefreshToken refreshTokenDto = DemoRefreshDto.refreshTokenDtos.Where(x => x.Token == tokenString).FirstOrDefault();
        //    if (refreshTokenDto == null)
        //    {
        //        _logger.LogInformation("No user associated with refresh token");
        //        return BadRequest("Invalid Refresh token");
        //    }

        //    //invalidate refresh token

        //    _logger.LogInformation("Removed refresh token");
        //    DemoRefreshDto.refreshTokenDtos.Remove(refreshTokenDto);

        //    //authenticate user 

        //    User user = DemoUsers.UsersRecords.FirstOrDefault(u => u.UserName == refreshTokenDto.UserName);
        //    if (user == null)
        //    {
        //        return BadRequest("User with this refresh token not found");
        //    }

        //    SuccessfullAuthenticationResponce responce = _auth.GenerateLoginResponce(user);
        //    _logger.LogInformation("Generated new token pair");

        //    return Ok(responce);

        //}


        
        [Authorize]
        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string CurrentUserName = AuthenticationService.GetCurrentUserId(identity);

            if (string.IsNullOrEmpty(CurrentUserName))
            {
                _logger.LogInformation("No user Logged in");
                return Unauthorized("User not logged in");
            }

            DemoRefreshDto.refreshTokenDtos.RemoveAll(o => o.UserName == CurrentUserName);
            _logger.LogInformation("User Logged out");
            return Ok("Logged out");

        }




    }
}
