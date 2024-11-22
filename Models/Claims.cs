using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models;

namespace PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models
{
    public class Claims
    {
        [Key]
        public int ClaimId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ClaimantName { get; set; }
        [Required]
        public decimal HourlyRate { get; set; }
        [Required]
        public int HoursWorked { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ClaimantComments { get; set; }
        [Required]
        public DateTime DateLogged { get; set; }
        [Column(TypeName = "nvarchar(max)")]
        public string UploadedFiles { get; set; } = "file";
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Status { get; set; }
    }
}
