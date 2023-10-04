using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using PairsOfEmployees.Common;
using PairsOfEmployees.Configuration;
using PairsOfEmployees.Controllers;
using PairsOfEmployees.Models;
using PairsOfEmployees.Services;
using System.Reflection;
using Newtonsoft.Json;

namespace PairsOfEmployeesTests
{
    public class FileUploadControllerTests
    {
        private readonly Mock<IOptions<ServiceConfiguration>> _serviceConfigurationMock;
        private readonly Mock<ICsvHelper> _csvHelperMock;
        private Mock<IEmployeeExtractorService> _employeeExtractorServiceMock;
        private readonly string _testFilesPath;

        public FileUploadControllerTests()
        {
            var serviceConfiguration = new ServiceConfiguration
            {
                AllowedFileTypes = new[] { ".csv" },
                FileSizeLimit = 20
            };

            _serviceConfigurationMock = new Mock<IOptions<ServiceConfiguration>>();
            _serviceConfigurationMock.Setup(x => x.Value).Returns(serviceConfiguration);

            _csvHelperMock = new Mock<ICsvHelper>();
            _employeeExtractorServiceMock = new Mock<IEmployeeExtractorService>();

            var assemblyPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            _testFilesPath = Path.Combine(assemblyPath!, "TestFiles");
        }

        [Fact]
        public async Task FileUpload_ValidFile_ReturnsFileEmployeeUploadResponse()
        {
            // Arrange
            var file = Path.Combine(_testFilesPath, "PairsWithMoreThanOneProj.csv");

            await using var stream = new FileStream(file, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(file))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var workedCouple1 = new CsvWorkedTogether()
            {
                EmployeeID1 = 1, EmployeeID2 = 2, ProjectID = 3, WorkedTimeTogether = 10
            };
            var workedCouple2 = new CsvWorkedTogether()
            {
                EmployeeID1 = 3, EmployeeID2 = 12, ProjectID = 3, WorkedTimeTogether = 100
            };
            var workedCouple3 = new CsvWorkedTogether()
            {
                EmployeeID1 = 10, EmployeeID2 = 12, ProjectID = 4, WorkedTimeTogether = 220
            };
            
            CsvDto csvDto = new CsvDto();
            csvDto.CsvWorkedTogetherCollection = new List<CsvWorkedTogether>
            {
                workedCouple1,
                workedCouple2,
                workedCouple3
            };
            
            _csvHelperMock.Setup(x => x.ParseCsvAsync(It.IsAny<FormFile>()))
                .ReturnsAsync(It.IsAny<ICollection<CsvModel>>());
            
            _employeeExtractorServiceMock
                .Setup(service => service.ExtractEmployees(It.IsAny<ICollection<CsvModel>>()))
                .Returns(csvDto);

            var controller = new FileUploadController(_serviceConfigurationMock.Object, _csvHelperMock.Object, _employeeExtractorServiceMock.Object);

            // Act
            var result = await controller.FileUpload(formFile);

            var object1 = JsonConvert.SerializeObject(result.Employees);
            var object2 = JsonConvert.SerializeObject(csvDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(object1, object2);
        }

        [Fact]
        public async Task FileUpload_InvalidFileExtension_ThrowsBadHttpRequestException()
        {
            // Arrange
            var file = Path.Combine(_testFilesPath, "InvalidFile.txt");

            await using var stream = new FileStream(file, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(file))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var controller = new FileUploadController(_serviceConfigurationMock.Object, _csvHelperMock.Object, _employeeExtractorServiceMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => controller.FileUpload(formFile));
        }

        [Fact]
        public async Task FileUpload_FileSizeExceedsLimit_ThrowsBadHttpRequestException()
        {
            // Arrange
            var file = Path.Combine(_testFilesPath, "PairsWithMoreThanOneProj.csv");

            await using var stream = new FileStream(file, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(file))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };


            var serviceConfiguration = new ServiceConfiguration
            {
                AllowedFileTypes = new[] { ".csv" },
                FileSizeLimit = 0
            };

            _serviceConfigurationMock.Setup(x => x.Value).Returns(serviceConfiguration);


            var controller = new FileUploadController(_serviceConfigurationMock.Object, _csvHelperMock.Object, _employeeExtractorServiceMock.Object);
            
            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => controller.FileUpload(formFile));
        }
    }
}