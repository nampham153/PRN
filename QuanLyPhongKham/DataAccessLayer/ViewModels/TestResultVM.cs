using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.ViewModels
{
    public class TestResultVM
    {
        [Required(ErrorMessage = "Medical Record is required")]
        public int RecordId { get; set; }

        [Required(ErrorMessage = "Test is required")]
        public int TestId { get; set; }

        [Required(ErrorMessage = "User is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Result Detail is required")]
        [StringLength(1000, ErrorMessage = "Result Detail cannot exceed 1000 characters")]
        public string ResultDetail { get; set; }

        [Required(ErrorMessage = "Test Date is required")]
        [DataType(DataType.DateTime)]
        public DateTime TestDate { get; set; }

        // Display properties for dropdown lists and information display
        public string TestName { get; set; }
        public string TestDescription { get; set; }
        public string UserName { get; set; }
        public string PatientName { get; set; }
        public string MedicalRecordDate { get; set; }
        public string Diagnosis { get; set; }

        // Additional helper properties for better user experience
        public string FormattedTestDate => TestDate.ToString("dd/MM/yyyy HH:mm");
        public string DisplayText => $"{TestName} - {PatientName} ({FormattedTestDate})";
    }
}
