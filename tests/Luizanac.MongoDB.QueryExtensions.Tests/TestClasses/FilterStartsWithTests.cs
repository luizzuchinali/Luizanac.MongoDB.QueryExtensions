namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class FilterStartsWithTests : BaseTest
{
	public FilterStartsWithTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Fact]
	public async Task StartsWith_ShouldReturn_AListThatStartsWithValue()
	{
		//Arrange
		var user = await DbContext.Users.GetFirst();
		var startWithValue = user.Name[..4];
		var filter = $"{nameof(User.Name)}_={startWithValue}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Name.StartsWith(startWithValue));
	}

	[Fact]
	public async Task NotStartsWith_ShouldReturn_AListThatNotStartsWithValue()
	{
		//Arrange
		var user = await DbContext.Users.GetFirst();
		var startWithValue = user.Name[..4];
		var filter = $"{nameof(User.Name)}!_={startWithValue}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => !x.Name.StartsWith(startWithValue));
	}
}