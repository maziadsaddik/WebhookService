using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;
using WebhookService.Appliaction.Contract;
using WebhookService.Domain.Entities;

namespace WebhookService.Infrastructure.Services
{
    public class WebhookDispatcher : IWebhookDispatcher
    {
        private readonly HttpClient _httpClient;
        private readonly ICryptoService _cryptoService;
        private readonly ILogger<WebhookDispatcher> _logger;

        public WebhookDispatcher(
            HttpClient httpClient,
            ICryptoService cryptoService,
            ILogger<WebhookDispatcher> logger
        )
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            _cryptoService = cryptoService;
            _logger = logger;
        }

        public async Task<DeliveryResult> DispatchAsync(
            Subscriber subscriber,
            Event @event,
            Delivery delivery
        )
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, subscriber.EndpointUrl);

                request.Content = new StringContent(@event.Payload, Encoding.UTF8, "application/json");

                // Add security headers
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                var bodyHash = ComputeSHA256Hash(@event.Payload);

                var signaturePayload = $"v1:{timestamp}:{@event.Id}:{bodyHash}";

                var secret = _cryptoService.Decrypt(subscriber.EncryptedSecret);

                var signature = _cryptoService.ComputeSignature(secret, signaturePayload);

                request.Headers.Add(
                    "X-SWR-Signature",
                    $"v1,ts={timestamp},kid={subscriber.KeyId},sig={signature}"
                );

                request.Headers.Add("X-SWR-Event-Id", @event.Id.ToString());

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                stopwatch.Stop();

                return new DeliveryResult
                {
                    Success = response.IsSuccessStatusCode,
                    HttpStatusCode = (int)response.StatusCode,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }
            catch (TaskCanceledException)
            {
                stopwatch.Stop();

                return new DeliveryResult
                {
                    Success = false,
                    ErrorMessage = "Request timeout",
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex, "Failed to dispatch webhook");

                return new DeliveryResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    DurationMs = stopwatch.ElapsedMilliseconds
                };
            }
        }

        private string ComputeSHA256Hash(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input.ToLower()));

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}