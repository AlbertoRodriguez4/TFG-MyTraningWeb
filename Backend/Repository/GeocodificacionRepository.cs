using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using AA2_CS.Model.DTOs;
using AA2_CS.Model.External;

namespace AA2_CS.Repository
{
    public class GeocodificacionRepository
    {
        private readonly HttpClient _http;

        public GeocodificacionRepository(HttpClient http)
        {
            _http = http;
            _http.DefaultRequestHeaders.UserAgent.ParseAdd(
                "MiAppGeocodificacion/1.0 (contacto@tudominio.com)"
            );
        }

        public async Task<CoordenadasDto?> ObtenerCoordenadasAsync(string direccion)
        {
            var url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(direccion)}";

            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<NominatimResult>>(json, options);

                if (result == null || result.Count == 0) return null;

                return new CoordenadasDto
                {
                    Lat = double.Parse(result[0].Lat, System.Globalization.CultureInfo.InvariantCulture),
                    Lon = double.Parse(result[0].Lon, System.Globalization.CultureInfo.InvariantCulture)
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<Establecimientos>> BuscarEstablecimientosAsync(double lat, double lon, string tipo, int radio = 1000)
        {
            string latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

            string query = $@"[out:json];
            (
              node[""leisure""=""{tipo}""](around:{radio},{latStr},{lonStr});
              way[""leisure""=""{tipo}""](around:{radio},{latStr},{lonStr});
              relation[""leisure""=""{tipo}""](around:{radio},{latStr},{lonStr});
            );
            out center;";

            var url = "https://overpass-api.de/api/interpreter?data=" + Uri.EscapeDataString(query);

            try
            {
                var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<Establecimientos>();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<OverpassResult>(json, options);

                var lista = new List<Establecimientos>();
                var vistos = new HashSet<string>();

                if (result?.Elements != null)
                {
                    foreach (var el in result.Elements)
                    {
                        double elLat = el.Lat ?? el.Center?.Lat ?? 0;
                        double elLon = el.Lon ?? el.Center?.Lon ?? 0;

                        string direccionFinal = "Dirección no disponible";
                        if (el.Tags != null)
                        {
                            var partesDireccion = new List<string>();

                            if (!string.IsNullOrWhiteSpace(el.Tags.Street))
                                partesDireccion.Add(el.Tags.Street);

                            if (!string.IsNullOrWhiteSpace(el.Tags.HouseNumber))
                                partesDireccion.Add(el.Tags.HouseNumber);

                            if (!string.IsNullOrWhiteSpace(el.Tags.Postcode) && !string.IsNullOrWhiteSpace(el.Tags.City))
                                partesDireccion.Add($"({el.Tags.Postcode} {el.Tags.City})");

                            if (partesDireccion.Count > 0)
                                direccionFinal = string.Join(", ", partesDireccion);
                        }

                        var nombre = el.Tags?.Name?.Trim();
                        if (string.IsNullOrWhiteSpace(nombre) || nombre.Equals("Sin nombre", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string claveUnica = $"{nombre.ToLowerInvariant()}|{Math.Round(elLat, 4)}|{Math.Round(elLon, 4)}";
                        if (!vistos.Add(claveUnica))
                            continue;

                        string telefono = !string.IsNullOrWhiteSpace(el.Tags?.ContactPhone)
                            ? el.Tags.ContactPhone
                            : el.Tags?.Phone ?? string.Empty;

                        string sitioWeb = !string.IsNullOrWhiteSpace(el.Tags?.ContactWebsite)
                            ? el.Tags.ContactWebsite
                            : el.Tags?.Website ?? string.Empty;

                        string horario = el.Tags?.OpeningHours ?? string.Empty;
                        string accesibilidad = el.Tags?.Wheelchair ?? string.Empty;
                        string operador = el.Tags?.Operator ?? string.Empty;

                        lista.Add(new Establecimientos
                        {
                            Nombre = nombre,
                            Lat = elLat,
                            Lon = elLon,
                            Direccion = direccionFinal,
                            Tipo = tipo,
                            Telefono = telefono,
                            SitioWeb = sitioWeb,
                            HorarioApertura = horario,
                            Accesibilidad = accesibilidad,
                            Operador = operador
                        });
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                return new List<Establecimientos>();
            }
        }
    }
}
