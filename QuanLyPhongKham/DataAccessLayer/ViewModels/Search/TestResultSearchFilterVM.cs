using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels.Search
{
    public class TestResultSearchFilterVM
    {
        public string SearchTerm { get; set; } = "";

        [RegularExpression("^(TestName|PatientName|UserName|TestDate|ResultDetail)$",
            ErrorMessage = "SortBy must be TestName, PatientName, UserName, TestDate, or ResultDetail")]
        public string SortBy { get; set; } = "TestDate";

        public bool SortDescending { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        // Filter properties
        public int? TestId { get; set; }
        public int? UserId { get; set; }
        public int? RecordId { get; set; }
        public DateTime? TestDateFrom { get; set; }
        public DateTime? TestDateTo { get; set; }
        public string ResultDetailContains { get; set; } = "";

        // Additional filter options
        public List<int> TestIds { get; set; } = new List<int>();
        public List<int> UserIds { get; set; } = new List<int>();
        public string PatientName { get; set; } = "";
        public string UserName { get; set; } = "";
        public string TestName { get; set; } = "";
    }
}
