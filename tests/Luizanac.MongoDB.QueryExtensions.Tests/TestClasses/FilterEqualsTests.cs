namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class FilterEqualsTests : BaseTest
{
	public FilterEqualsTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Theory]
	[InlineData(56)]
	[InlineData(32)]
	public async Task Equals_ShouldReturn_ANotEmptyList(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}=={value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age == value);
	}

	[Theory]
	[InlineData(56)]
	[InlineData(32)]
	public async Task NotEquals_ShouldReturn_UsersThatAgeNotEqualsToValue(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}!={value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age != value);
	}
}