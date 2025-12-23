using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PolyclinicApplication.Common.Results;

namespace PolyclinicAPI.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(kvp => kvp.Value!.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage))
                    .ToList();

                var message = errors.Count == 1
                    ? errors[0]
                    : string.Join("; ", errors);

                var apiResult = ApiResult<object>.BadRequest(message);
                context.Result = new BadRequestObjectResult(apiResult);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}