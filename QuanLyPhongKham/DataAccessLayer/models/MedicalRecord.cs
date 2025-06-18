using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime Date { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string Note { get; set; }

        public ICollection<TestResult> TestResults { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
