using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CUNAMUTUAL_TAKEHOME
{
    public class ServiceItem
    {
        [Key]
        [JsonIgnore]
        public string Identifier { get; set; }
        public string Body { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Statuses Status { get; set; }
        public string Detail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdateAt { get; set; }
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