using System.Runtime.Serialization;

namespace InDaCompany.Data.Implementations
{
    [Serializable]
    public class DAOException : Exception
    {
        public DAOException() : base("Si è verificato un errore durante l'accesso ai dati.") { }

        public DAOException(string message) : base(message) { }

        public DAOException(string message, Exception innerException)
            : base(message, innerException) { }

        protected DAOException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public string? SqlState { get; set; }
        public int? ErrorCode { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            if (SqlState != null || ErrorCode.HasValue)
            {
                return $"{baseString}\nSQL State: {SqlState ?? "N/A"}\nError Code: {ErrorCode?.ToString() ?? "N/A"}";
            }
            return baseString;
        }
    }
}
