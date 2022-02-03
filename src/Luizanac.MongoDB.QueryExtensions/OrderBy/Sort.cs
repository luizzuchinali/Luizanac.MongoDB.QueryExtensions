namespace Luizanac.MongoDB.QueryExtensions.OrderBy;

public class Sort
{
	private const string Asc = "asc";
	private const string Desc = "desc";
	public ESortType SortType { get; }
	public LambdaExpression LambdaExpression { get; }

	public Sort(string sortType, LambdaExpression lambdaExpression)
	{
		if (!(sortType.Contains(Asc) || sortType.Contains(Desc)))
			throw new ArgumentException("sortType can't be different of asc or desc", nameof(sortType));
		SortType = sortType == Asc ? ESortType.Asc : ESortType.Desc;
		LambdaExpression = lambdaExpression;
	}
}