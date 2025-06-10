using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.Backend.Services.PairService
{
	public class PairService : IPairService
	{
		public List<PairResult> GetEmployeePairs(IEnumerable<EmployeeData> data)
		{
			var employeesByProject = data.GroupBy(r => r.ProjectID);
			var result = new List<PairResult>();

			foreach (var project in employeesByProject)
			{
				var employees = project.ToArray();

				if (employees.Length < 2) continue;

				int maxOverlapDays = 0;
				int firstEmpWithMax = 0;
				int secondEmpWithMax = 0;

				for (int i = 0; i < employees.Length - 1; i++)
				{
					var a = employees[i];
					for (int j = i + 1; j < employees.Length; j++)
					{
						var b = employees[j];
						int overlap = OverlapDays(a.DateFrom, a.DateTo, b.DateFrom, b.DateTo);
						if (overlap <= maxOverlapDays)
							continue;

						maxOverlapDays = overlap;
						firstEmpWithMax = a.EmpID;
						secondEmpWithMax = b.EmpID;
					}
				}

				if (maxOverlapDays > 0)
				{
					result.Add(new PairResult
					{
						FirstEmployee = firstEmpWithMax,
						SecondEmployee = secondEmpWithMax,
						DaysWorked = maxOverlapDays,
						ProjectID = project.Key
					});
				}
			}

			return result;
		}

		private static int OverlapDays(
			DateTime empStart1, DateTime? empEnd1,
			DateTime empStart2, DateTime? empEnd2)
		{
			var end1 = empEnd1 ?? DateTime.Today;
			var end2 = empEnd2 ?? DateTime.Today;

			var start = empStart1 > empStart2 ? empStart1 : empStart2;
			var end = end1 < end2 ? end1 : end2;

			return end >= start ? (end - start).Days + 1 : 0;
		}
	}
}
