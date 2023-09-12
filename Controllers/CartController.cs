using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Models;
using ShoppingApi.MyDbContext;

namespace ShoppingApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CatlogController> _logger;

        public CartController(ApplicationDbContext db, ILogger<CatlogController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("getcartforuser/{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            try
            {
                var cartDto = new CartResponce();
                User UserAssociatedWithCart = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
                Cart CartAssociatedWithUser = await _db.Carts.Where(x => x.UserId == userId).FirstOrDefaultAsync();
                if (CartAssociatedWithUser == null)
                {
                    return NoContent();
                }
                List<CartItem> ItemsAssociatedWithCart = _db.CartItems.Where(x => x.CartId == CartAssociatedWithUser.Id).ToList();


                if (ItemsAssociatedWithCart == null)
                {
                    return NoContent();
                }
                List<CarItemtResponceDto> CartItemsList = new();

                CartItemsList.AddRange(from i in ItemsAssociatedWithCart
                                       let item = _db.Items.FirstOrDefault(x => x.Id == i.ItemId)
                                       select new CarItemtResponceDto()
                                       {
                                           ItemId = item.Id,
                                           BrandId = item.BrandId,
                                           Description = item.Description,
                                           ImgUrl = item.ImageUrl,
                                           ItemCost = item.Cost,
                                           name = item.Name,
                                           qty = i.Qty,
                                           TotalCost = item.Cost * i.Qty,
                                           TypeId = item.CatlogId
                                       });

                cartDto = new()
                {
                    Id = CartAssociatedWithUser.Id,
                    UserId = UserAssociatedWithCart.Id,
                    UserName = UserAssociatedWithCart.UserName,
                    CartItems = CartItemsList,
                    CartStatus = CartAssociatedWithUser.CartStatus,
                    CartCost = CartAssociatedWithUser.CartCost,
                };

                return Ok(cartDto);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return BadRequest();
            }
            //List<Item> items = new();

        }

        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddCartDto newCartItem)
        {
            var exsistingCart = await _db.Carts.FirstOrDefaultAsync(x => x.UserId == newCartItem.UserId);
            int CartId;
            int CartCost = 0;

            //create new Cart
            if (exsistingCart == null)
            {
                Cart c = new Cart()
                {
                    CartCost = 0,
                    CartStatus = newCartItem.CartStatus,
                    UserId = newCartItem.UserId
                };
                _db.Carts.Add(c);

                await _db.SaveChangesAsync();
                CartId = c.Id;
            }
            else
            {
                CartCost = exsistingCart.CartCost;
                CartId = exsistingCart.Id;
            }
            // update items in cart

            foreach (var i in newCartItem.CartItems)
            {
                Item currItem = _db.Items.FirstOrDefault(x => x.Id == i.ItemId);
                CartCost += (currItem.Cost * i.Qty);

                var itemExistInCart = await _db.CartItems.FirstOrDefaultAsync(x => x.ItemId == i.ItemId);

                if (itemExistInCart != null)
                {
                    if (i.Qty == 0)
                    {
                        _db.CartItems.Remove(itemExistInCart);
                        _db.SaveChanges();
                    }
                    else
                    {
                        itemExistInCart.Qty = i.Qty;
                        _db.SaveChanges();
                    }
                    //update 
                }
                else
                {
                    //add
                    _db.CartItems.Add(new CartItem()
                    {
                        CartId = CartId,
                        ItemId = i.ItemId,
                        Qty = i.Qty,
                    });

                }
            }

            //update cart cost
            Cart cart = _db.Carts.FirstOrDefault(x => x.Id == CartId);
            cart.CartCost = CartCost;
            await _db.SaveChangesAsync();

            return Ok("added to cart id " + CartId);

        }


        [HttpDelete("EmptyCartForUser/{userId}")]
        public async Task<IActionResult> DeleteCartItems(int userId)
        {
            Cart CartAssociatedWithUser = await _db.Carts.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            List<CartItem> cartItems = _db.CartItems.Where(x => x.CartId == CartAssociatedWithUser.Id).ToList();
            foreach (var i in cartItems)
            {
                _db.CartItems.Remove(i);
            }
            _db.SaveChanges();
            return Ok();
        }

    }
}
