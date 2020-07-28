using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using EventManager.ActionFilters;
using EventManager.ModelBinders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _repository.Account.GetAllAccountsAsync(trackChanges: false);
            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpGet("{accountId}", Name = "AccountById")]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public IActionResult GetAccount(Guid accountId)
        {
            var account = HttpContext.Items["account"];

            var accountDTO = _mapper.Map<AccountDTO>(account);
            return Ok(accountDTO);
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
                _logger.LogError("Some accountIds are not valid in a collection.");
                return NotFound();
            }

            var accountsDTO = _mapper.Map<IEnumerable<AccountDTO>>(accounts);

            return Ok(accountsDTO);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateAccount([FromBody] AccountForCreationDTO account)
        {
            var accountEntity = _mapper.Map<Account>(account);

            _repository.Account.CreateAccount(accountEntity);
            await _repository.SaveAsync();

            var accountToReturn = _mapper.Map<AccountDTO>(accountEntity);

            return CreatedAtRoute("AccountById", new { accountId = accountToReturn.AccountId }, accountToReturn);
        }

        [HttpPost("collection")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateAccountCollection([FromBody] IEnumerable<AccountForCreationDTO> accountForCreationDTOs)
        {
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

        [HttpPut("{accountId}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> UpdateAccount(Guid accountId, [FromBody] AccountForUpdateDTO account)
        {
            var accountEntity = HttpContext.Items["account"];

            _mapper.Map(account, accountEntity);
            await _repository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{accountId}")]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> PartiallyUpdateAccount(Guid accountId,
            [FromBody] JsonPatchDocument<AccountForUpdateDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                _logger.LogError($"Account patchDocument object sent from client is null.");
                return BadRequest("PatchDocument is null");
            }

            var account = HttpContext.Items["account"];

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

        [HttpDelete("{accountId}")]
        [ServiceFilter(typeof(ValidateAccountExistsAttribute))]
        public async Task<IActionResult> DeleteAccount(Guid accountId)
        {
            var account = HttpContext.Items["account"] as Account;

            _repository.Account.DeleteAccount(account);
            await _repository.SaveAsync();

            return NoContent();
        }
    }
}