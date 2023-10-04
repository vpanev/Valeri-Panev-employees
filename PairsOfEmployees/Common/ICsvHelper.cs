using PairsOfEmployees.Models;

namespace PairsOfEmployees.Common
{
    public interface ICsvHelper
    {
        Task<ICollection<CsvModel>> ParseCsvAsync(IFormFile csvFile);

        CsvModel EmployeesFactory(string[] values);
    }
}
