using CourseService.IntegrationTests.Common.Fixtures;

namespace CourseService.IntegrationTests.Collections;

[CollectionDefinition("Postgres collection")]
public class PostgresCollection : ICollectionFixture<PostgresContainerFixture>
{
}
