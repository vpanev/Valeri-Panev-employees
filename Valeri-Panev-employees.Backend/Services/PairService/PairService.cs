using Valeri_Panev_employees.Backend.Services.PairService.Models;

namespace Valeri_Panev_employees.Backend.Services.PairService
{
	public class PairService : IPairService
	{
		/// <summary>
		/// Analyzes employee project data to identify pairs of employees who worked together on the same project
		/// </summary>
		/// <param name="data">
		/// Collection of employee project data
		/// </param>
		/// <returns>
		/// A list of <see cref="PairResult"/> objects representing employee pairs who worked
		/// together the longest on each project.
		/// </returns>
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

		/// <summary>
		/// Calculates the number of days two employees worked together on a project.
		/// </summary>
		/// <param name="empStart1">The start date of the first employee's assignment.</param>
		/// <param name="empEnd1">The end date of the first employee's assignment, or null if ongoing.</param>
		/// <param name="empStart2">The start date of the second employee's assignment.</param>
		/// <param name="empEnd2">The end date of the second employee's assignment, or null if ongoing.</param>
		/// <returns>
		/// The number of calendar days both employees worked on the project simultaneously.
		/// Returns 0 if there is no overlap in their work periods.
		/// </returns>
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
