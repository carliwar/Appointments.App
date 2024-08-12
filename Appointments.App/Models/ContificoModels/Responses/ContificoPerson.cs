using System.Text.Json.Serialization;

namespace Appointments.App.Models.ContificoModels.Responses
{
    public class ContificoPerson
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }

        [JsonPropertyName("razon_social")]
        public string RazonSocial { get; set; }

        [JsonPropertyName("nombre_comercial")]
        public string NombreComercial { get; set; }

        [JsonPropertyName("ruc")]
        public string Ruc { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("es_cliente")]
        public bool EsCliente { get; set; }

        [JsonPropertyName("es_extranjero")]
        public bool EsExtranjero { get; set; }

        [JsonPropertyName("telefonos")]
        public string Telefonos { get; set; }

        [JsonPropertyName("cedula")]
        public string Cedula { get; set; }
    }
}
