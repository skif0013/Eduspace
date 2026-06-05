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
            // Пример: без Authorization (или с неверным токеном) — ожидаем 401/400 в зависимости от логики
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
            // 1) Зарегистрировать пользователя или подготовить запись в БД (вызовом endpoint'а регистрации или напрямую через DbContext)
            // 2) Получить корректный access token (можно вызвать auth endpoint или сформировать через TokenService если есть helper)
            // Ниже замените `validAccessToken` и `validRefreshToken` на реальные значения, полученные в Arrange
            string validAccessToken = await Arrange_AndGetAccessTokenAsync();
            string validRefreshToken = "тут-refresh-token-из-БД";

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/Tokens/RevokeRefreshToken");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", validAccessToken);
            request.Content = new StringContent(JsonSerializer.Serialize(new { refreshToken = validRefreshToken }), Encoding.UTF8, "application/json");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<string> Arrange_AndGetAccessTokenAsync()
{
    var email = "test@example.com";
    var password = "P@ssw0rd1";
    var userName = "testuser_" + Guid.NewGuid().ToString("N").Substring(0, 6);

    // 1) Регистрация через API
    var register = new { userName = userName, email = email, password = password };
    var regResp = await Client.PostAsync("/api/Users/register",
        new StringContent(JsonSerializer.Serialize(register), Encoding.UTF8, "application/json"));
    regResp.EnsureSuccessStatusCode();

    // 2) Ожидаем появление OutboxMessage и извлекаем код подтверждения
    string? token = null;
    var maxAttempts = 20;
    var delayMs = 100;
    for (int attempt = 0; attempt < maxAttempts; attempt++)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<IdentityService.Infrastructure.Database.ApplicationDbContext>();

        var outboxMsg = await db.OutboxMessages
            .Where(m => m.Type == "identity.user.created" && m.Content.Contains(email))
            .OrderByDescending(m => m.OccurredOnUtc)
            .FirstOrDefaultAsync();

        if (outboxMsg != null)
        {
            try
            {
                var evt =
                    System.Text.Json.JsonSerializer.Deserialize<Shared.Messages.EmailVerifyEvent>(
                        outboxMsg.Content);
                token = evt?.Code;
                if (!string.IsNullOrEmpty(token)) break;
            }
            catch
            {
                // игнорируем парсинг-ошибки в попытках
            }
        }

        await Task.Delay(delayMs);
    }

    if (string.IsNullOrEmpty(token))
    {
        throw new InvalidOperationException("Confirmation token not found in Outbox messages.");
    }

    // 3) Подтверждаем email через UserManager
    using (var scope = Factory.Services.CreateScope())
    {
        var userManager = scope.ServiceProvider
            .GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<
                IdentityService.Domain.Entities.User>>();
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) throw new InvalidOperationException("User not found for confirm.");
        var confirmResult = await userManager.ConfirmEmailAsync(user, token);
        if (!confirmResult.Succeeded)
            throw new InvalidOperationException($"ConfirmEmailAsync failed: {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
    }

    // 4) Логинимся и возвращаем access token
    var login = new { email = email, password = password };
    var loginResp = await Client.PostAsync("/api/Users/login",
        new StringContent(JsonSerializer.Serialize(login), Encoding.UTF8, "application/json"));
    loginResp.EnsureSuccessStatusCode();

    var json = await loginResp.Content.ReadAsStringAsync();
    Console.WriteLine($"Login Response: {json}");
    
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;
    
    // Попробуем найти поле "data" (вероятно, Response обернут в структуру Result)
    if (root.TryGetProperty("data", out var dataElement) && dataElement.TryGetProperty("accessToken", out var tokenElement))
    {
        return tokenElement.GetString()!;
    }
    
    // Или напрямую accessToken
    if (root.TryGetProperty("accessToken", out var directToken))
    {
        return directToken.GetString()!;
    }
    
    throw new InvalidOperationException($"Could not find accessToken in login response. Content: {json}");
    }
    }
}