using System.ComponentModel.DataAnnotations;

namespace CUNAMUTUAL_TAKEHOME
{
    public class ServiceItem
    {
        [Key]
        public string Identifier { get; set; }
        public Statuses Status { get; set; }
        public string Detail { get; set; }
    }

    public enum Statuses
    {
        NONE,
        STARTED,
        PROCESSED,
        COMPLETED,
        ERROR
    }
}