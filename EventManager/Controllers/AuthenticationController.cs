using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using EventManager.ActionFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<Account> _userManager;
        private readonly RoleManager<AccountRole> _roleManager;

        public AuthenticationController(ILoggerManager logger, IMapper mapper,
            UserManager<Account> userManager, RoleManager<AccountRole> roleManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterAccount([FromBody] AccountForRegistrationDTO accountForRegistration)
        {
            var account = _mapper.Map<Account>(accountForRegistration);

            var result = await _userManager.CreateAsync(account, accountForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return BadRequest(ModelState);
            }

            foreach (var item in accountForRegistration.Roles)
            {
                if(!_roleManager.RoleExistsAsync(item).Result)
                    return BadRequest(ModelState);
            }
            await _userManager.AddToRolesAsync(account, accountForRegistration.Roles);

            return StatusCode(201);
        } 
    }
}
