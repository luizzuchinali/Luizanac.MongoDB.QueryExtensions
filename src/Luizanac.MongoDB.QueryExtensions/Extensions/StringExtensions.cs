namespace Luizanac.MongoDB.QueryExtensions.Extensions;

public static class StringExtensions
{
	public static string ToPascalCase(this string str)
	{
		var builder = new StringBuilder();
		char? previous = null;
		foreach (var c in str)
		{
			builder.Append(previous is null or '.' ? char.ToUpper(c) : c);
			previous = c;
		}

		return builder.ToString();
	}

	public static string ToCamelCase(this string str)
	{
		var builder = new StringBuilder();
		char? previous = null;
		foreach (var c in str)
		{
			builder.Append(previous is null or '.' ? char.ToLower(c) : c);
			previous = c;
		}

		return builder.ToString();
	}

	public static string ConvertCase(this string str, ECaseType caseType) => caseType switch
	{
			ECaseType.CamelCase => str.ToCamelCase(),
			ECaseType.PascalCase => str.ToPascalCase(),
			_ => str
	};
	
	public static IEnumerable<string> GetProperties(this string str, char splitSeparator = '.', ECaseType caseType = ECaseType.PascalCase)
	{
		var properties = str.Split(splitSeparator, StringSplitOptions.RemoveEmptyEntries);
		return properties.Select(x => x.ConvertCase(caseType));
	}
}