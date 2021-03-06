﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.ActionFilters
{
    public class ValidateEventExistsAttribute : IAsyncActionFilter
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        public ValidateEventExistsAttribute(IRepositoryManager repository, ILoggerManager logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string method = context.HttpContext.Request.Method;
            var trackChanges = (method.Equals("PUT") || method.Equals("PATCH"));

            var accountId = (Guid)context.ActionArguments["accountId"];
            var account = await _repository.Account.GetAccountAsync(accountId, trackChanges);

            if(account == null)
            {
                _logger.LogInfo($"Account with id: {accountId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
                return;
            }

            var eventId = (Guid)context.ActionArguments["eventId"];
            var eventEntity = await _repository.Event.GetEventAsync(accountId, eventId, trackChanges);

            if(eventEntity == null)
            {
                _logger.LogInfo($"Event with id: {eventId} doesn't exist in the database.");
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add("event", eventEntity);
                await next();
            }
        }
    }
}
