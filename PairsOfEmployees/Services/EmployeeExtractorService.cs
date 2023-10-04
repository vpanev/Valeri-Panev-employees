using PairsOfEmployees.Common;
using PairsOfEmployees.Models;

namespace PairsOfEmployees.Services
{
    public class EmployeeExtractorService : IEmployeeExtractorService
    {
        private readonly ICsvHelper _csvHelper;
        private readonly ILogger _logger;

        public EmployeeExtractorService(ILogger<EmployeeExtractorService> logger, ICsvHelper fileParser)
        {
            _csvHelper = fileParser;
            _logger = logger;
        }

        public CsvDto ExtractEmployees(ICollection<CsvModel> parsedCsv)
        {
            var pairOfEmployeesWorkedTogether = new List<CsvWorkedTogether>();

            var groupedEmployeesByProjectId = parsedCsv
                .GroupBy(e => e.ProjectID)
                .Where(e => e.Count() > 1)
                .ToList();

            foreach (var project in groupedEmployeesByProjectId)
            {
                var projectToList = project.ToList();

                for (int i = 0; i < projectToList.Count; i++)
                {
                    var firstEmployee = projectToList[i];

                    for (int j = i + 1; j < projectToList.Count; j++)
                    {
                        var secondEmployee = projectToList[j];

                        var startDate = firstEmployee.DateFrom > secondEmployee.DateFrom ? firstEmployee.DateFrom : secondEmployee.DateFrom;

                        var endDate = firstEmployee.DateTo < secondEmployee.DateTo ? firstEmployee.DateTo : secondEmployee.DateTo;

                        var duration = endDate!.Value.AddDays(1) - startDate;
                        var daysWorkedTogether = duration.Days;

                        var csvWorkedTogether = new CsvWorkedTogether
                        {
                            //Swap ids in order to not have same ids in different columns. This will help us with the grouping below.
                            EmployeeID1 = firstEmployee.EmpID < secondEmployee.EmpID ? firstEmployee.EmpID : secondEmployee.EmpID,
                            EmployeeID2 = firstEmployee.EmpID > secondEmployee.EmpID ? firstEmployee.EmpID : secondEmployee.EmpID,
                            ProjectID = firstEmployee.ProjectID,
                            WorkedTimeTogether = daysWorkedTogether
                        };

                        if (firstEmployee.EmpID != secondEmployee.EmpID && daysWorkedTogether > 0)
                        {
                            pairOfEmployeesWorkedTogether.Add(csvWorkedTogether);
                        }
                    }
                }
            }

            var csvDto = new CsvDto();
            if (pairOfEmployeesWorkedTogether.Any())
            {
                var pairs = pairOfEmployeesWorkedTogether
                    .GroupBy(x => new { x.EmployeeID1, x.EmployeeID2 })
                    .SelectMany(x => x)
                    .OrderBy(x=> x.WorkedTimeTogether)
                    .ToList();

                csvDto.CsvWorkedTogetherCollection = pairs;
            }

            return csvDto;
        }
    }
}
