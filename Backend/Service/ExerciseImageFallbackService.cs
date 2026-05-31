using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AA2_CS.Service
{
    public class ExerciseImageFallbackService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ExerciseImageFallbackService> _logger;
        private List<ExerciseImageEntry>? _entries;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public ExerciseImageFallbackService(IHttpClientFactory httpClientFactory, ILogger<ExerciseImageFallbackService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string? GetImageUrl(string exerciseName)
        { // Obtener las imagenes del ejercicio con diferentes niveles de coincidencia 
            if (_entries == null)
            {
                LoadAsync().GetAwaiter().GetResult();
            }

            if (_entries == null || _entries.Count == 0)
                return null;

            var normalized = NormalizeName(exerciseName);

            // 1. Coincidencia exacta
            var exact = _entries.FirstOrDefault(e => e.NormalizedName == normalized);
            if (exact != null)
                return exact.ImageUrl;

            // 2. Coincidencia parcial (contains bidireccional)
            var partial = _entries.FirstOrDefault(e =>
                e.NormalizedName.Contains(normalized) || normalized.Contains(e.NormalizedName));
            if (partial != null)
                return partial.ImageUrl;

            // 3. Coincidencia por palabras clave significativas
            var keywords = GetSignificantKeywords(normalized);
            if (keywords.Length > 0)
            {
                var keywordMatch = _entries
                    .Select(e => new
                    {
                        Entry = e,
                        Score = keywords.Count(k => e.NormalizedName.Contains(k))
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .FirstOrDefault();

                if (keywordMatch != null && keywordMatch.Score >= Math.Max(1, keywords.Length / 2))
                    return keywordMatch.Entry.ImageUrl;
            }

            return null;
        }

        private async Task LoadAsync()
        { // Cargar el fallback de imágenes desde un JSON público, con manejo de concurrencia para evitar múltiples cargas simultáneas
            await _semaphore.WaitAsync();
            try
            {
                if (_entries != null) return;

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                var json = await client.GetStringAsync("https://raw.githubusercontent.com/yuhonas/free-exercise-db/main/dist/exercises.json");
                var exercises = JsonSerializer.Deserialize<List<FreeExerciseDbItem>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var list = new List<ExerciseImageEntry>();
                if (exercises != null)
                {
                    foreach (var ex in exercises)
                    {
                        if (ex.Images != null && ex.Images.Count > 0)
                        {
                            list.Add(new ExerciseImageEntry
                            { // Construir la URL de la imagen usando el primer nombre de imagen disponible, asumiendo que el JSON tiene un formato específico
                                Name = ex.Name,
                                NormalizedName = NormalizeName(ex.Name),
                                ImageUrl = $"https://raw.githubusercontent.com/yuhonas/free-exercise-db/main/exercises/{ex.Images[0]}"
                            });
                        }
                    }
                }

                _entries = list;
                _logger.LogInformation("Fallback de imágenes cargado con {Count} ejercicios.", list.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cargando fallback de imágenes de ejercicios.");
                _entries = new List<ExerciseImageEntry>();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static string NormalizeName(string name)
        { // Normalizar el nombre del ejercicio para mejorar las coincidencias, eliminando caracteres especiales y palabras comunes
            var cleaned = name.ToLowerInvariant().Trim();
            // Eliminar caracteres especiales comunes: ° , . ( ) - /
            cleaned = Regex.Replace(cleaned, @"[°.,()\-/]", " ");
            // Compactar espacios múltiples
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            return cleaned;
        }

        private static string[] GetSignificantKeywords(string normalizedName)
        { // Extraer palabras clave significativas, eliminando palabras comunes y muy cortas para mejorar la coincidencia por palabras clave
            var stopWords = new HashSet<string> { "the", "a", "an", "and", "or", "with", "on", "in", "of", "to", "for", "from", "by", "assisted", "machine", "leverage" };
            return normalizedName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 2 && !stopWords.Contains(w))
                .ToArray();
        }

        private class ExerciseImageEntry
        {
            public string Name { get; set; } = string.Empty;
            public string NormalizedName { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
        }

        private class FreeExerciseDbItem
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("images")]
            public List<string> Images { get; set; } = new();
        }
    }
}
