using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.Backend.Services.CsvParser
{
	public interface ICsvParser

	{
		Task<List<EmployeeData>> ParseAsync(IFormFile file);
	}
}
