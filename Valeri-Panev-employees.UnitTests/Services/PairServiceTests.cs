using Valeri_Panev_employees.Backend.Services.PairService;
using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.UnitTests.Services
{
	public class PairServiceTests
	{
		private readonly IPairService _pairService;

		public PairServiceTests()
		{
			_pairService = new PairService();
		}

		[Fact]
		public void GetEmployeePairs_SingleProject_FindsCorrectPair()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 3, 1), DateTo = new DateTime(2022, 10, 31) },
				new() { EmpID = 3, ProjectID = 100, DateFrom = new DateTime(2022, 5, 1), DateTo = new DateTime(2022, 9, 30) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].FirstEmployee);
			Assert.Equal(2, result[0].SecondEmployee);
			Assert.Equal(100, result[0].ProjectID);
			Assert.Equal(245, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_MultipleProjects_FindsBestPairForEachProject()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
                // Project 100
                new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 3, 1), DateTo = new DateTime(2022, 10, 31) },
                
                // Project 200
                new() { EmpID = 3, ProjectID = 200, DateFrom = new DateTime(2022, 4, 1), DateTo = new DateTime(2022, 8, 31) },
				new() { EmpID = 4, ProjectID = 200, DateFrom = new DateTime(2022, 5, 1), DateTo = new DateTime(2022, 7, 31) },
				new() { EmpID = 5, ProjectID = 200, DateFrom = new DateTime(2022, 6, 1), DateTo = new DateTime(2022, 6, 30) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Equal(2, result.Count);

			var pair100 = result.First(p => p.ProjectID == 100);
			Assert.Equal(1, pair100.FirstEmployee);
			Assert.Equal(2, pair100.SecondEmployee);
			Assert.Equal(245, pair100.DaysWorked);

			var pair200 = result.First(p => p.ProjectID == 200);
			Assert.Equal(3, pair200.FirstEmployee);
			Assert.Equal(4, pair200.SecondEmployee);
			Assert.Equal(92, pair200.DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_EmployeesWithNullEndDate_UsesTodayAsEndDate()
		{
			// Arrange
			var today = DateTime.Today;
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = null },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2023, 1, 1), DateTo = null }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);

			var expectedDays = (today - new DateTime(2023, 1, 1)).Days + 1;
			Assert.Equal(expectedDays, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_CompletelyOverlappingDates_CalculatesCorrectDays()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 3, 1), DateTo = new DateTime(2022, 8, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].FirstEmployee);
			Assert.Equal(2, result[0].SecondEmployee);
			Assert.Equal(184, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_PartiallyOverlappingDates_CalculatesCorrectDays()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 6, 30) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 6, 1), DateTo = new DateTime(2022, 12, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].FirstEmployee);
			Assert.Equal(2, result[0].SecondEmployee);
			Assert.Equal(30, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_SameDayOverlap_CountsAsOneDay()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 1) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 1) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_LeapYearDates_CalculatesCorrectDays()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2020, 1, 1), DateTo = new DateTime(2020, 12, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2020, 2, 1), DateTo = new DateTime(2020, 3, 1) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(30, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_EmptyInput_ReturnsEmptyList()
		{
			// Arrange
			var employees = new List<EmployeeData>();

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEmployeePairs_SingleEmployeePerProject_ReturnsEmptyList()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) },
				new() { EmpID = 2, ProjectID = 200, DateFrom = new DateTime(2022, 3, 1), DateTo = new DateTime(2022, 10, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEmployeePairs_NoOverlappingWorkPeriods_ReturnsEmptyList()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 3, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 4, 1), DateTo = new DateTime(2022, 6, 30) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEmployeePairs_StartDateAfterEndDate_ReturnsZeroOverlapDays()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 6, 1), DateTo = new DateTime(2022, 3, 1) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 12, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Empty(result);
		}

		[Fact]
		public void GetEmployeePairs_MultiplePairsWithSameOverlap_SelectsFirstPairFound()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 3, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 3, 31) },
				new() { EmpID = 3, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 3, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].FirstEmployee);
			Assert.Equal(2, result[0].SecondEmployee);
			Assert.Equal(90, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_EmployeesWithFutureDates_CalculatesCorrectOverlap()
		{
			// Arrange
			var futureStart = DateTime.Today.AddDays(10);
			var futureEnd = DateTime.Today.AddDays(100);

			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = futureStart, DateTo = futureEnd },
				new() { EmpID = 2, ProjectID = 100, DateFrom = futureStart.AddDays(20), DateTo = futureEnd.AddDays(20) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(71, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_OverlappingExactlyOneDayAtBoundary_CountsAsOneDay()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 6, 30) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 6, 30), DateTo = new DateTime(2022, 12, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_ExtremelyLongTimeSpan_CalculatesCorrectDays()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2000, 1, 1), DateTo = new DateTime(2020, 12, 31) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2010, 1, 1), DateTo = new DateTime(2015, 12, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);

			// Jan 1, 2010 to Dec 31, 2015 = 2,191 days (including leap years)
			Assert.Equal(2191, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_MixOfNullAndNonNullEndDates_CalculatesCorrectDays()
		{
			// Arrange
			var today = DateTime.Today;
			var employees = new List<EmployeeData>
			{
				new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = null },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 6, 1), DateTo = new DateTime(2022, 12, 31) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);

			// If today is after Dec 31, 2022, then overlap is Jun 1 to Dec 31 (214 days)
			// If today is before Dec 31, 2022, then overlap is Jun 1 to today
			var expectedEndDate = today < new DateTime(2022, 12, 31) ? today : new DateTime(2022, 12, 31);
			var expectedDays = (expectedEndDate - new DateTime(2022, 6, 1)).Days + 1;

			Assert.Equal(expectedDays, result[0].DaysWorked);
		}

		[Fact]
		public void GetEmployeePairs_MultipleTiesInOverlapDays_SelectsFirstPairEncountered()
		{
			// Arrange
			var employees = new List<EmployeeData>
			{
                new() { EmpID = 1, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 30) },
				new() { EmpID = 2, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 30) },
				new() { EmpID = 3, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 30) },
				new() { EmpID = 4, ProjectID = 100, DateFrom = new DateTime(2022, 1, 1), DateTo = new DateTime(2022, 1, 30) }
			};

			// Act
			var result = _pairService.GetEmployeePairs(employees);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].FirstEmployee);
			Assert.Equal(2, result[0].SecondEmployee);
			Assert.Equal(30, result[0].DaysWorked);
		}
	}
}