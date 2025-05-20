using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;
       public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }


        //POST:/api/Auth/Register
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var identityUser = new IdentityUser()
            {
                UserName = registerRequestDTO.Username,
                Email = registerRequestDTO.Username
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDTO.Password);
            if (identityResult.Succeeded)
            {
                //Add roles to this user
                if (registerRequestDTO.Roles != null && registerRequestDTO.Roles.Any())
                {
                    identityResult = await userManager.AddToRolesAsync(identityUser, registerRequestDTO.Roles);
                    if (identityResult.Succeeded)
                    {
                        return Ok("User was registered! Please login");
                    }

                }

            }
            return BadRequest("Something went wrong");
        }
        //POST:/api/Auth/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var user= await userManager.FindByEmailAsync(loginRequestDTO.UserName);
            if(user!=null)
            {
               var checkedPasswordResult = await userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
                if(checkedPasswordResult)
                {
                    //Get Roles for this User
                    var roles= await userManager.GetRolesAsync(user);
                    if (roles != null)
                    {
                        //create token
                        var jwtToken=tokenRepository.CreateJWTToken(user, roles.ToList());
                        var responseDTO = new LoginResponseDTO()
                        {
                            JWTToken = jwtToken
                        };
                        return Ok(responseDTO);
                    }

                   
                }
            }
            return BadRequest("Username or Password is incorrect");

        }
    }
}
