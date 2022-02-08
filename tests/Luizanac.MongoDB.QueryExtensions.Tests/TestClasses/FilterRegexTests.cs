using System.Text.RegularExpressions;

namespace Luizanac.MongoDB.QueryExtensions.Tests.TestClasses;

public class FilterRegexTests : BaseTest
{
	public FilterRegexTests(TestDbContext dbContext) : base(dbContext)
	{
	}

	[Fact]
	public async Task Regex_ShouldReturn_AListThatContainsOnlyUsersThatMatchPattern()
	{
		//Arrange
		var user = await DbContext.Users.GetFirst();
		var regexValue = user.Name[..3];
		var pattern = $"^{regexValue}";
		var filter = $"{nameof(User.Name)}$={pattern}";

		//Act
		var users = await DbContext.Users.Query().Filter(filter).ToListAsync();

		//Assert
		users.Should().NotBeEmpty();
		var regex = new Regex($"{pattern}", RegexOptions.IgnoreCase);
		users.Should().OnlyContain(x => regex.IsMatch(x.Name));
	}
}