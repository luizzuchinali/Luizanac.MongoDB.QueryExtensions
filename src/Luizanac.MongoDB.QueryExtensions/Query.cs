namespace Luizanac.MongoDB.QueryExtensions;

public class Query<T> where T : class
{
	public IMongoCollection<T> Collection { get; }
	public FindOptions<T> Options { get; }
	public ECaseType CaseType { get; }

	private readonly IList<FilterDefinition<T>> _filterDefinitions;

	private FilterDefinitionBuilder<T> _filterBuilder;
	protected FilterDefinitionBuilder<T> FilterBuilder => _filterBuilder ??= new FilterDefinitionBuilder<T>();

	private SortDefinitionBuilder<T> _sortBuilder;
	protected SortDefinitionBuilder<T> SortBuilder => _sortBuilder ??= new SortDefinitionBuilder<T>();

	public Query(IMongoCollection<T> collection, ECaseType caseType = ECaseType.PascalCase)
	{
		Collection = collection;
		CaseType = caseType;
		_filterDefinitions = new List<FilterDefinition<T>>();
		Options = new()
		{
				Limit = 10
		};
	}

	public Query(IMongoCollection<T> collection, FindOptions<T> options, ECaseType caseType = ECaseType.PascalCase)
			: this(collection, caseType)
	{
		Options = options;
	}

	public FilterDefinition<T> GetFilterDefinition() =>
			_filterDefinitions.Count > 0 ? new FilterDefinitionBuilder<T>().And(_filterDefinitions) : FilterDefinition<T>.Empty;

	/// <summary>
	/// Add sort capability to <see cref="Query{T}"/> instance
	/// </summary>
	/// <param name="sort">the sort string property,asc/desc</param>
	/// <returns>The current instance of <see cref="Query{T}"/></returns>
	public Query<T> OrderBy(string sort)
	{
		if (string.IsNullOrEmpty(sort) || string.IsNullOrWhiteSpace(sort)) return this;

		var parser = new SortParser<T>(sort, SortBuilder, CaseType);
		Options.Sort = SortBuilder.Combine(parser.GetSortDefinitions());
		return this;
	}
}