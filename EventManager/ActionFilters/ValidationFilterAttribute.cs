﻿using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.ActionFilters
{
    public class ValidationFilterAttribute : IActionFilter
    {
        private readonly ILoggerManager _logger;

        public ValidationFilterAttribute(ILoggerManager logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            object action = context.RouteData.Values["action"];
            object controller = context.RouteData.Values["controller"];

            object param = context.ActionArguments.SingleOrDefault(d => d.Value.ToString().Contains("DTO")).Value;

            if (param == null)
            {
                _logger.LogError($"Object sent from the client is null. Controller: {controller}," +
                    $" action:{action}.");
                context.Result = new BadRequestObjectResult($"Object is null. Controller: {controller}," +
                    $" action: {action}.");
                return;
            }

            if (!context.ModelState.IsValid)
            {
                _logger.LogError($"Invalid model state for object. Controller: {controller}," +
                    $" action: {action}.");
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

    }
}
