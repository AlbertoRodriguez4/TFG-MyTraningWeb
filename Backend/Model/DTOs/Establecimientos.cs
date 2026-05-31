namespace AA2_CS.Model.DTOs
{
    // DTO para representar establecimientos. Incluye información como nombre, ubicación (latitud y longitud), dirección, tipo de establecimiento, 
    // teléfono, sitio web, horario de apertura, accesibilidad y operador.
    public class Establecimientos
    {
        public string Nombre { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Direccion { get; set; }
        public string Tipo { get; set; }
        public string Telefono { get; set; }
        public string SitioWeb { get; set; }
        public string HorarioApertura { get; set; }
        public string Accesibilidad { get; set; }
        public string Operador { get; set; }

        public Establecimientos() { }

        public Establecimientos(string nombre, string direccion, double lat, double lon)
        {
            Nombre = nombre;
            Direccion = direccion;
            Lat = lat;
            Lon = lon;
        }
    }
}
