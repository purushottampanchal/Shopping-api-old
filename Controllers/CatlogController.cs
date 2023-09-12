using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingApi.Models;
using ShoppingApi.Models.Dto;
using ShoppingApi.MyDbContext;
using System.Linq;

namespace ShoppingApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[CustomAttributes.CustomLog]
    public class CatlogController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CatlogController> _logger;

        public CatlogController(ApplicationDbContext db, ILogger<CatlogController> logger)
        {
            _db = db;
            _logger = logger;
        }

        //[Authorize(Roles = "User, Admin")]
        [HttpGet("Items")]
        public async Task<ActionResult<List<Item>>> Items()
        {

            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));

                _logger.LogCritical(messages);
                return BadRequest(messages);

            }

            var res = await _db.Items.ToListAsync();
            if (res.Count == 0 || res == null)
            {
                _logger.LogError("No Items in Database");
                return NotFound("Catlog is empty");
            }
            _logger.LogInformation("Success");
            return Ok(res);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Items")]
        public async Task<ActionResult<int>> Items([FromBody] ItemDto itemDto)
        {
            if (ModelState.IsValid)
            {
                var existsBrand = _db.Brands.FirstOrDefault(c => c.Id == itemDto.BrandId);
                var existsCatalog = _db.Catlogs.FirstOrDefault(c => c.Id == itemDto.CatlogId);

                if (existsBrand == null || existsCatalog == null)
                {
                    _logger.LogError("tried to add catalog item with invalid brand id or catalog id");
                    return BadRequest("No catalog or brand associated with id provided");
                }

                var item = new Item()
                {
                    BrandId = itemDto.BrandId,
                    Name = itemDto.Name,
                    CatlogId = itemDto.CatlogId,
                    Cost = itemDto.Cost,
                    ImageUrl = itemDto.ImageUrl,
                };
                Console.WriteLine(item.ToString());
                await _db.Items.AddAsync(item);
                await _db.SaveChangesAsync();
                _logger.LogInformation($"New Item added in database", itemDto);
                return Ok();

            }


            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("Items")]
        public async Task<ActionResult<int>> UpdateItem([FromBody] Item item)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                          .SelectMany(x => x.Errors)
                                          .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }


            var ExsistingItem = _db.Items.FirstOrDefault(x => x.Id == item.Id);
            if (ExsistingItem == null)
            {
                _logger.LogError($"Item with Id {item.Id} not found in database to update");
                return NotFound("Item Does not exist");
            }
            ExsistingItem.Name = item.Name;
            ExsistingItem.Cost = item.Cost;
            ExsistingItem.CatlogId = item.CatlogId;
            ExsistingItem.BrandId = item.BrandId;
            ExsistingItem.ImageUrl = item.ImageUrl;

            var res = await _db.SaveChangesAsync();
            _logger.LogInformation($"item with id {item.Id} updated ");
            return Ok(res);

        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("Items/{id}")]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }

            Item res = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
            if (res == null)
            {
                _logger.LogError($"item with id {id} does not exsist in database");
                return NotFound("Item Does not exsist");
            }
            _logger.LogInformation("Successs");
            return Ok(res);
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("Items/withname/{name}")]
        public async Task<ActionResult<Item>> GetItemByName(string name)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }
            Item res = await _db.Items.FirstOrDefaultAsync(x => x.Name == name);
            if (res == null)
            {
                _logger.LogError($"Item with name {name} does not exist");
                return NotFound($"Item With name {name} does not exsist");
            }
            _logger.LogInformation("Success");
            return Ok(res);
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("Items/type/{catlogTypeId}")]
        public async Task<ActionResult<List<Item>>> GetItemByCatlogId(int catlogTypeId)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }
            var res = await _db.Items.Where(x => x.CatlogId == catlogTypeId).ToListAsync();
            if (res == null || res.Count == 0)
            {
                _logger.LogError($"tried to fetch Item with catalog id {catlogTypeId} which does not exsist in database");
                return NotFound($"Catlog with id {catlogTypeId} not found");
            }
            _logger.LogInformation("Success");
            return Ok(res);
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("Items/type/{catlogTypeId}/brand/{catlogBrandId}")]
        public async Task<ActionResult<List<Item>>> GetItemByCatlogId(int catlogTypeId, int catlogBrandId)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }

            var res = await _db.Items.Where(x => x.CatlogId == catlogTypeId && x.BrandId == catlogBrandId).ToListAsync();
            if (res == null || res.Count == 0)
            {
                _logger.LogError($"attempt to fetch Items with {catlogTypeId} and {catlogBrandId} which does not exsist in databse");
                return NotFound($"item with catlog id {catlogTypeId}Item With name  does not exsist found");

            }
            _logger.LogInformation("Success");
            return Ok(res);
        }


        //[Authorize(Roles = "Admin, User")]
        [HttpGet("Items/type/all/brand/{catlogBrandId}")]
        public async Task<ActionResult<List<Item>>> GetAllItemsOfCatlog(int catlogBrandId)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
                _logger.LogCritical(messages);
                return BadRequest(messages);
            }

            var res = await _db.Items.Where(x => x.BrandId == catlogBrandId).ToListAsync();
            if (res == null || res.Count == 0)
            {
                _logger.LogError($"attempt to fetch items with catalog brand id {catlogBrandId} which doesnt exsist");
                return NotFound($"item with id {catlogBrandId} does not esist");
            }
            _logger.LogInformation("Success");
            return Ok(res);
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("catlogtypes")]
        public async Task<ActionResult<List<Catlog>>> GetAllCatlogs()
        {
            if (ModelState.IsValid)
            {
                var res = await _db.Catlogs.ToListAsync();
                if (res == null || res.Count == 0)
                {
                    _logger.LogError("No catalogs in database");
                    return NotFound($"No catlogs in database");
                }
                _logger.LogInformation("Success");
                return Ok(res);

            }

            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("catlogBrands")]
        public async Task<ActionResult<List<Brand>>> GetAllBrands()
        {
            if (ModelState.IsValid)
            {

                var res = await _db.Brands.ToListAsync();
                if (res == null || res.Count == 0)
                {
                    _logger.LogError("No any brands in database");
                    return NotFound($"No brands in database");
                }
                _logger.LogInformation("Success");
                return Ok(res);

            }
            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogError(messages);
            return BadRequest(messages);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteCatlogById(int id)
        {
            if (ModelState.IsValid)
            {
                var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == id);
                if (item != null)
                {
                    _db.Items.Remove(item);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"Catalog item with id {id} Deleted");

                    return Ok(item);
                }
                _logger.LogError($"Catalog item with id {id} not found ");
                return NotFound("Item Does not exist");
            }

            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddCatalog")]
        public async Task<ActionResult> AddCatalog([FromBody] CatalogDto catlogDto)
        {
            if (ModelState.IsValid)
            {

                var exists = await _db.Catlogs.FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == catlogDto.Name.ToLower().Trim());
                if (exists != null)
                {
                    _logger.LogInformation($"Tried to add catlog type wit same name -{catlogDto.Name} already exist in db");
                    return BadRequest($"catlog type with name {catlogDto.Name} already exist");
                }


                await _db.Catlogs.AddAsync(new Catlog()
                {
                    Name = catlogDto.Name
                });
                await _db.SaveChangesAsync();
                _logger.LogInformation($"new Catalog added in databse {catlogDto.Name} ");
                return Ok();

            }

            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogError(messages);
            return BadRequest(messages);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddBrand")]
        public async Task<ActionResult> AddBrand([FromBody] BrandDto brandDto)
        {
            if (ModelState.IsValid)
            {
                var exists = await _db.Brands.FirstOrDefaultAsync(c => c.Name.ToLower().Trim() == brandDto.Name.ToLower().Trim());
                if (exists != null)
                {
                    _logger.LogInformation($"Tried to add brand with same name -{brandDto.Name} already exist in db");
                    return BadRequest($"brand with name {brandDto.Name} already exist");
                }

                await _db.Brands.AddAsync(new Brand()
                {
                    Name = brandDto.Name
                });
                _logger.LogInformation($"New Brand {brandDto.Name} added in databse");
                await _db.SaveChangesAsync();
                return Ok();

            }

            string messages = string.Join("; ", ModelState.Values
                                       .SelectMany(x => x.Errors)
                                       .Select(x => x.ErrorMessage));
            _logger.LogCritical(messages);
            return BadRequest(messages);
        }



    }
}

