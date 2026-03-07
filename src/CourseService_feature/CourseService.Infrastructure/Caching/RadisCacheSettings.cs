namespace CourseService.Infrastructure.Caching;

public class RadisCacheSettings
{
    public string KeyPrefix { get; set; }
    public int CatalogExpirationMinutes { get; set; }
    public int CourseExpirationMinutes { get; set; }
}
