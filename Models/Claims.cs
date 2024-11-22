using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models;

namespace PROG_2B_POE_Final_Part_ST10438307_Daniel_Gorin.Models
{
    public class Claims
    {
        [Key]
        public int ClaimId { get; set; }//Primary Key
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ClaimantName { get; set; }//First and Last Name of Claimant
        [Required]
        public decimal HourlyRate { get; set; }//Amount payed per hour in Rands
        [Required]
        public int HoursWorked { get; set; }//number fo hours worked for the claim
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ClaimantComments { get; set; }//any additional comments the claimant may have
        [Required]
        public DateTime DateLogged { get; set; }//Date the claimant submited the claim
        [Column(TypeName = "nvarchar(max)")]
        public string UploadedFiles { get; set; } = "file";
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Status { get; set; }//Whther the claim is pending, accepted or denied
    }
}
