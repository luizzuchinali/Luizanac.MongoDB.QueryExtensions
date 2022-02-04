namespace Luizanac.MongoDB.QueryExtensions.Filter;

public class FilterParser<T> where T : class
{
	protected readonly IEnumerable<Term> Terms;
	protected readonly ECaseType CaseType;
	protected readonly FilterDefinitionBuilder<T> FilterBuilder;

	public FilterParser(
			string filters,
			FilterDefinitionBuilder<T> filterBuilder,
			ECaseType caseType = ECaseType.PascalCase)
	{
		if (string.IsNullOrEmpty(filters) || string.IsNullOrWhiteSpace(filters))
			throw new ArgumentException("invalid argument", nameof(filters));

		Terms = from filter in filters.Split(',', StringSplitOptions.RemoveEmptyEntries)
		        where !string.IsNullOrWhiteSpace(filter)
		        select new Term(filter);

		CaseType = caseType;
		FilterBuilder = filterBuilder;
	}

	public IEnumerable<FilterDefinition<T>> GetFilterDefinitions() => Terms.Select(HandleTerm);

	private FilterDefinition<T> HandleTerm(Term term)
	{
		var filterDefinitions = new List<FilterDefinition<T>>();
		foreach (var name in term.Names)
		{
			var propertyName = name.ConvertCase(CaseType);
			if (!HasProperty(propertyName))
				continue;

			var valueDefinitions =
					term.Values.Select(x => GetFilterDefinitionFromOperator(term.ParsedOperator, propertyName, x));
			filterDefinitions.Add(FilterBuilder.Or(valueDefinitions));
		}

		return FilterBuilder.Or(filterDefinitions);
	}

	protected bool HasProperty(string property)
	{
		var type = typeof(T);
		if (!property.Contains('.')) return type.GetProperty(property) != null;

		var props = property.Split('.');
		foreach (var prop in props)
		{
			var propertyInfo = type.GetProperty(prop);
			if (propertyInfo == null)
				return false;

			type = propertyInfo.PropertyType;
		}

		return true;
	}

	protected virtual FilterDefinition<T> GetFilterDefinitionFromOperator(
			EFilterOperator @operator,
			string property,
			string value) =>
			@operator switch
			{
					EFilterOperator.GreaterThan =>
							FilterBuilder.Gt(property, value),
					EFilterOperator.GreaterThanOrEqualTo =>
							FilterBuilder.Gte(property, value),
					EFilterOperator.LessThan =>
							FilterBuilder.Lt(property, value),
					EFilterOperator.LessThanOrEqualTo =>
							FilterBuilder.Lte(property, value),
					EFilterOperator.Equals =>
							FilterBuilder.Eq(property, value),
					EFilterOperator.NotEquals =>
							FilterBuilder.Ne(property, value),
					EFilterOperator.StartsWith =>
							FilterBuilder.Regex(property, new BsonRegularExpression($"^{value}", "i")),
					EFilterOperator.NotStartsWith =>
							FilterBuilder.Regex(property, new BsonRegularExpression($"^(?!{value})", "i")),
					EFilterOperator.Contains =>
							FilterBuilder.Regex(property, new BsonRegularExpression($"{value}", "i")),
					EFilterOperator.NotContains =>
							FilterBuilder.Regex(property, new BsonRegularExpression($"^((?!{value}).)*$", "i")),
					EFilterOperator.Regex =>
							FilterBuilder.Regex(property, new BsonRegularExpression(value, "i")),
					_ => FilterDefinition<T>.Empty
			};
}