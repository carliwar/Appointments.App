namespace Appointments.App.Models.ContificoModels.Requests
{
    internal class ContificoPersonSaveRequest
    {
        public string id { get; set; }
        public string tipo { get; set; }
        public string razon_social { get; set; }
        public string telefonos { get; set; }
        public string cedula { get; set; }
        public string email { get; set; }
        public string direccion { get; set; }
        public string ruc { get; set; }
        public bool es_extranjero { get; set; }
        public bool es_vendedor { get; set; }
        public bool es_cliente { get; set; }
        public bool es_empleado { get; set; }
        public bool es_proveedor { get; set; }
    }
}
