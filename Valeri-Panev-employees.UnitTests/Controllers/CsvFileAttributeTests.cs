using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Valeri_Panev_employees.Backend.Controllers;
using Valeri_Panev_employees.Backend.Controllers.PairsController.Attributes;

namespace Valeri_Panev_employees.UnitTests.Controllers
{
	public class CsvFileAttributeTests : BaseTest
	{
		[Fact]
		public void OnActionExecuting_CsvFile_ContinuesExecution()
		{
			// Arrange
			var file = CreateMockFile("content", "test.csv");
			var actionContext = new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor());

			var actionArguments = new Dictionary<string, object> { { "file", file } };
			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				actionArguments!,
				new object());

			var filter = new CsvFileAttribute();

			// Act
			filter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.Null(actionExecutingContext.Result);
		}

		[Fact]
		public void OnActionExecuting_NonCsvFile_ReturnsBadRequest()
		{
			// Arrange
			var file = CreateMockFile("content", "test.txt");
			var actionContext = new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor());

			var actionArguments = new Dictionary<string, object> { { "file", file } };
			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				actionArguments!,
				new object());

			var filter = new CsvFileAttribute();

			// Act
			filter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.NotNull(actionExecutingContext.Result);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
			var wrapper = Assert.IsType<ApiWrapper<object>>(badRequestResult.Value);
			Assert.Contains("Only CSV files are supported.", wrapper.Errors);
		}

		[Fact]
		public void OnActionExecuting_MissingFile_ReturnsBadRequest()
		{
			// Arrange
			var actionContext = new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor());

			var actionArguments = new Dictionary<string, object>();
			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				actionArguments!,
				new object());

			var filter = new CsvFileAttribute();

			// Act
			filter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.NotNull(actionExecutingContext.Result);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
			var wrapper = Assert.IsType<ApiWrapper<object>>(badRequestResult.Value);
			Assert.Contains("Parameter 'file' is required", wrapper.Errors[0]);
		}

		[Fact]
		public void OnActionExecuting_CustomParameterName_ValidatesCorrectParameter()
		{
			// Arrange
			var file = CreateMockFile("content", "test.txt");
			var actionContext = new ActionContext(
				new DefaultHttpContext(),
				new RouteData(),
				new ActionDescriptor());

			var actionArguments = new Dictionary<string, object> { { "customParam", file } };
			var actionExecutingContext = new ActionExecutingContext(
				actionContext,
				new List<IFilterMetadata>(),
				actionArguments!,
				new object());

			var filter = new CsvFileAttribute("customParam");

			// Act
			filter.OnActionExecuting(actionExecutingContext);

			// Assert
			Assert.NotNull(actionExecutingContext.Result);
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionExecutingContext.Result);
			var wrapper = Assert.IsType<ApiWrapper<object>>(badRequestResult.Value);
			Assert.Contains("Only CSV files are supported.", wrapper.Errors);
		}
	}
}