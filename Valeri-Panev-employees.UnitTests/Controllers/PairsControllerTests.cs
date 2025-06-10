using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Valeri_Panev_employees.Backend.Controllers;
using Valeri_Panev_employees.Backend.Controllers.PairsController;
using Valeri_Panev_employees.Backend.Controllers.PairsController.Dtos;
using Valeri_Panev_employees.Backend.Services.CsvParser;
using Valeri_Panev_employees.Backend.Services.PairService;
using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.UnitTests.Controllers
{
	public class PairsControllerTests : BaseTest
	{
		private readonly Mock<ICsvParser> _mockParser;
		private readonly Mock<IPairService> _mockPairService;
		private readonly Mock<ILogger<PairsController>> _mockLogger;
		private readonly PairsController _controller;

		public PairsControllerTests()
		{
			_mockParser = new Mock<ICsvParser>();
			_mockPairService = new Mock<IPairService>();
			_mockLogger = new Mock<ILogger<PairsController>>();
			_controller = new PairsController(_mockParser.Object, _mockPairService.Object, _mockLogger.Object);
		}

		[Fact]
		public async Task Upload_ValidFile_ReturnsOkResult()
		{
			// Arrange
			var file = CreateMockFile("valid-content", "test.csv");

			var employeeData = new List<EmployeeData>
			{
				new EmployeeData
				{
					EmpID = 1,
					ProjectID = 100,
					DateFrom = new DateTime(2022, 1, 1),
					DateTo = new DateTime(2022, 12, 31)
				},
				new EmployeeData
				{
					EmpID = 2,
					ProjectID = 100,
					DateFrom = new DateTime(2022, 6, 1),
					DateTo = new DateTime(2023, 1, 15)
				}
			};

			var pairResults = new List<PairResult>
			{
				new PairResult
				{
					FirstEmployee = 1,
					SecondEmployee = 2,
					ProjectID = 100,
					DaysWorked = 214
				}
			};

			_mockParser.Setup(p => p.ParseAsync(It.IsAny<IFormFile>()))
				.ReturnsAsync(employeeData);

			_mockPairService.Setup(s => s.GetEmployeePairs(It.IsAny<IEnumerable<EmployeeData>>()))
				.Returns(pairResults);

			// Act
			var result = await _controller.Upload(file);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var wrapper = Assert.IsType<ApiWrapper<List<PairResponse>>>(okResult.Value);
			Assert.True(wrapper.Success);
			Assert.Empty(wrapper.Errors);
			Assert.NotNull(wrapper.Data);
			Assert.Single(wrapper.Data);
			Assert.Equal(1, wrapper.Data[0].EmployeeID1);
			Assert.Equal(2, wrapper.Data[0].EmployeeID2);
			Assert.Equal(100, wrapper.Data[0].ProjectID);
			Assert.Equal(214, wrapper.Data[0].TotalWorkDays);
		}

		[Fact]
		public async Task Upload_ValidFileWithMultipleProjects_ReturnsCorrectPairs()
		{
			// Arrange
			var file = CreateMockFile("valid-content-multiple-projects", "test.csv");

			var employeeData = new List<EmployeeData>
			{
				new EmployeeData { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) },
				new EmployeeData { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 6, 1), DateTo = new DateTime(2023, 1, 15) },
				new EmployeeData { EmpID = 3, ProjectID = 200, DateFrom = new DateTime(2022, 3, 1), DateTo = new DateTime(2022, 9, 30) },
				new EmployeeData { EmpID = 4, ProjectID = 200, DateFrom = new DateTime(2022, 5, 1), DateTo = new DateTime(2022, 8, 31) }
			};

			var pairResults = new List<PairResult>
			{
				new PairResult { FirstEmployee = 1, SecondEmployee = 2, ProjectID = 100, DaysWorked = 214 },
				new PairResult { FirstEmployee = 3, SecondEmployee = 4, ProjectID = 200, DaysWorked = 123 }
			};

			_mockParser.Setup(p => p.ParseAsync(It.IsAny<IFormFile>()))
				.ReturnsAsync(employeeData);

			_mockPairService.Setup(s => s.GetEmployeePairs(It.IsAny<IEnumerable<EmployeeData>>()))
				.Returns(pairResults);

			// Act
			var result = await _controller.Upload(file);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var wrapper = Assert.IsType<ApiWrapper<List<PairResponse>>>(okResult.Value);
			Assert.True(wrapper.Success);
			Assert.NotNull(wrapper.Data);
			Assert.Equal(2, wrapper.Data.Count);
		}

		[Fact]
		public async Task Upload_EmptyFile_ReturnsBadRequest()
		{
			// Arrange
			var emptyFile = CreateMockFile("", "empty.csv", 0);

			// Act
			var result = await _controller.Upload(emptyFile);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			var wrapper = Assert.IsType<ApiWrapper<List<PairResponse>>>(badRequestResult.Value);
			Assert.Null(wrapper.Data);
			Assert.False(wrapper.Success);
			Assert.Contains("Empty file.", wrapper.Errors);
		}

		[Fact]
		public async Task Upload_NoValidRows_ReturnsBadRequest()
		{
			// Arrange
			var file = CreateMockFile("invalid-content", "test.csv");

			_mockParser.Setup(p => p.ParseAsync(It.IsAny<IFormFile>()))
				.ReturnsAsync(new List<EmployeeData>());

			// Act
			var result = await _controller.Upload(file);

			// Assert
			var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
			var wrapper = Assert.IsType<ApiWrapper<List<PairResponse>>>(badRequestResult.Value);
			Assert.Null(wrapper.Data);
			Assert.False(wrapper.Success);
			Assert.Contains("No valid rows.", wrapper.Errors);
		}
	}
}