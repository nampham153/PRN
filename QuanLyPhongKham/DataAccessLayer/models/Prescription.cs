using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        public int RecordId { get; set; }
        [ForeignKey("RecordId")]
        public MedicalRecord MedicalRecord { get; set; }

        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        public int Quantity { get; set; }
        public string Dosage { get; set; }
    }
}

