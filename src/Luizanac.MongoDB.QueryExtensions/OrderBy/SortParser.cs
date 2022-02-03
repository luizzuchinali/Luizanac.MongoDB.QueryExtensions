namespace Luizanac.MongoDB.QueryExtensions.OrderBy;

/// <summary>
///	Parse a string to a list of <see cref="Sort"/> and create list of <see cref="SortDefinition{TDocument}"/>
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class SortParser<T>
{
	public IList<Sort> Sorters { get; } = new List<Sort>();
	private readonly SortDefinitionBuilder<T> _sortBuilder;

	public SortParser(string sort, SortDefinitionBuilder<T> sortBuilder, ECaseType caseType)
	{
		_ = sort ?? throw new ArgumentNullException(nameof(sort), "sort can't be null");
		var sorts = sort.Split("|");
		foreach (var sortPart in sorts)
		{
			var sorter = sortPart.Split(',');
			var properties = sorter[0].GetProperties('.', caseType);
			var lambda = typeof(T).GetLambdaExpression(properties);
			Sorters.Add(new Sort(sorter[1], lambda));
		}

		_sortBuilder = sortBuilder;
	}

	/// <summary>
	/// Create an array of <see cref="SortDefinition{TDocument}"/> based on list of <see cref="Sort"/>
	/// </summary>
	/// <returns>An <see cref="IEnumerable{T}"/> of <see cref="SortDefinition{TDocument}"/></returns>
	public IEnumerable<SortDefinition<T>> GetSortDefinitions()
	{
		var sortDefinitions =
				from sorter in Sorters
				let expressionField = new ExpressionFieldDefinition<T>(sorter.LambdaExpression)
				select sorter.SortType.Equals(ESortType.Asc)
						? _sortBuilder.Ascending(expressionField)
						: _sortBuilder.Descending(expressionField);

		return sortDefinitions;
	}
}