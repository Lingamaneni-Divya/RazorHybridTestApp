using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntuneMobilityViolation.Job.Tests
{
    public class ServiceExtensionsTests
    {
        [Fact]
        public async Task RetryPolicy_Should_Handle_Transient_Errors()
        {
            // Arrange
            int retryCount = 0;
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response => response.StatusCode == HttpStatusCode.ServiceUnavailable) // Ensure it retries on 503 errors
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (response, timespan, retryAttempt, context) =>
                    {
                        retryCount++;
                    });

            var handler = new FakeHttpMessageHandler();
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };

            // Act
            var response = await policy.ExecuteAsync(async () =>
            {
                return await httpClient.GetAsync("/test");
            });

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            Assert.Equal(3, retryCount); // It should retry 3 times before failing
        }

        // Fake HTTP handler to simulate transient failures
        private class FakeHttpMessageHandler : HttpMessageHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                await Task.Delay(50); // Simulate network delay
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable); // Always return 503 to trigger retry
            }
        }
    }
}