namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

[Collection("TestDbContext")]
public class BaseTest
{
	protected readonly TestDbContext DbContext;

	public BaseTest(TestDbContext dbContext)
	{
		DbContext = dbContext;
	}
}