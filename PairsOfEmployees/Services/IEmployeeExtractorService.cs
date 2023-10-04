using PairsOfEmployees.Models;

namespace PairsOfEmployees.Services
{
    public interface IEmployeeExtractorService
    {
        public CsvDto ExtractEmployees(ICollection<CsvModel> parsedCsv);
    }
}
