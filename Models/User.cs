using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SampleAppWithCaching.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; }

        //Enable this property for optimistic locking
        //[JsonIgnore]
        //[ConcurrencyCheck]
        //public byte[]? RowVersion { get; set; }
    }
}
