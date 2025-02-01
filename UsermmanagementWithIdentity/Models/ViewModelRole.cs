using System.ComponentModel.DataAnnotations;

namespace UsermmanagementWithIdentity.Models
{
    public class ViewModelRole
    {
        [Required]
        [StringLength(250)]
        public string Name { get; set; }
    }
}
