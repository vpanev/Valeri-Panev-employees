using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PairsOfEmployees.Common;
using PairsOfEmployees.Models;
using PairsOfEmployees.Services;

namespace PairsOfEmployeesTests
{
    public class EmployeeExtractorServiceTests
    {
        [Fact]
        public void ExtractEmployees_ReturnsEmptyCsvDto_WhenParsedCsvIsEmpty()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<EmployeeExtractorService>>();
            var csvHelperMock = new Mock<ICsvHelper>();
            var service = new EmployeeExtractorService(loggerMock.Object, csvHelperMock.Object);

            var csvModels = new List<CsvModel>
            {
                new() { EmpID = 1, ProjectID = 101, DateFrom = new DateTime(2023, 1, 1), DateTo = new DateTime(2023, 1, 5) },
             };
            
            // Act
            var result = service.ExtractEmployees(csvModels);

            // Assert
            Assert.Null(result.CsvWorkedTogetherCollection);
        }

        [Fact]
        public void ExtractEmployees_ReturnsCsvDtoWithPairs()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<EmployeeExtractorService>>();
            var csvHelperMock = new Mock<ICsvHelper>();
            var service = new EmployeeExtractorService(loggerMock.Object, csvHelperMock.Object);

            var csvModels = new List<CsvModel>
            {
                new() { EmpID = 1, ProjectID = 101, DateFrom = new DateTime(2023, 1, 1), DateTo = new DateTime(2023, 1, 5) },
                new() { EmpID = 2, ProjectID = 101, DateFrom = new DateTime(2023, 1, 3), DateTo = new DateTime(2023, 1, 7) },
                new() { EmpID = 3, ProjectID = 102, DateFrom = new DateTime(2023, 1, 5), DateTo = new DateTime(2023, 1, 10) },
            };

            // Act
            var result = service.ExtractEmployees(csvModels);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.CsvWorkedTogetherCollection);
            Assert.Single(result.CsvWorkedTogetherCollection);
            var pair = result.CsvWorkedTogetherCollection.First();
            Assert.Equal(1, pair.EmployeeID1);
            Assert.Equal(2, pair.EmployeeID2);
            Assert.Equal(101, pair.ProjectID);
            Assert.Equal(3, pair.WorkedTimeTogether);
            Assert.Equal(1, result.CsvWorkedTogetherCollection.Count);
        }
    }
}
