namespace AA2_CS.Model.DTOs
{
    // DTO para representar coordenadas geográficas con latitud y longitud. Utilizado en funcionalidades relacionadas con la ubicación, 
    // como el registro de check-ins o la visualización de mapas.
    public class CoordenadasDto
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
