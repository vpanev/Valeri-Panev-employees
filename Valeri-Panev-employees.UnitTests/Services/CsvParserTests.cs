using Microsoft.Extensions.Logging;
using Moq;
using System.Globalization;
using Valeri_Panev_employees.Backend.Services.CsvParser;

namespace Valeri_Panev_employees.UnitTests.Services
{
	public class CsvParserTests : BaseTest
	{
		private readonly ICsvParser _csvParser;
		private readonly Mock<ILogger<CsvParser>> _mockLogger;

		public CsvParserTests()
		{
			_mockLogger = new Mock<ILogger<CsvParser>>();
			_csvParser = new CsvParser(_mockLogger.Object);
		}

		[Fact]
		public async Task ParseAsync_ValidCsvFile_ReturnsExpectedEmployeeData()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "1,100,01/01/2022,31/12/2022\n" +
							 "2,100,01/06/2022,15/01/2023";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Equal(2, result.Count);

			Assert.Equal(1, result[0].EmpID);
			Assert.Equal(100, result[0].ProjectID);
			Assert.Equal(new DateTime(2022, 1, 1), result[0].DateFrom);
			Assert.Equal(new DateTime(2022, 12, 31), result[0].DateTo);

			Assert.Equal(2, result[1].EmpID);
			Assert.Equal(100, result[1].ProjectID);
			Assert.Equal(new DateTime(2022, 6, 1), result[1].DateFrom);
			Assert.Equal(new DateTime(2023, 1, 15), result[1].DateTo);
		}

		[Fact]
		public async Task ParseAsync_DateToIsNull_UsesTodayDate()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "1,100,01/01/2022,NULL";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Single(result);
			Assert.Equal(1, result[0].EmpID);
			Assert.Equal(100, result[0].ProjectID);
			Assert.Equal(new DateTime(2022, 1, 1), result[0].DateFrom);
			Assert.Equal(DateTime.Today, result[0].DateTo);
		}

		[Fact]
		public async Task ParseAsync_NULLDateTo_UsesTodayDate()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "1,100,01/01/2022,NULL";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Single(result);
			Assert.Equal(DateTime.Today, result[0].DateTo);
		}

		[Fact]
		public async Task ParseAsync_InvalidEmpIdOrProjectId_SkipsRow()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "invalid,100,01/01/2022,31/12/2022\n" +
							 "1,invalid,01/06/2022,15/01/2023\n" +
							 "3,300,01/07/2022,31/12/2022";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Single(result);
			Assert.Equal(3, result[0].EmpID);
			Assert.Equal(300, result[0].ProjectID);
		}

		[Fact]
		public async Task ParseAsync_MissingColumns_SkipsRow()
		{
			// Arrange
			var csvContent = "EmpID,DateFrom,DateTo\n" +
							 "1,01/01/2022,31/12/2022";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[InlineData("dd/MM/yyyy", "01/01/2022", "31/12/2022")]
		[InlineData("dd-MMM-yyyy", "01-Jan-2022", "31-Dec-2022")]
		[InlineData("dd-MMM-yy", "01-Jan-22", "31-Dec-22")]
		[InlineData("yyyy/MM/dd", "2022/01/01", "2022/12/31")]
		[InlineData("yyyy-MM-dd", "2022-01-01", "2022-12-31")]
		[InlineData("MM/dd/yyyy", "01/01/2022", "12/31/2022")]
		[InlineData("dd-MM-yyyy", "01-01-2022", "31-12-2022")]
		[InlineData("dd.MM.yyyy", "01.01.2022", "31.12.2022")]
		[InlineData("yyyy-MM-dd'T'HH:mm:ss", "2022-01-01T00:00:00", "2022-12-31T23:59:59")]
		public async Task ParseAsync_DifferentDateFormats_ParsesCorrectly(string format, string fromDate, string toDate)
		{
			// Arrange
			var csvContent = $"EmpID,ProjectID,DateFrom,DateTo\n" +
							 $"1,100,{fromDate},{toDate}";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act
			var result = await _csvParser.ParseAsync(file);

			// Assert
			Assert.Single(result);

			var expectedFromDate = DateTime.ParseExact(fromDate, format, CultureInfo.InvariantCulture);
			var expectedToDate = DateTime.ParseExact(toDate, format, CultureInfo.InvariantCulture);

			Assert.Equal(expectedFromDate, result[0].DateFrom);
			Assert.Equal(expectedToDate, result[0].DateTo);
		}

		[Fact]
		public async Task ParseAsync_UnrecognizedDateFormat_ThrowsFormatException()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "1,100,202/01/01,31-December";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act & Assert
			await Assert.ThrowsAsync<FormatException>(() => _csvParser.ParseAsync(file));
		}

		[Fact]
		public async Task ParseAsync_NullDateFrom_ThrowsFormatException()
		{
			// Arrange
			var csvContent = "EmpID,ProjectID,DateFrom,DateTo\n" +
							 "1,100,NULL,31/12/2022";

			var file = CreateMockFile(csvContent, "employees.csv");

			// Act & Assert
			// This should throw because DateFrom is required and cannot be NULL
			await Assert.ThrowsAsync<FormatException>(() => _csvParser.ParseAsync(file));
		}
	}
}