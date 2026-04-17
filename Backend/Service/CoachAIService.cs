using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AA2_CS.Model;

namespace AA2_CS.Services
{
    public class CoachAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly string _model;

        public CoachAIService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["AiSettings:GroqApiUrl"] ?? throw new InvalidOperationException("Falta AiSettings:GroqApiUrl en la configuración.");
            _apiKey = configuration["AiSettings:GroqApiKey"] ?? throw new InvalidOperationException("Falta AiSettings:GroqApiKey en la configuración.");
            _model = configuration["AiSettings:Model"] ?? throw new InvalidOperationException("Falta AiSettings:Model en la configuración.");
        }

        public async Task<string> GenerarRespuestaAsync(CoachAIChatRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Mensaje))
            {
                throw new ArgumentException("El mensaje es obligatorio.");
            }

            var mensajes = new List<object>
            {
                new
                {
                    role = "system",
                    content = ConstruirPromptSistema(request.ContextoUsuario)
                }
            };

            foreach (var mensaje in request.Historial.Where(EsMensajeValido))
            {
                mensajes.Add(new
                {
                    role = mensaje.Role.Trim().ToLowerInvariant(),
                    content = mensaje.Content.Trim()
                });
            }

            var mensajeUsuario = request.Mensaje.Trim();
            if (!request.Historial.Any(m => string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(m.Content?.Trim(), mensajeUsuario, StringComparison.Ordinal)))
            {
                mensajes.Add(new { role = "user", content = mensajeUsuario });
            }

            var payload = new
            {
                model = _model,
                messages = mensajes,
                max_tokens = 1000,
                temperature = 0.7
            };

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, _apiUrl)
            {
                Content = JsonContent.Create(payload)
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            using var response = await _httpClient.SendAsync(requestMessage, cancellationToken);
            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Error de IA ({(int)response.StatusCode}): {responseJson}");
            }

            using var jsonDocument = JsonDocument.Parse(responseJson);
            if (!jsonDocument.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("La IA no devolvió una respuesta válida.");
            }

            var reply = choices[0].GetProperty("message").GetProperty("content").GetString();
            if (string.IsNullOrWhiteSpace(reply))
            {
                throw new InvalidOperationException("La IA devolvió una respuesta vacía.");
            }

            return reply.Trim();
        }

        private static bool EsMensajeValido(CoachAIMessage mensaje)
        {
            if (mensaje == null || string.IsNullOrWhiteSpace(mensaje.Content))
            {
                return false;
            }

            var role = mensaje.Role?.Trim().ToLowerInvariant();
            return role == "user" || role == "assistant";
        }

        private static string ConstruirPromptSistema(string? contextoUsuario)
        {
            var promptBase = """
                             Eres CoachAI, un entrenador personal y nutricionista virtual experto, amigable y motivador.
                             Respondes únicamente preguntas sobre actividad física, entrenamiento, nutrición deportiva, recuperación muscular y bienestar físico.
                             Si alguien pregunta algo fuera de estos temas, redirige amablemente al fitness.
                             Tono: energético, motivador y cercano.
                             Formato: usa negritas para ideas clave, listas con "•" y respuestas concisas pero completas.
                             Responde siempre en español.
                             """;

            if (string.IsNullOrWhiteSpace(contextoUsuario))
            {
                return promptBase;
            }

            return $"{promptBase}\n\n{contextoUsuario.Trim()}";
        }
    }
}
