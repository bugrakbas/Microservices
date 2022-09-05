using Data.Context;
using Data.DTOs.Request;
using Data.Models;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DBContext _dbContext;

        public OrderController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> Create(OrderRequestDto requestDto)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dbContext))
                {
                    unitOf.GetRepository<Order>().Add(new Order()
                    {
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        CustomerId = requestDto.CustomerId,
                        Price = requestDto.Price,
                        Quantity = requestDto.Quantity,
                    });
                    if (unitOf.SaveChanges() == 1)
                        return Ok();

                    else
                        return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> Update(OrderRequest requestDto)
        {
            try
            {
                var order = new Order();
                using (UnitOfWork unitOf = new UnitOfWork(new DBContext()))
                {
                    order = unitOf.GetRepository<Order>().Get(requestDto.OrderId);
                }
                using (UnitOfWork unitOf = new UnitOfWork(_dbContext))
                {
                    unitOf.GetRepository<Order>().Update(new Order()
                    {
                        Id = requestDto.OrderId,
                        CustomerId = requestDto.CustomerId,
                        Price = requestDto.Price,
                        Quantity = requestDto.Quantity,
                        Status = requestDto.Status,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = order.CreatedAt
                    });
                    if (unitOf.SaveChanges() == 1)
                        return Ok();
                    else
                        return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteOrder")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dbContext))
                {
                    unitOf.GetRepository<Order>().Delete(unitOf.GetRepository<Order>().Get(Id));
                    if (unitOf.SaveChanges() == 1)
                        return Ok();
                    else
                        return BadRequest();
                };

            }
            catch (Exception ex)
            {
                return BadRequest();

            }

        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> Get(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dbContext))
                {
                    var order = unitOf.GetRepository<Order>().Get(Id);
                    if (order != null)
                        return Ok();
                    else
                        return NotFound();
                }
            }

            catch (Exception ex)
            {
                return NoContent();
            }
        }

        [HttpGet("GetOrderAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dbContext))
                {
                    List<Order> orders = new List<Order>();
                    foreach (var item in unitOf.GetRepository<Order>().GetAll().ToList())
                    {
                        orders.Add(new Order()
                        {
                            CreatedAt = item.CreatedAt,
                            CustomerId = item.CustomerId,
                            Price = item.Price,
                            Quantity = item.Quantity,
                            Status = item.Status,
                            UpdatedAt = item.UpdatedAt,
                            Id = item.Id
                        });
                    }
                    if (orders.Count() > 0)
                        return Ok();
                    else
                        return NoContent();
                }
            }
            catch (Exception ex)
            {
                return NoContent();
            }
        }

    }
}
