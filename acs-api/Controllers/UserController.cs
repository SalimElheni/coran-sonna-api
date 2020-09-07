using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ACS.Controllers
{
    using ACS.Services;
    using ACS.Models;
    using Microsoft.AspNetCore.Authorization;
    using Renci.SshNet.Common;
    using ACS.Helpers;

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] ACS.Models.RegisterModel model)
        {
            try
            {
                if (!RegexUtils.IsValidEmail(model.Username))
                {
                    return Ok(ResponseModel<UserModel>.ErrorResponse(null, new List<string> { "Username invalid" }));
                }

                var user = _userService.Create(model);
                return Ok(ResponseModel<UserModel>.SuccessResponse(user));
            }
            catch (ScpException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return Ok(ResponseModel<UserModel>.ErrorResponse(user, new List<string> { "Username or password is incorrect" }));

            return Ok(ResponseModel<UserModel>.SuccessResponse(user));
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public IActionResult Confirm(string unm, string uuid)
        {
            var userResponse = _userService.ConfirmAccount(unm, uuid);

            return Ok(ResponseModel<bool>.SuccessResponse(userResponse));
        }
        [AllowAnonymous]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var user = _userService.GetAll();

            return Ok(ResponseModel<List<UserModel>>.SuccessResponse(user));
        }


        [AllowAnonymous]
        [HttpPost("auth")]
        public IActionResult Authenticate_id([FromBody] AuthModel model)
        {
            if (model != null && model.Passphrase == IUserService.MyPassPhrase)
                return Ok(ResponseModel<UserModel>.SuccessResponse(null));

            return Ok(ResponseModel<UserModel>.ErrorResponse(null));
        }
    }
}



