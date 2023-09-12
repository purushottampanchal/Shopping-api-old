using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Models;
using ShoppingApi.Models.Dto;
using ShoppingApi.MyDbContext;

namespace ShoppingApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<CatlogController> _logger;

        public UserController(ApplicationDbContext db, ILogger<CatlogController> logger)
        {
            _db = db;
            _logger = logger;
        }



        [HttpGet("GetAddress/User/{userId}")]
        [Authorize(Roles = "User")]
        
        public async Task<IActionResult> GetUserAddress(int userId)
        {

            List<Address> addresses = _db.Addresses.Where(x => x.UserId == userId).ToList();
            if (addresses == null || addresses.Count == 0)
            {
                return NotFound("No Address present for user");
            }
            else
            {

                return Ok(addresses);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet("GetUserDetails/{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            User user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) return NotFound();
            else return Ok(user);
        }


        [Authorize(Roles = "User")]
        [HttpPost("AddAddress/User/")]
        public async Task<IActionResult> SetUserAddress(AddressDto addressDto)
        {
            if (ModelState.IsValid)
            {
                Address address = new Address()
                {
                    UserId = addressDto.UserId,
                    City = addressDto.City,
                    HouseNo = addressDto.HouseNo,
                    Pincode = addressDto.Pincode,
                    State = addressDto.State,
                    StreetArea = addressDto.StreetArea,
                };

                _db.Addresses.Add(address);
                var res = await _db.SaveChangesAsync();
                _logger.LogInformation($"Address added for user {addressDto.UserId}");
                return Ok(res);

            }

            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);

        }

        [Authorize(Roles = "User")]
        [HttpGet("GetCardDetails/User/{userId}")]
        public async Task<IActionResult> GetUserCardDetails(int userId)
        {

            List<CardDetail> cardDetails = _db.CardDetails.Where(x => x.UserId == userId).ToList();
            if (cardDetails == null || cardDetails.Count == 0)
            {
                return NotFound($"No Card details found for user {userId}");
            }

            return Ok(cardDetails);


        }

        [Authorize(Roles = "User")]
        [HttpPost("AddCard/User/")]
        public async Task<IActionResult> AddCardForUser(CardDto cardDto)
        {
            if (ModelState.IsValid)
            {
                CardDetail card = new()
                {
                    CardNo = cardDto.CardNo,
                    NameOnCard = cardDto.NameOnCard,
                    CVV = cardDto.CVV,
                    Expiration = cardDto.Expiration,
                    UserId = cardDto.UserId,
                };

                _db.CardDetails.Add(card);
                var res = await _db.SaveChangesAsync();
                _logger.LogInformation($"Address added for user {cardDto.UserId}");
                return Ok(res);

            }

            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("RemoveCard/User/{id}")]
        public async Task<IActionResult> RemoveCard(int id)
        {
            var card = _db.CardDetails.FirstOrDefault(x => x.Id == id);
            if (card == null)
                return NotFound();
            else
            {
                _db.CardDetails.Remove(card);
                await _db.SaveChangesAsync();
                return Ok("Card removed with id " + id);
            }
        }

        [Authorize(Roles = "User")]
        [HttpDelete("RemoveAddress/User/{id}")]
        public async Task<IActionResult> RemoveAddress(int id)
        {
            var address = _db.Addresses.FirstOrDefault(x => x.Id == id);
            if (address == null)
                return NotFound();
            else
            {
                _db.Addresses.Remove(address);
                await _db.SaveChangesAsync();
                return Ok("Address removed with id " + id);
            }
        }



    }
}
