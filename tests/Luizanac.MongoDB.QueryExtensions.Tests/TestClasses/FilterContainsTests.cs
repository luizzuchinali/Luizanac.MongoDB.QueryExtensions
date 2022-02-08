namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class FilterContainsTests : BaseTest
{
	public FilterContainsTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Fact]
	public async Task Contains_ShouldReturn_AListThatContainsUsersWithValue()
	{
		//Arrange
		var user = await DbContext.Users.GetFirst();
		var containsValue = user.Name[1..3];
		var filter = $"{nameof(User.Name)}@={containsValue}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Name.Contains(containsValue, StringComparison.InvariantCultureIgnoreCase));
	}

	[Fact]
	public async Task NotContains_ShouldReturn_AListThatNotContainsUsersWithValue()
	{
		//Arrange
		var user = await DbContext.Users.GetFirst();
		var containsValue = user.Name[1..3];
		var filter = $"{nameof(User.Name)}!@={containsValue}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => !x.Name.Contains(containsValue, StringComparison.InvariantCultureIgnoreCase));
	}
}