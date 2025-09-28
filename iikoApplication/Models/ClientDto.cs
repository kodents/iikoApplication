using System.ComponentModel.DataAnnotations;

namespace iikoApplication.Models
{
    /// <summary> Client data transfer object </summary>
    public class ClientDto
    {
        /// <summary> Unique client identifier </summary>
        [Required]
        [Range(1, long.MaxValue)]
        public long ClientId { get; set; }

        /// <summary> Username </summary>
        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;
    }
}