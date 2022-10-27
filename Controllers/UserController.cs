using entrance_test.Model;
using WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApi.Controller
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("~/api/signup")]
        public IActionResult Signup([FromBody] UserRequest userRequest)
        {
            Validation.Validation validation = new Validation.Validation();
            if(String.IsNullOrEmpty(userRequest.email))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "please enter email!");
            }
            
            if(String.IsNullOrEmpty(userRequest.passwrord))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "please enter password");
            }

            if(!validation.IsValidEmail(userRequest.email))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "email wrong format");
            }
            
            if(_userService.GetByEmail(userRequest.email) != null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "email is available");
            }
            
            if(userRequest.passwrord.Length <= 8 && userRequest.passwrord.Length >= 20)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Password must be between 8-20 characters");
            }

            var response = _userService.SaveUser(userRequest);
            if(response != null)
            {
                return StatusCode(StatusCodes.Status201Created, response);
            }    
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }    
        }

        [HttpPost]
        [Route("~/api/signin")]
        public IActionResult signin([FromBody] SignInRequest signInRequest)
        {
            Validation.Validation validation = new Validation.Validation();
            if (String.IsNullOrEmpty(signInRequest.email))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "please enter email");
            }
            
            if(String.IsNullOrEmpty(signInRequest.password))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "please enter password");
            }

            if (!validation.IsValidEmail(signInRequest.email))
            {
                return StatusCode(StatusCodes.Status400BadRequest, "email wrong format");
            }

            if (_userService.GetByEmail(signInRequest.email) == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "email not exits");
            }

            if (signInRequest.password.Length <= 8 && signInRequest.password.Length >= 20)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Password must be between 8-20 characters");
            }

            var response = _userService.SignIn(signInRequest);
            if(response != null)
            {
                return StatusCode(StatusCodes.Status200OK, response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }    
        }

        [Authorize]
        [HttpPost]
        [Route("~/api/signout")]
        public IActionResult SignOut()
        {
            try
            {
                HttpContext.Session.Clear();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Route("~/api/refresh-token")]
        public IActionResult RefreshToken(TokenRequest tokenRequest)
        {
            if(_userService.GetByRefreshToken(tokenRequest.refreshToken) == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "refreshToken in the inbound does not exist");
            }

            var reponse = _userService.RefreshToken(tokenRequest);

            if(reponse != null)
            {
                return StatusCode(StatusCodes.Status200OK, reponse);
            }                
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }           
        }
    }
}
