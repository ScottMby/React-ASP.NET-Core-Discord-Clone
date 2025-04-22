using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Discord_Clone.Server.Utilities
{
    /// <summary>
    /// Verifies that a IFormFile passed as a parameter is an image and within a maximum size.
    /// </summary>
    /// <param name="maxSize">Max file size in bytes.</param>
    [AttributeUsage(AttributeTargets.Method)]
    public class ImageValidationFilter(long maxSize) : Attribute, IActionFilter
    {
        private readonly long _maxSize = maxSize;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var param = context.ActionArguments.SingleOrDefault(p => p.Value is IFormFile);

            if (param.Value is not IFormFile file || file.Length == 0)
            {
                context.Result = new BadRequestObjectResult("Invalid file.");
                return;
            }

            file = (IFormFile)param.Value;

            if (!ImageValidator.IsFileExtensionAllowed(file))
            {
                context.Result = new BadRequestObjectResult("File extension not allowed.");
                return;
            }

            if (!ImageValidator.IsFileSizeWithinRange(file, _maxSize))
            {
                context.Result = new BadRequestObjectResult("File size exceeds the limit.");
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
