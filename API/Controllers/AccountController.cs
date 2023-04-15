
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController: BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService,IUserRepository userRepository, IMapper mapper)
        {
            _context=context;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        [HttpPost("register")] //POST: api/account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)

        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");
            var user =_mapper.Map<AppUser>(registerDto);
            using var hmac = new HMACSHA512();
            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            // var user = new AppUser
            // {
            //     UserName = registerDto.Username.ToLower(),
            //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            //     PasswordSalt = hmac.Key
            // };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
                //PhotoUrl = user.Photos.FirstOrDefault(x =>x.IsMain)?.Url
            };
        }
        [HttpPost("login")] 
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto){
            //var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName ==loginDto.Username.ToLower());
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username.ToLower());
            if (user==null) return Unauthorized("invalid username");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            if (computedHash.Length != user.PasswordHash.Length) 
                return Unauthorized("invalid password (length)");
            for(int i=0; i<computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x =>x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };

        }
        private async Task<bool> UserExists (string username)
        {
            return await _context.Users.AnyAsync(user=>user.UserName==username.ToLower());
        }
    }
}