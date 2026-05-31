using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AA2_CS.Model.DTOs;

namespace AA2_CS.Services
{
    public class CoachAIService
    {
        //Se realiza la integración con la API de Groq para generar respuestas de CoachAI basadas en el historial de chat y el contexto del usuario.
        //No se necesita un repositorio o entidad específica, ya que esta clase se enfoca únicamente en la comunicación con la API de IA.
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly string _model;

        public CoachAIService(HttpClient httpClient, IConfiguration configuration)
        {
            // Se inyecta HttpClient para realizar las solicitudes HTTP a la API de Groq, y IConfiguration para obtener las configuraciones necesarias (URL, clave API, modelo).
            // Todas las variables de configuración son obligatorias, y se lanzará una excepción si falta alguna para evitar errores en tiempo de ejecución.
            // Se obtienen de la sección "AiSettings" en appsettings.json, lo que permite una fácil modificación sin necesidad de recompilar el código.
            _httpClient = httpClient;
            _apiUrl = configuration["AiSettings:GroqApiUrl"] ?? throw new InvalidOperationException("Falta AiSettings:GroqApiUrl en la configuración.");
            _apiKey = configuration["AiSettings:GroqApiKey"] ?? throw new InvalidOperationException("Falta AiSettings:GroqApiKey en la configuración.");
            _model = configuration["AiSettings:Model"] ?? throw new InvalidOperationException("Falta AiSettings:Model en la configuración.");
        }

        public async Task<string> GenerarRespuestaAsync(CoachAIChatRequest request, CancellationToken cancellationToken = default)
        { // Validación básica de entrada
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
            // Agrega al historial solo los mensajes válidos (con contenido no vacío y rol correcto)
            foreach (var mensaje in request.Historial.Where(EsMensajeValido))
            {
                mensajes.Add(new
                {
                    role = mensaje.Role.Trim().ToLowerInvariant(),
                    content = mensaje.Content.Trim()
                });
            }
            // Agrega el mensaje del usuario actual solo si no es idéntico a un mensaje previo del mismo rol para evitar duplicados en el historial
            var mensajeUsuario = request.Mensaje.Trim();
            if (!request.Historial.Any(m => string.Equals(m.Role, "user", StringComparison.OrdinalIgnoreCase)
                                            && string.Equals(m.Content?.Trim(), mensajeUsuario, StringComparison.Ordinal)))
            {
                mensajes.Add(new { role = "user", content = mensajeUsuario });
            }
            // Construye el payload para la API de Groq, incluyendo el modelo, el historial de mensajes y parámetros de generación
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
            // Comprobar que el mensaje no es nulo, tiene contenido no vacío y un rol válido (user o assistant)
            if (mensaje == null || string.IsNullOrWhiteSpace(mensaje.Content))
            {
                return false;
            }

            var role = mensaje.Role?.Trim().ToLowerInvariant();
            return role == "user" || role == "assistant";
        }

        private static string ConstruirPromptSistema(string? contextoUsuario)
        { 
            // El prompt del sistema establece el comportamiento y tono de CoachAI, asegurando que las respuestas sean relevantes y motivadoras para temas de fitness, 
            // entrenamiento y nutrición.
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
