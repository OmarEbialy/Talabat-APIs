using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    public class AccountController : BaseAPIsController
    {
        private readonly UserManager<AppUser>   _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController
            (UserManager<AppUser> userManager , SignInManager<AppUser> signInManager , 
            ITokenService tokenService , IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("login")]  // .../api/Account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            });
        }

        [HttpPost("register")] // .../api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExist(registerDto.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "Email Already In Use !" } });


            var user = new AppUser()
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,                  // hossammostafa@gmail.com
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.Email.Split("@")[0]  // hossammostafa
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user  = await _userManager.FindByEmailAsync(email);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet("address")] // .../api/account/address
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserAddressByEmailAsync(User);

            var address = _mapper.Map<Address , AddressDto>(user.Address);

            return Ok(address);
        }

        [Authorize]
        [HttpPut("address")] // PUT :  .../api/account/address
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto updatedAddress)
        {
            var address = _mapper.Map<AddressDto,Address>(updatedAddress);

            var user = await _userManager.FindUserAddressByEmailAsync(User);

            address.Id = user.Address.Id;

            user.Address = address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return Ok(updatedAddress);
        }

        [HttpGet("emailexist")]
        public async Task<ActionResult<bool>> CheckEmailExist(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;

        }
    }
}
