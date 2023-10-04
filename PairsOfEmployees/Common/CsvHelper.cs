using PairsOfEmployees.Models;

namespace PairsOfEmployees.Common
{
    public class CsvHelper : ICsvHelper
    {
        private readonly ILogger<CsvHelper> _logger;

        public CsvHelper(ILogger<CsvHelper> logger)
        {
            _logger = logger;
        }

        public async Task<ICollection<CsvModel>> ParseCsvAsync(IFormFile csvFile)
        {
            var employees = new HashSet<CsvModel>(new CsvRecordEqualityComparer());

            using (StreamReader reader = new StreamReader(csvFile.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line?.Split(',', StringSplitOptions.RemoveEmptyEntries);

                    if (values != null) employees.Add(EmployeesFactory(values));
                }
            }
            _logger.LogInformation("Successfully parsed CSV file");
            return employees;
        }

        public CsvModel EmployeesFactory(string[] values)
        {
            CsvModel csvWorkerModel = new CsvModel();

            try
            {
                csvWorkerModel = new CsvModel
                {
                    EmpID = int.Parse(values[0]),
                    ProjectID = int.Parse(values[1]),
                    DateFrom = DateTime.Parse(values[2]),
                    DateTo = DateTime.Parse(values[3].ToLower() == "null" || string.IsNullOrEmpty(values[3])
                        ? DateTime.Today.ToString()
                        : values[3]),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Csv parsing model fail - {exception}", ex);
            }

            return csvWorkerModel;
        }
    }
}
