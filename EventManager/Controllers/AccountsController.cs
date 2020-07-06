using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using EventManager.ModelBinders;
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
            var accounts = _repository.Account.GetAllAccounts(trackChanges: false);
            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpGet("{accountId}", Name = "AccountById")]
        public IActionResult GetAccount(Guid accountId)
        {
            var account = _repository.Account.GetAccount(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse");
                return NotFound();
            }
            else
            {
                var accountDTO = _mapper.Map<AccountDTO>(account);
                return Ok(accountDTO);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] AccountForCreationDTO account)
        {
            if (account == null)
            {
                _logger.LogError("AccountForCreationDTO object sent from client is null");
                return BadRequest("Account object is null");
            }

            var accountEntity = _mapper.Map<Account>(account);
            _repository.Account.CreateAccount(accountEntity);
            _repository.Save();

            var accountToReturn = _mapper.Map<AccountDTO>(accountEntity);

            return CreatedAtRoute("AccountById", new { accountId = accountToReturn.AccountId }, accountToReturn);
        }

        [HttpGet("collection/({accountIds})", Name = "AccountCollection")]
        public IActionResult GetAccountCollection([ModelBinder(BinderType = 
            typeof(ArrayModelBinder))]IEnumerable<Guid> accountIds)
        {
            if (accountIds == null)
            {
                _logger.LogError("AccountId collection from the client is null");
                return BadRequest("AccountId collection from the client is null");
            }

            var accounts = _repository.Account.GetByIds(accountIds, trackChanges: false);

            if (accountIds.Count() != accounts.Count())
            {
                _logger.LogError("Some accountIds ids are not valid in a collection!");
                return NotFound();
            }

            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpPost("collection")]
        public IActionResult CreateAccountCollection([FromBody] IEnumerable<AccountForCreationDTO> accountForCreationDTOs)
        {
            if (accountForCreationDTOs == null)
            {
                _logger.LogError("Account collection sent from client is null");
                return BadRequest("Account collection is null");
            }

            var accounts = _mapper.Map<IEnumerable<Account>>(accountForCreationDTOs);

            foreach (var account in accounts)
            {
                _repository.Account.CreateAccount(account);
            }
            _repository.Save();

            var accountDTOs = _mapper.Map<IEnumerable<AccountDTO>>(accounts);
            var accountIds = string.Join(",", accountDTOs.Select(a => a.AccountId));

            return CreatedAtRoute("AccountCollection", new { accountIds }, accountDTOs);
        }

    }
}