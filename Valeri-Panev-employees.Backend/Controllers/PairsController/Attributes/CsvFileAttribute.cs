using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Valeri_Panev_employees.Backend.Controllers.PairsController.Attributes
{
	/// <summary>
	/// An action filter attribute that validates if the provided file parameter is a valid CSV file.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CsvFileAttribute : ActionFilterAttribute
	{
		private readonly string _parameterName;

		public CsvFileAttribute(string parameterName = "file")
		{
			_parameterName = parameterName;
		}

		/// <summary>
		/// Called before the action method is invoked to validate the file parameter.
		/// </summary>
		/// <param name="context">The context for the action execution.</param>
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			if (!context.ActionArguments.TryGetValue(_parameterName, out var value) || value is not IFormFile file)
			{
				context.Result =
					new BadRequestObjectResult(ApiWrapper<object>.CreateError($"Parameter '{_parameterName}' is required and must be a file."));

				return;
			}

			var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
			if (fileExtension != ".csv")
			{
				context.Result = new BadRequestObjectResult(ApiWrapper<object>.CreateError("Only CSV files are supported."));
				return;
			}

			base.OnActionExecuting(context);
		}
	}

}
