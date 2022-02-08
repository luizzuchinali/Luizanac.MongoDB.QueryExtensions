namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class OrderByTests : BaseTest
{
	public OrderByTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Fact]
	public async Task SimpleOrderBy_ShouldReturn_ASortedList()
	{
		//Arrange
		var sortedUsers = (await DbContext.Users.GetAll()).OrderByDescending(x => x.Age).ToList();
		var sortString = $"{nameof(User.Age)},desc";

		//Act
		var users = await DbContext.Users.Query().OrderBy(sortString).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().Equal(sortedUsers, (src, dest) => src.Id == dest.Id);
	}

	[Fact]
	public async Task MultipleOrderBy_ShouldReturn_ASortedList()
	{
		//Arrange
		var sortedUsers =
				(await DbContext.Users.GetAll())
				.OrderByDescending(x => x.Age)
				.ThenBy(x => x.Address.State)
				.ToList();
		var sortString = $"{nameof(User.Age)},desc|{nameof(User.Address)}.{nameof(Address.State)},asc";

		//Act
		var users = await DbContext.Users.Query().OrderBy(sortString).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		users.Should().Equal(sortedUsers, (src, dest) => src.Id == dest.Id);
	}
}