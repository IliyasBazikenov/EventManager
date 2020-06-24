using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public AccountsController(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            try
            {
                var accounts = _repository.Account.GetAllAccounts(trackChanges: false);
                var accountsDTO = accounts.Select(a => new AccountDTO
                {
                    AccountId = a.AccountId,
                    FirstName = a.FirstName,
                    SecondName = a.SecondName,
                    Email = a.Email,
                    Address = a.Address,
                    LastName = a.LastName,
                    BirthDate = a.BirthDate,
                    PhoneNumber = a.PhoneNumber
                });

                return Ok(accountsDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something gone wrong in the {nameof(GetAccounts)} action {ex}");

                return StatusCode(500, "Internal server error");
            }
        }
    }
}