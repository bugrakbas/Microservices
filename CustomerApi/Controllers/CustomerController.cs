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
        public async Task<ResponseDto<string>> CreateCustomer(CustomerRequestDto requestDto)
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
                        return new ResponseDto<string>()
                        {
                            IsSuccess = true,
                            Message = "Completed",
                            Status = HttpStatusCode.Created
                        };
                    else
                        return new ResponseDto<string>()
                        {
                            IsSuccess = false,
                            Message = "Fail",
                            Status = HttpStatusCode.BadRequest
                        };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Status = HttpStatusCode.BadRequest
                };
            }

        }

        [HttpPut("UpdateCustomer")]
        public async Task<ResponseDto<string>> UpdateCustomer(UpdateCustomerRequestDto requestDto)
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
                        return new ResponseDto<string>()
                        {
                            IsSuccess = true,
                            Message = "Completed",
                            Status = HttpStatusCode.OK
                        };
                    else
                        return new ResponseDto<string>()
                        {
                            IsSuccess = false,
                            Message = "Fail",
                            Status = HttpStatusCode.BadRequest
                        };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<string>()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Status = HttpStatusCode.BadRequest
                };
            }
        }

        [HttpDelete("DeleteCustomer")]
        public async Task<ResponseDto<string>> DeleteCustomer(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    unitOf.GetRepository<Customer>().Delete(unitOf.GetRepository<Customer>().Get(Id));
                    if (unitOf.SaveChanges() == 1)
                        return new ResponseDto<string>()
                        {
                            Status = HttpStatusCode.OK,
                            Message = "Success",
                            IsSuccess = true
                        };
                    else
                        return new ResponseDto<string>()
                        {
                            Status = HttpStatusCode.BadRequest,
                            Message = "Failed",
                            IsSuccess = false
                        };
                };

            }
            catch (Exception ex)
            {
                return new ResponseDto<string>()
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = ex.Message,
                    IsSuccess = false
                }; ;

            }

        }

        [HttpGet("GetCustomer")]
        public async Task<ResponseDto<GetCustomerResponseDto>> GetCustomer(Guid Id)
        {
            try
            {
                using (UnitOfWork unitOf = new UnitOfWork(_dBContext))
                {
                    var customer = unitOf.GetRepository<Customer>().Get(Id);
                    if (customer != null)
                        return new ResponseDto<GetCustomerResponseDto>()
                        {
                            Data = new GetCustomerResponseDto()
                            {
                                Id = Id,
                                Email = customer.Email,
                                Name = customer.Name,
                                CreatedAt = customer.CreatedAt,
                                UpdatedAt = customer.UpdatedAt
                            },
                            IsSuccess = true,
                            Message = "Completed",
                            Status = HttpStatusCode.OK
                        };
                    else
                        return new ResponseDto<GetCustomerResponseDto>()
                        {
                            IsSuccess = false,
                            Message = "Not found",
                            Status = HttpStatusCode.NoContent
                        };
                }
            }

            catch (Exception ex)
            {
                return new ResponseDto<GetCustomerResponseDto>()
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Status = HttpStatusCode.NoContent
                };
            }
        }

        [HttpGet("GetCustomerAll")]
        public async Task<ResponseDto<List<Customer>>> GetCustomerAll()
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
                        return new ResponseDto<List<Customer>>
                        {
                            IsSuccess = true,
                            Data = customers,
                            Message = "Success",
                            Status = HttpStatusCode.OK
                        };
                    else
                        return new ResponseDto<List<Customer>>
                        {
                            IsSuccess = false,
                            Message = "No content",
                            Status = HttpStatusCode.NoContent
                        };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<List<Customer>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Status = HttpStatusCode.NoContent
                };
            }
        }

        [HttpGet("Validate")]
        public async Task<ResponseDto<ValidateResponse>> Validate(string email, string password)
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
                            return new ResponseDto<ValidateResponse>
                            {
                                Data = new ValidateResponse() { Token = token },
                                IsSuccess = true,
                                Message = "Success",
                                Status = HttpStatusCode.OK
                            };
                        }
                        return new ResponseDto<ValidateResponse>
                        {
                            IsSuccess = false,
                            Message = "Failed",
                            Status = HttpStatusCode.Unauthorized
                        };
                    }
                    else
                        return new ResponseDto<ValidateResponse>
                        {
                            IsSuccess = false,
                            Message = "Failed",
                            Status = HttpStatusCode.Unauthorized
                        };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<ValidateResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Status = HttpStatusCode.Unauthorized
                };
            }

        }
    }
}
