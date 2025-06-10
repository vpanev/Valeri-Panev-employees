using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.Backend.Services.PairService
{
	public interface IPairService
	{
		List<PairResult> GetEmployeePairs(IEnumerable<EmployeeData> source);
	}
}
