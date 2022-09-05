using Data.Context;
using Data.DTOs.Request;
using Data.DTOs.Response;
using Data.Jwt;
using Data.Models;
using DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CustomerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DBContext _dBContext;

        public CustomerController(DBContext dBContext)
        {
            _dBContext = dBContext;
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> CreateCustomer(CustomerRequestDto requestDto)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    unitOf.GetRepository<Customer>().Add(new Customer()
                    {
                        Email = requestDto.Email,
                        Name = requestDto.Name,
                        Password = requestDto.Password,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
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

        [HttpPut("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer(CustomerRequest requestDto)
        {
            try
            {
                var customer = new Customer();
                using (UnitOfWork unitOf = new UnitOfWork(new DBContext()))
                {
                    customer = unitOf.GetRepository<Customer>().Get(requestDto.CustomerId);
                }
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    unitOf.GetRepository<Customer>().Update(new Customer()
                    {
                        Id = requestDto.CustomerId,
                        Name = requestDto.Name,
                        Password = requestDto.Password,
                        Email = requestDto.Email,
                        UpdatedAt = DateTime.Now,
                        CreatedAt = customer.CreatedAt
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

        [HttpDelete("DeleteCustomer")]
        public async Task<IActionResult> DeleteCustomer(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    unitOf.GetRepository<Customer>().Delete(unitOf.GetRepository<Customer>().Get(Id));
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

        [HttpGet("GetCustomer")]
        public async Task<IActionResult> GetCustomer(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    var customer = unitOf.GetRepository<Customer>().Get(Id);
                    if (customer != null)
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

        [HttpGet("GetCustomerAll")]
        public async Task<IActionResult> GetCustomerAll()
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    List<Customer> customers = new List<Customer>();
                    foreach (var item in unitOf.GetRepository<Customer>().GetAll().ToList())
                    {
                        customers.Add(new Customer()
                        {
                            CreatedAt = item.CreatedAt,
                            CurrentToken = item.CurrentToken,
                            Email = item.Email,
                            Id = item.Id,
                            Name = item.Name,
                            Password = item.Password,
                            UpdatedAt = item.UpdatedAt
                        });
                    }
                    if (customers.Count() > 0)
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

        [HttpGet("Validate")]
        public async Task<IActionResult> Validate(string email, string password)
        {
            try
            {
                var customer = new Customer();
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    customer = unitOf.GetRepository<Customer>().Get(x => x.Email == email);
                }
                using (UnitOfWork unitOf = new UnitOfWork(new DBContext()))
                {
                    if (customer != null)
                    {
                        var token = new JwtManager(_dBContext).Get(email, password);
                        if (!string.IsNullOrEmpty(token))
                        {
                            unitOf.GetRepository<Customer>().Update(new Customer()
                            {
                                Id = customer.Id,
                                Name = customer.Name,
                                Email = customer.Email,
                                UpdatedAt = DateTime.Now,
                                CreatedAt = customer.CreatedAt,
                                CurrentToken = token,
                            });
                            unitOf.SaveChanges();
                            return Ok();
                        }
                      return Unauthorized();
                    }
                    else
                        return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }

        }
    }
}
