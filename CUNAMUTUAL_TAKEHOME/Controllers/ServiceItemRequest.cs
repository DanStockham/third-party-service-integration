using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ServiceItemRequest
    {
        [Required]
        public string Body { get; set; }
    }
}