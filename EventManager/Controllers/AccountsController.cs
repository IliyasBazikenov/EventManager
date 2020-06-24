using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IMapper _mapper;
        public AccountsController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAccounts()
        {
            try
            {
                var accounts = _repository.Account.GetAllAccounts(trackChanges: false);
                var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

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