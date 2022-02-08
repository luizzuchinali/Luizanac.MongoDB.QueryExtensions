namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class FilterGreaterThanTests : BaseTest
{
	public FilterGreaterThanTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Theory]
	[InlineData(20)]
	[InlineData(10)]
	[InlineData(0)]
	public async Task GreaterThan_ShouldReturn_OnlyGreaterThanValue(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}>{value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age > value);
	}

	[Theory]
	[InlineData(20)]
	[InlineData(10)]
	[InlineData(0)]
	public async Task GreaterThanOrEqualTo_ShouldReturn_OnlyGreaterThanOrEqualToValue(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}>={value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age >= value);
	}

	[Theory]
	[InlineData(20)]
	[InlineData(10)]
	[InlineData(4)]
	public async Task LessThan_ShouldReturn_OnlyLessThanValue(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}<{value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age < value);
	}

	[Theory]
	[InlineData(20)]
	[InlineData(10)]
	[InlineData(0)]
	public async Task LessThanOrEqualTo_ShouldReturn_OnlyLessThanOrEqualToValue(int value)
	{
		//Arrange
		var filter = $"{nameof(User.Age)}<={value}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().OnlyContain(x => x.Age <= value);
	}

	[Fact]
	public async Task LessThanZero_ShouldBe_EmptyList()
	{
		//Arrange
		const string filter = $"{nameof(User.Age)}<0";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().BeEmpty();
		users.Should().NotContain(x => x.Age > 0);
	}
}