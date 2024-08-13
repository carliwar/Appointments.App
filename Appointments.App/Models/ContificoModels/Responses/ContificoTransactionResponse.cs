using System.Text.Json.Serialization;

namespace Appointments.App.Models.ContificoModels.Responses
{
    public class ContificoError
    {
        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }

        [JsonPropertyName("cod_error")]
        public int Cod_Error { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        public ContificoError(string mensaje, int cod_Error)
        {
            Mensaje = mensaje;
            Cod_Error = cod_Error;
        }
    }

    public class ContificoTransactionResponse
    {
        [JsonPropertyName("contificoError")]
        public ContificoError Error { get; set; }
        public bool IsValid
        {
            get => Error == null;
        }
        [JsonPropertyName("statusCode")]
        public int StatusCode { get; set; } = 200;

        [JsonPropertyName("objectId")]
        public string ObjectId { get; set; }
    }
}
