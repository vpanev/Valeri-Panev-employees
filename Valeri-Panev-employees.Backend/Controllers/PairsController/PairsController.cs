using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Valeri_Panev_employees.Backend.Controllers.PairsController.Attributes;
using Valeri_Panev_employees.Backend.Controllers.PairsController.Dtos;
using Valeri_Panev_employees.Backend.Services.CsvParser;
using Valeri_Panev_employees.Backend.Services.PairService;

namespace Valeri_Panev_employees.Backend.Controllers.PairsController
{
	[ApiController]
	[Route("api/[controller]")]
	public sealed class PairsController : ControllerBase
	{
		private readonly ICsvParser _parser;
		private readonly IPairService _pairService;
		private readonly ILogger<PairsController> _logger;

		public PairsController(ICsvParser parser, IPairService pairService, ILogger<PairsController> logger)
		{
			_parser = parser;
			_pairService = pairService;
			_logger = logger;
		}

		[HttpPost("upload")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(typeof(ApiWrapper<List<PairResponse>>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[CsvFile]
		public async Task<ActionResult<ApiWrapper<List<PairResponse>>>> Upload([Required] IFormFile file)
		{
			if (file.Length == 0)
			{
				_logger.LogWarning($"{nameof(PairsController)}: Empty file received with name: {file.Name}");
				return BadRequest(ApiWrapper<List<PairResponse>>.CreateError("Empty file."));
			}

			var employeeData = await _parser.ParseAsync(file);

			if (!employeeData.Any())
			{
				_logger.LogWarning($"{nameof(PairsController)}: No valid rows found.");
				return BadRequest(ApiWrapper<List<PairResponse>>.CreateError("No valid rows."));
			}

			var pairResults = _pairService.GetEmployeePairs(employeeData);

			var pairResponses = pairResults
				.Select(pr => new PairResponse
				{
					EmployeeID1 = pr.FirstEmployee,
					EmployeeID2 = pr.SecondEmployee,
					ProjectID = pr.ProjectID,
					TotalWorkDays = pr.DaysWorked
				})
				.ToList();

			return Ok(ApiWrapper<List<PairResponse>>.CreateSuccess(pairResponses));
		}
	}
}
