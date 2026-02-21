namespace CourseService.Infrastructure.Caching;

public class RadisCacheSettings
{
    public int CatalogExpirationMinutes { get; set; }
    public int CourseExpirationMinutes { get; set; }
}
