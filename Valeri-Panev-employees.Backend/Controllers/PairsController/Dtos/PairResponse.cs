namespace Valeri_Panev_employees.Backend.Controllers.PairsController.Dtos
{
	public sealed class PairResponse
	{
		public int EmployeeID1 { get; init; }
		public int EmployeeID2 { get; init; }
		public int ProjectID { get; init; }
		public int TotalWorkDays { get; set; }
	}
}
