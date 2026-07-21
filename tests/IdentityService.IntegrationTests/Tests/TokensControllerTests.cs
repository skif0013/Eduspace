using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using IdentityService.IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.IntegrationTests.Tests
{
    // Интеграционные тесты используют in-memory SQLite (no Docker)
    public class TokensControllerTests : IntegrationTestBase
    {
        public TokensControllerTests() : base()
        {
        }

        [Fact]
        public async Task RevokeRefreshToken_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Tokens/RevokeRefreshToken");
            request.Content = new StringContent(JsonSerializer.Serialize(new { refreshToken = "invalid" }),
                Encoding.UTF8, "application/json");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            root.TryGetProperty("isError", out var isErrorProp).Should()
                .BeTrue($"Ожидали Result-ответ от middleware. Body: {content}");
            isErrorProp.GetBoolean().Should().BeTrue($"Ожидали isError=true в middleware Result. Body: {content}");
        }

        [Fact]
        public async Task RevokeRefreshToken_WithValidAccessToken_ReturnsOk()
        {
            // Arrange
            // Извлекаем оба реальных токена, сгенерированных при логине
            var (validAccessToken, validRefreshToken) = await Arrange_AndGetTokensAsync();

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Tokens/RevokeRefreshToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", validAccessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(new { refreshToken = validRefreshToken }), 
                Encoding.UTF8, "application/json");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Надежно создает подтвержденного пользователя напрямую в SQLite и логинит его через API
        /// </summary>
        private async Task<(string AccessToken, string RefreshToken)> Arrange_AndGetTokensAsync()
        {
            var email = "test@example.com";
            var password = "P@ssw0rd1";
            var userName = "testuser_" + Guid.NewGuid().ToString("N").Substring(0, 6);

            // 1) Создаем и подтверждаем пользователя напрямую в БД (обходим капризный Outbox)
            using (var scope = Factory.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider
                    .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<IdentityService.Domain.Entities.User>>();

                var user = new IdentityService.Domain.Entities.User 
                { 
                    UserName = userName, 
                    Email = email,
                    EmailConfirmed = true // Подтверждаем почту сразу
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Direct user creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                }
            }

            // 2) Логинимся через стандартный эндпоинт, чтобы IdentityService сгенерировал токены
            var login = new { email = email, password = password };
            var loginResp = await Client.PostAsync("/api/Users/login",
                new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json"));
            loginResp.EnsureSuccessStatusCode();

            var json = await loginResp.Content.ReadAsStringAsync();
            
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            
            // 3) Извлекаем токены из ответа (с учетом возможной Result-обертки)
            JsonElement dataElement;
            if (root.TryGetProperty("data", out dataElement))
            {
                var access = dataElement.GetProperty("accessToken").GetString()!;
                var refresh = dataElement.GetProperty("refreshToken").GetString()!;
                return (access, refresh);
            }
            
            if (root.TryGetProperty("accessToken", out var directToken))
            {
                var access = directToken.GetString()!;
                var refresh = root.GetProperty("refreshToken").GetString()!;
                return (access, refresh);
            }
            
            throw new InvalidOperationException($"Could not find tokens in login response. Content: {json}");
        }
    }
}