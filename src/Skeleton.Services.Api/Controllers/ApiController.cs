using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Skeleton.Services.Api.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private readonly ICollection<string> _errors = new List<string>();

        protected IActionResult CustomResponse(object? result = null)
        {
            if (IsOperationValid())
            {
                if (result is null)
                    return NoContent();

                return Ok(result);
            }

            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                {"Messages", _errors.ToArray()}
            })
            {
                Type = null,
                Title = "Ocorreu um ou mais erros de validação.",
                Status = StatusCodes.Status400BadRequest,
                Detail = null,
                Instance = $"{HttpContext.Request.Method.ToLower()} {HttpContext.Request.Path}"
            });
        }

        protected IActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected IActionResult CustomResponse(ValidationResult? validationResult)
        {
            foreach (var error in validationResult!.Errors)
            {
                AddError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        private bool IsOperationValid()
        {
            return !_errors.Any();
        }

        protected void AddError(string erro)
        {
            _errors.Add(erro);
        }

        protected void ClearErrors()
        {
            _errors.Clear();
        }
    }
}