using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DataAccessLayer.models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string Unit { get; set; }
        public string Usage { get; set; }

        [JsonIgnore]
        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
