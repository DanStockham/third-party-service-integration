using System.ComponentModel.DataAnnotations;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ServiceItemUpdate
    {
        [Required]
        public string Status { get; set; }
        [Required]
        public string Detail { get; set; }
    }
}