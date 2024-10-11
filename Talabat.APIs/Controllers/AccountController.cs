using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.contract;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IAuthService authService, 
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")] // api/Account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded is false)
            {
                return Unauthorized(new ApiResponse(401));
            }
            else
            {
                return (new UserDto
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token = await _authService.CreateTokenAsynk(user, _userManager)
                });
            }
        }

        [HttpPost("register")] // api/Account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
            {
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] {"this email is already exist!"} });
            }

            var user = new AppUser
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,
            };

            var result =await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded is false)
            {
                return Unauthorized(new ApiResponse(400));
            }
            else
            {
                return (new UserDto
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token = await _authService.CreateTokenAsynk(user, _userManager)
                });
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Token = await _authService.CreateTokenAsynk(user, _userManager)
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            //var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var user = await _userManager.FindUserWithAddressByEmailAsync(User);

            var mappedAddress = _mapper.Map<Address, AddressDto>(user.Address);

            return Ok(mappedAddress);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("address")]
        public async Task<ActionResult<Address>> UpdateUserAddress(AddressDto address)
        {
            var updatedAddress = _mapper.Map<AddressDto, Address>(address);

            var user = await _userManager.FindUserWithAddressByEmailAsync(User);

            updatedAddress.Id = user.Address.Id;

            user.Address = updatedAddress;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) 
            { 
                return BadRequest(400);
            }
            else 
            { 
                return Ok(address); 
            }
        }


        [HttpGet("emailExist")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
    }
}
