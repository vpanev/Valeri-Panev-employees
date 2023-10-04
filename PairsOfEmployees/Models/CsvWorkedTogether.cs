namespace PairsOfEmployees.Models
{
    public class CsvWorkedTogether
    {
        public int EmployeeID1 { get; init; }
        public int EmployeeID2 { get; init; }
        public int ProjectID { get; init; }
        public int WorkedTimeTogether { get; set; }
    }
}
