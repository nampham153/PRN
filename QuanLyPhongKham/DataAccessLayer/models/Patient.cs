using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}