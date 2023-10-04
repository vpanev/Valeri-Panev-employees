using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PairsOfEmployees.Common;
using PairsOfEmployees.Configuration;
using PairsOfEmployees.Models;
using PairsOfEmployees.Services;

namespace PairsOfEmployees.Controllers
{
    [ApiController]
    [Route("files")]
    public class FileUploadController : ControllerBase
    {
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly ICsvHelper _csvHelper;
        private readonly IEmployeeExtractorService _employeeExtractorService;

        public FileUploadController(IOptions<ServiceConfiguration> serviceConfiguration, ICsvHelper csvHelper, IEmployeeExtractorService employeeExtractorService)
        {
            _serviceConfiguration = serviceConfiguration.Value;
            _csvHelper = csvHelper;
            _employeeExtractorService = employeeExtractorService;
        }

        [HttpPost]
        [Route("/upload-file")]
        public async Task<FileEmployeeUploadResponse> FileUpload([FromForm]IFormFile file)
        {
            var allowedFileTypes = _serviceConfiguration.AllowedFileTypes;

            if (!allowedFileTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                throw new BadHttpRequestException("Unsupported file extension.");
            }

            if (file.Length > _serviceConfiguration.FileSizeLimit * 1024 * 1024)
            {
                throw new BadHttpRequestException("Max file size is 20MB.");
            }

            var parsedCsvFile = await _csvHelper.ParseCsvAsync(file);
            var employees = _employeeExtractorService.ExtractEmployees(parsedCsvFile);

            return new FileEmployeeUploadResponse(employees);
        }
        public record FileEmployeeUploadResponse(CsvDto Employees);
    }
}
