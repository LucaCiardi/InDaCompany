namespace InDaCompany.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }

        public string UserFriendlyMessage => ErrorMessage ?? "Si è verificato un errore durante l'elaborazione della richiesta.";

        public bool IsNotFound => StatusCode == 404;
        public bool IsServerError => StatusCode >= 500;
    }
}
