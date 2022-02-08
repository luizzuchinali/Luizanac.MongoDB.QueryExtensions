namespace Luizanac.MongoDB.QueryExtensions.Tests;

[CollectionDefinition("TestDbContext")]
public class TestAppCollection : ICollectionFixture<TestDbContext>
{
}

public class TestDbContext : IDisposable
{
	internal static MongoDbRunner _runner;
	private readonly IMongoClient _client;
	private readonly IMongoDatabase _database;

	public TestDbContext()
	{
		CreateConnection();
		_client = new MongoClient(_runner.ConnectionString);
		_database = _client.GetDatabase("test-db");

		Users.Seed();
	}

	public IMongoCollection<User> Users => _database.GetCollection<User>(nameof(Users));

	internal static void CreateConnection()
	{
		_runner = MongoDbRunner.Start();
	}

	public void Dispose()
	{
		_runner.Dispose();
	}
}