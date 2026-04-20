using System.Net.Http.Json;

namespace CourseService.IntegrationTests.Common.Helpers;

public static class HttpRequestFactory
{
    public static HttpRequestMessage CreateAuthorized(HttpMethod method, string url, Guid userId)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-Test-UserId", userId.ToString());

        return request;
    }

    public static HttpRequestMessage CreateAuthorized<T>(HttpMethod method, string url, Guid userId, T body)
    {
        var request = CreateAuthorized(method, url, userId);
        request.Content = JsonContent.Create(body);

        return request;
    }

    public static HttpRequestMessage CreateUnauthorized(HttpMethod method, string url)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Add("X-Test-Auth-Fail", "true");

        return request;
    }

    public static HttpRequestMessage CreateUnauthorized<T>(HttpMethod method, string url, T body)
    {
        var request = CreateUnauthorized(method, url);
        request.Content = JsonContent.Create(body);

        return request;
    }
}
