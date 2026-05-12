using System.Text.Json;
using System.Text.Json.Serialization;

namespace AA2_CS.Service
{
    public class ExerciseDbClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExerciseDbClient> _logger;
        private const string BaseUrl = "https://exercisedb.p.rapidapi.com";

        public ExerciseDbClient(HttpClient httpClient, IConfiguration configuration, ILogger<ExerciseDbClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<ExerciseDbResponse>> GetAllExercisesAsync(int limit = 50)
        {
            return await FetchExercisesAsync($"{BaseUrl}/exercises?limit={limit}");
        }

        public async Task<List<ExerciseDbResponse>> GetExercisesByTargetAsync(string targetMuscle)
        {
            return await FetchExercisesAsync($"{BaseUrl}/exercises/target/{Uri.EscapeDataString(targetMuscle)}");
        }

        public async Task<List<ExerciseDbResponse>> SearchExercisesAsync(string query)
        {
            return await FetchExercisesAsync($"{BaseUrl}/exercises/name/{Uri.EscapeDataString(query)}");
        }

        private async Task<List<ExerciseDbResponse>> FetchExercisesAsync(string url)
        {
            var apiKey = _configuration["ExerciseDbSettings:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "TU_API_KEY_AQUI")
            {
                _logger.LogWarning("ExerciseDB API Key no configurada.");
                return new List<ExerciseDbResponse>();
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("x-rapidapi-key", apiKey);
                request.Headers.Add("x-rapidapi-host", "exercisedb.p.rapidapi.com");

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    _logger.LogError("ExerciseDB error {StatusCode}: {Body}", response.StatusCode, errorBody);
                    return new List<ExerciseDbResponse>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var exercises = JsonSerializer.Deserialize<List<ExerciseDbResponse>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return exercises ?? new List<ExerciseDbResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a ExerciseDB API. URL: {Url}", url);
                return new List<ExerciseDbResponse>();
            }
        }

        // Mapeo de grupo muscular del frontend a target de ExerciseDB
        public static string? MapMuscleGroupToTarget(string muscleGroup)
        {
            var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "chest", "pectorals" },
                { "back", "lats" },
                { "shoulders", "delts" },
                { "arms", "biceps" },
                { "legs", "quadriceps" },
                { "core", "abs" },
                { "cardio", "cardiovascular system" }
            };

            return mapping.TryGetValue(muscleGroup, out var target) ? target : muscleGroup;
        }
    }

    public class ExerciseDbResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("target")]
        public string Target { get; set; } = string.Empty;

        [JsonPropertyName("bodyPart")]
        public string BodyPart { get; set; } = string.Empty;

        [JsonPropertyName("equipment")]
        public string Equipment { get; set; } = string.Empty;

        [JsonPropertyName("gifUrl")]
        public string GifUrl { get; set; } = string.Empty;

        [JsonPropertyName("secondaryMuscles")]
        public List<string> SecondaryMuscles { get; set; } = new();

        [JsonPropertyName("instructions")]
        public List<string> Instructions { get; set; } = new();

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
    }
}
