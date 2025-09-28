namespace iikoApplication.Models
{
    /// <summary> Client entity </summary>
    public class Client
    {
        /// <summary> Unique business identifier  </summary>
        public long ClientId { get; set; }

        /// <summary> Username </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary> System-generated unique identifier </summary>
        public Guid SystemId { get; set; } = Guid.NewGuid();
    }
}