namespace Luizanac.MongoDB.QueryExtensions.Filter;

public class Term
{
	public Term(string filters)
	{
		if (string.IsNullOrWhiteSpace(filters)) throw new ArgumentNullException(nameof(filters));

		var filterSplits = filters.Split(Operators, StringSplitOptions.RemoveEmptyEntries)
				.Select(t => t.Trim()).ToArray();
		Names = Regex.Split(filterSplits[0], SplitPattern).Select(t => t.Trim()).ToArray();
		Values = filterSplits.Length > 1 ? Regex.Split(filterSplits[1], SplitPattern).Select(t => t.Trim()).ToArray() : null;
		Operator = Array.Find(Operators, filters.Contains) ?? "==";
		ParsedOperator = GetOperatorParsed(Operator);
	}

	private const string SplitPattern = @"(?<!($|[^\\])(\\\\)*?\\)\|";

	private static readonly string[] Operators =
	{
			"!@=",
			"@=",
			"!_=",
			"_=",
			"!=",
			"==",
			">=",
			"<=",
			">",
			"<",
			"$="
	};

	public string[] Names { get; }

	public EFilterOperator ParsedOperator { get; }

	public string[] Values { get; }

	public string Operator { get; }

	private static EFilterOperator GetOperatorParsed(string @operator) =>
			@operator switch
			{
					"==" => EFilterOperator.Equals,
					"!=" => EFilterOperator.NotEquals,
					"<" => EFilterOperator.LessThan,
					">" => EFilterOperator.GreaterThan,
					">=" => EFilterOperator.GreaterThanOrEqualTo,
					"<=" => EFilterOperator.LessThanOrEqualTo,
					"@=" => EFilterOperator.Contains,
					"!@=" => EFilterOperator.NotContains,
					"_=" => EFilterOperator.StartsWith,
					"!_=" => EFilterOperator.NotStartsWith,
					"$=" => EFilterOperator.Regex,
					_ => EFilterOperator.Equals
			};
}