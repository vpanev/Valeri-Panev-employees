using PairsOfEmployees.Models;

namespace PairsOfEmployees.Common
{
    public class CsvRecordEqualityComparer : IEqualityComparer<CsvModel>
    {
        public bool Equals(CsvModel x, CsvModel y)
        {
            return x.EmpID == y.EmpID
                   && x.ProjectID == y.ProjectID
                   && x.DateTo == y.DateTo
                   && x.DateFrom == y.DateFrom;
        }

        public int GetHashCode(CsvModel obj)
        {
            return obj.EmpID.GetHashCode()
                   ^ obj.ProjectID.GetHashCode()
                   ^ obj.DateTo.GetHashCode()
                   ^ obj.DateFrom.GetHashCode();
        }
    }
}