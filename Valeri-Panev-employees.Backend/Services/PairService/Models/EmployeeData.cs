namespace Valeri_Panev_employees.Backend.Services.PairService.Models
{
	public class EmployeeData
	{
		public int EmpID { get; set; }
		public int ProjectID { get; set; }
		public DateTime DateFrom { get; set; }
		public DateTime? DateTo { get; set; } = DateTime.Now;
	}
}
