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
    public class OrderController : ControllerBase
    {

        private readonly ApplicationDbContext _db;
        private readonly ILogger<CatlogController> _logger;

        public OrderController(ApplicationDbContext db, ILogger<CatlogController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "User")]

        [HttpPost("AddOrder")]
        public async Task<ActionResult> AddOrder([FromBody] AddOrderDto addOrderDto)
        {
            int totalCost = 0;
            Order order = new Order()
            {
                UserId = addOrderDto.UserId,
            };

            await _db.Orders.AddAsync(order);

            await _db.SaveChangesAsync();

            foreach (var i in addOrderDto.OrderItems)
            {
                _db.OrderItems.Add(new OrderItem()
                {
                    ItemId = i.ItemId,
                    OrderId = order.Id,
                    ItemQty = i.Qty
                });

            }

            foreach (var i in addOrderDto.OrderItems)
            {
                totalCost += _db.Items.FirstOrDefault(x => x.Id == i.ItemId).Cost * i.Qty;
            }
            await _db.SaveChangesAsync();

            OrderDetail orderDetail = new OrderDetail()
            {
                OrderId = order.Id,
                Address = addOrderDto.Address,
                OrderCost = totalCost,
                OrderStatus = addOrderDto.OrderStatus,
                TotalItems = addOrderDto.TotalQty,
                Remark = addOrderDto.Remark,
                ShippingCharges = 60 //constant shipping charges 
            };
            _db.OrderDetails.Add(orderDetail);
            await _db.SaveChangesAsync();

            return Ok(orderDetail.OrderId);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("SetOrderDelivered/{orderId}")]
        public async Task<ActionResult> SetOrderDelivered(int orderId)
        {

            OrderDetail orderDetailAssociatedWithOrder = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (orderDetailAssociatedWithOrder == null)
            {
                return NotFound($"No order associated with id {orderId}");
            }

            orderDetailAssociatedWithOrder.OrderStatus = "Delivered";
            _db.SaveChanges();

            return Ok("Order delivered id " + orderId);

        }
       
        [HttpPut("SetOrderShipped/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetOrderShipped(int orderId)
        {
            OrderDetail orderDetailAssociatedWithOrder = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (orderDetailAssociatedWithOrder == null)
            {
                return NotFound($"No order associated with id {orderId}");
            }

            orderDetailAssociatedWithOrder.OrderStatus = "Shipped";
            _db.SaveChanges();

            return Ok("Order delivered id " + orderId);
        }

        [Authorize(Roles = "Admin, User")]
        [HttpPut("SetOrderCancelled/{orderId}")]
        public async Task<ActionResult> SetOrderCancelled(int orderId)
        {

            OrderDetail orderDetailAssociatedWithOrder = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (orderDetailAssociatedWithOrder == null)
            {
                return NotFound($"No order associated with id {orderId}");
            }

            orderDetailAssociatedWithOrder.OrderStatus = "Cancelled";
            _db.SaveChanges();

            return Ok("Order delivered id " + orderId);

        }
        
        [HttpPut("SetRemarkPaid/{orderId}/")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> SetRemarkPaid(int orderId, [FromBody] RemarkDto remark)
        {
            OrderDetail orderDetailAssociatedWithOrder = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            if (orderDetailAssociatedWithOrder == null)
            {
                return NotFound($"No order associated with id {orderId}");
            }

            orderDetailAssociatedWithOrder.Remark = remark.Remark;
            await _db.SaveChangesAsync();
            return Ok(orderId);
        }

        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllOrders()
        {
            List<Order> orders = _db.Orders.ToList();
            List<OrderDto> responce = new List<OrderDto>();

            foreach (var order in orders)
            {
                User userAssociatedWithOrder = await _db.Users.FirstOrDefaultAsync(x => x.Id == order.UserId);
                OrderDetail orderDetail = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == order.Id);
                List<OrderDtoItem> itemsAssociatedWithOrder = new();

                List<OrderItem> orderItemsAssociatedWithOid = _db.OrderItems.Where(x => x.OrderId == order.Id).ToList();

                itemsAssociatedWithOrder.AddRange(from item in orderItemsAssociatedWithOid
                                                  let itemFromOrder = _db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault()
                                                  select new OrderDtoItem()
                                                  {
                                                      Name = itemFromOrder.Name,
                                                      BrandId = itemFromOrder.BrandId,
                                                      CatlogId = itemFromOrder.CatlogId,
                                                      Cost = itemFromOrder.Cost,
                                                      ImageUrl = itemFromOrder.ImageUrl,
                                                      Description = itemFromOrder.Description,
                                                      itemQty = item.ItemQty
                                                  });


                //responce.Add(orderDto);
                OrderDto orderDto = new OrderDto()
                {
                    Id = order.Id,
                    UserId = userAssociatedWithOrder.Id,
                    UserName = userAssociatedWithOrder.UserName,
                    UserPhoneNo = userAssociatedWithOrder.PhoneNumber,
                    Address = orderDetail.Address,
                    OrderItems = itemsAssociatedWithOrder,  //list
                    TotalCost = orderDetail.OrderCost,
                    OrderStatus = orderDetail.OrderStatus,
                    Remark = orderDetail.Remark,
                    ShippingChareges = orderDetail.ShippingCharges
                };

                responce.Add(orderDto);

            }

            return Ok(responce);
        }

        [HttpGet("GetOrderDetails/{orderId}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult> GetOrder(int orderId)
        {

            Order orderAssociatedWithOrderId = await _db.Orders.Where(x => x.Id == orderId).FirstOrDefaultAsync();
            User userAssociatedWithOrder = await _db.Users.FirstOrDefaultAsync(x => x.Id == orderAssociatedWithOrderId.UserId);
            OrderDetail orderDetail = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == orderId);
            List<OrderDtoItem> itemsAssociatedWithOrder = new();

            List<OrderItem> orderItemsAssociatedWithOid = _db.OrderItems.Where(x => x.OrderId == orderId).ToList();

            itemsAssociatedWithOrder.AddRange(from item in orderItemsAssociatedWithOid
                                              let itemFromOrder = _db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault()
                                              select new OrderDtoItem()
                                              {
                                                  Name = itemFromOrder.Name,
                                                  BrandId = itemFromOrder.BrandId,
                                                  CatlogId = itemFromOrder.CatlogId,
                                                  Cost = itemFromOrder.Cost,
                                                  ImageUrl = itemFromOrder.ImageUrl,
                                                  Description = itemFromOrder.Description,
                                                  itemQty = item.ItemQty
                                              });

            OrderDto orderDto = new OrderDto()
            {
                Id = orderId,
                UserId = userAssociatedWithOrder.Id,
                UserName = userAssociatedWithOrder.UserName,
                UserPhoneNo = userAssociatedWithOrder.PhoneNumber,
                Address = orderDetail.Address,
                OrderItems = itemsAssociatedWithOrder,  //list
                TotalCost = orderDetail.OrderCost, //without shipping cosy
                OrderStatus = orderDetail.OrderStatus,
                Remark = orderDetail.Remark,
                ShippingChareges = orderDetail.ShippingCharges
            };

            return Ok(orderDto);

        }

        [HttpGet("GetOrderDetailsForUser/{userId}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetOrderForUser(int userId)
        {
            List<Order> orderAssociatedWithUserId = _db.Orders.Where(x => x.UserId == userId).ToList();
            User userAssociatedWithOrder = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            List<OrderDto> responce = new List<OrderDto>();

            foreach (var order in orderAssociatedWithUserId)
            {
                OrderDetail orderDetail = await _db.OrderDetails.FirstOrDefaultAsync(x => x.OrderId == order.Id);
                List<OrderDtoItem> itemsAssociatedWithOrder = new();

                List<OrderItem> orderItemsAssociatedWithOid = _db.OrderItems.Where(x => x.OrderId == order.Id).ToList();

                itemsAssociatedWithOrder.AddRange(from item in orderItemsAssociatedWithOid
                                                  let itemFromOrder = _db.Items.Where(x => x.Id == item.ItemId).FirstOrDefault()
                                                  select new OrderDtoItem()
                                                  {
                                                      Name = itemFromOrder.Name,
                                                      BrandId = itemFromOrder.BrandId,
                                                      CatlogId = itemFromOrder.CatlogId,
                                                      Cost = itemFromOrder.Cost,
                                                      ImageUrl = itemFromOrder.ImageUrl,
                                                      Description = itemFromOrder.Description,
                                                      itemQty = item.ItemQty
                                                  });


                //responce.Add(orderDto);
                OrderDto orderDto = new OrderDto()
                {
                    Id = order.Id,
                    UserId = userId,
                    UserName = userAssociatedWithOrder.UserName,
                    UserPhoneNo = userAssociatedWithOrder.PhoneNumber,
                    Address = orderDetail.Address,
                    OrderItems = itemsAssociatedWithOrder,  //list
                    TotalCost = orderDetail.OrderCost,
                    OrderStatus = orderDetail.OrderStatus,
                    Remark = orderDetail.Remark,
                    ShippingChareges = orderDetail.ShippingCharges
                };

                responce.Add(orderDto);

            }




            return Ok(responce);
        }

        //[Authorize(Roles = "User")]

    }
}
