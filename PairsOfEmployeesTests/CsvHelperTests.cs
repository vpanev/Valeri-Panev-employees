using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PairsOfEmployees.Common;

namespace PairsOfEmployeesTests
{
    public class CsvHelperTests
    {
        private readonly Mock<ILogger<CsvHelper>> _loggerMock;
        private readonly ICsvHelper _csvHelper;
        private readonly string _testFilesPath;

        public CsvHelperTests()
        {
            _loggerMock = new Mock<ILogger<CsvHelper>>();
            _csvHelper = new CsvHelper(_loggerMock.Object);
            var assemblyPath = Directory.GetParent(Directory.GetCurrentDirectory())!.Parent!.Parent!.FullName;
            _testFilesPath = Path.Combine(assemblyPath!, "TestFiles");
        }

        [Fact]
        public async Task ParseCsvAsync_ValidCsvFile_ReturnsParsedModels()
        {
            // Arrange
            var file = Path.Combine(_testFilesPath, "PairsWithMoreThanOneProj.csv");

            await using var stream = new FileStream(file, FileMode.Open);
            var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(file))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            // Act
            var result = await _csvHelper.ParseCsvAsync(formFile);

            // Assert

            // Note that there is one duplicate in the file. Actual records are 12 but as they are put in HashSet, duplicates are removed.
            Assert.NotNull(result);
            Assert.Equal(11, result.Count);
        }
    }
}
