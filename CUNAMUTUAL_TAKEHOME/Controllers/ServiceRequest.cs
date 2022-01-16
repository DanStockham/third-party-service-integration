using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CUNAMUTUAL_TAKEHOME.Controllers
{
    public class ServiceRequest
    {
        [JsonIgnore]
        public string Id { get; set; }
        [Required]
        public string Body { get; set; }
    }
}