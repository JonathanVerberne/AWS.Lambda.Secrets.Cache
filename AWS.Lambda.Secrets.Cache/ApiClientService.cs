using Amazon.XRay.Recorder.Handlers.System.Net;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AWS.Lambda.Secrets.Cache
{
    public class ApiClientService
    {
        private readonly HttpMethod _method;
        private readonly Uri _endpointUrl;
        private readonly string _token;
        private readonly ILogger<ApiClientService> _logger;

        public ApiClientService(HttpMethod method, Uri endpointUrl, string? token = null)
        {
            _method = method;
            _endpointUrl = endpointUrl;
            _token = token;

            _logger = LoggerFactory
                        .Create(builder => builder.AddConsole())
                        .CreateLogger<ApiClientService>();
        }

        public async Task<(String, HttpStatusCode)> ExecuteHttpRequest(object request)
        {
            var stopWatch = new Stopwatch();

            try
            {
                using var httpClient = new HttpClient(new HttpClientXRayTracingHandler(new HttpClientHandler()));

                if (_token != null)
                {
                    httpClient.DefaultRequestHeaders.Add("X-Aws-Parameters-Secrets-Token", _token);
                }

                using var httpRequestMessage = new HttpRequestMessage(_method, _endpointUrl);

                if (request != null)
                {
                    _logger.LogInformation($"API request content -> {JsonSerializer.Serialize(request)}");
                    httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(request));
                    httpRequestMessage.Content.Headers.Clear();
                    httpRequestMessage.Content.Headers.Add("Content-Type", "application/json");
                }

                _logger.LogInformation($"Initiating API call -> {_endpointUrl}");

                stopWatch.Start();
                var apiResponse = await httpClient.SendAsync(httpRequestMessage);
                stopWatch.Stop();

                var responseContent = await apiResponse.Content.ReadAsStringAsync();

                _logger.LogInformation($"API response <- {responseContent}, latency:{stopWatch.ElapsedMilliseconds}ms");
                _logger.LogInformation($"API http status response <- {apiResponse.StatusCode}");

                return (responseContent, apiResponse.StatusCode);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Failed to contact service container target.");
                throw;
            }
            catch (TaskCanceledException taskEx)
            {
                _logger.LogError(taskEx, "Timeout trying to contact service container target.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in API Service");
                throw;
            }
        }
    }
}
