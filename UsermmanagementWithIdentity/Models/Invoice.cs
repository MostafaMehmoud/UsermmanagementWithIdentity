using System.ComponentModel.DataAnnotations;

namespace UsermmanagementWithIdentity.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public DateTime DateOfRegistration { get; set; }   
        [Required]
        public int Code { get; set; }
        public string ProductName {  get; set; }
        public decimal Price { get; set; }
        public DateTime DateOfCreation { get; set; }


    }
}
