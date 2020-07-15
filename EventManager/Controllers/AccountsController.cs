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
using Microsoft.AspNetCore.JsonPatch;
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
        public async  Task<IActionResult> GetAccounts()
        {
            var accounts = await _repository.Account.GetAllAccountsAsync(trackChanges: false);
            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpGet("{accountId}", Name = "AccountById")]
        public async Task<IActionResult> GetAccount(Guid accountId)
        {
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }
            else
            {
                var accountDTO = _mapper.Map<AccountDTO>(account);
                return Ok(accountDTO);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountForCreationDTO account)
        {
            if (account == null)
            {
                _logger.LogError("AccountForCreationDTO object sent from client is null.");
                return BadRequest("Account object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the AccountForCreationDTO.");
                return UnprocessableEntity(ModelState);
            }

            var accountEntity = _mapper.Map<Account>(account);
            _repository.Account.CreateAccount(accountEntity);
            await _repository.SaveAsync();

            var accountToReturn = _mapper.Map<AccountDTO>(accountEntity);

            return CreatedAtRoute("AccountById", new { accountId = accountToReturn.AccountId }, accountToReturn);
        }

        [HttpGet("collection/({accountIds})", Name = "AccountCollection")]
        public async Task<IActionResult> GetAccountCollection([ModelBinder(BinderType =
            typeof(ArrayModelBinder))]IEnumerable<Guid> accountIds)
        {
            if (accountIds == null)
            {
                _logger.LogError("Parameter accountId from the client is null.");
                return BadRequest("Parameter accountId is null");
            }

            var accounts = await _repository.Account.GetByIdsAsync(accountIds, trackChanges: false);

            if (accountIds.Count() != accounts.Count())
            {
                _logger.LogError("Some accountIds ids are not valid in a collection.");
                return NotFound();
            }

            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateAccountCollection([FromBody] IEnumerable<AccountForCreationDTO> accountForCreationDTOs)
        {
            if (accountForCreationDTOs == null)
            {
                _logger.LogError("Account collection sent from client is null.");
                return BadRequest("Account collection is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the AccountForCreationDTO.");
                return UnprocessableEntity(ModelState);
            }

            var accounts = _mapper.Map<IEnumerable<Account>>(accountForCreationDTOs);

            foreach (var account in accounts)
            {
                _repository.Account.CreateAccount(account);
            }
            await _repository.SaveAsync();

            var accountDTOs = _mapper.Map<IEnumerable<AccountDTO>>(accounts);
            var accountIds = string.Join(",", accountDTOs.Select(a => a.AccountId));

            return CreatedAtRoute("AccountCollection", new { accountIds }, accountDTOs);
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(Guid accountId)
        {
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges: false);
            if (account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            _repository.Account.DeleteAccount(account);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateAccount(Guid accountId, [FromBody]AccountForUpdateDTO account)
        {
            if(account == null)
            {
                _logger.LogError("Account object sent from client is null.");
                return BadRequest("Account object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the AccountForUpdateDTO.");
                return UnprocessableEntity(ModelState);
            }

            var accountEntity = await _repository.Account.GetAccountAsync(accountId, true);
            if(accountEntity == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            _mapper.Map(account, accountEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{accountId}")]
        public async Task<IActionResult> PartiallyUpdateAccount(Guid accountId, 
            [FromBody]JsonPatchDocument<AccountForUpdateDTO> patchDocument)
        {
            if(patchDocument == null)
            {
                _logger.LogError($"Account patchDocument object sent from client is null.");
                return BadRequest("PatchDocument is null");
            }

            var account = await _repository.Account.GetAccountAsync(accountId, true);
            if(account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the databse.");
                return NotFound();
            }

            var accountToPatch = _mapper.Map<AccountForUpdateDTO>(account);

            TryValidateModel(accountToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogInfo("Invalid model state for the AccountForUpdateDTO");
                return UnprocessableEntity();
            }

            _mapper.Map(accountToPatch, account);
            await _repository.SaveAsync();

            return NoContent();
        }

    }
}