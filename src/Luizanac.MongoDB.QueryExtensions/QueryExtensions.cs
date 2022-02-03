namespace Luizanac.MongoDB.QueryExtensions;

public static class QueryExtensions
{
	/// <summary>
	/// Create a query from <see cref="IMongoCollection{TDocument}"/>
	/// </summary>
	/// <param name="collection"><see cref="IMongoCollection{TDocument}"/></param>
	/// <param name="caseType"><see cref="ECaseType"/></param>
	/// <typeparam name="T">Entity type</typeparam>
	/// <returns>The query</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static Query<T> Query<T>(this IMongoCollection<T> collection, ECaseType caseType = ECaseType.PascalCase)
			where T : class
	{
		_ = collection ?? throw new ArgumentNullException(nameof(collection), "collection can't be null");
		return new Query<T>(collection, caseType);
	}

	/// <summary>
	/// Create a query from <see cref="IMongoCollection{TDocument}"/>
	/// </summary>
	/// <param name="collection"><see cref="IMongoCollection{TDocument}"/></param>
	/// <param name="options"><see cref="FindOptions{TDocument}"/></param>
	/// <param name="caseType"><see cref="ECaseType"/></param>
	/// <typeparam name="T">Eentity type</typeparam>
	/// <returns>The query</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static Query<T> Query<T>(this IMongoCollection<T> collection, FindOptions<T> options, ECaseType caseType = ECaseType.PascalCase)
			where T : class
	{
		_ = collection ?? throw new ArgumentNullException(nameof(collection), "collection can't be null");
		return new Query<T>(collection, options, caseType);
	}

	/// <summary>
	/// Fetch data from your <see cref="Query{T}"/>
	/// </summary>
	/// <param name="query"><see cref="Query{T}"/></param>
	/// <typeparam name="T">Your entity type</typeparam>
	/// <returns>The fetched data</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static async Task<IEnumerable<T>> ToListAsync<T>(this Query<T> query)
			where T : class
	{
		_ = query ?? throw new ArgumentNullException(nameof(query), "query can't be null");
		return await query.Collection.FindAsync(query.GetFilterDefinition(), query.Options).ToListAsync();
	}

	/// <summary>
	/// Wrapper to call ToListAsync
	/// </summary>
	/// <param name="cursor"><see cref="IAsyncCursor{TDocument}"/></param>
	/// <typeparam name="T">Your entity type</typeparam>
	/// <returns>The fetched data</returns>
	public static async Task<IEnumerable<T>> ToListAsync<T>(this Task<IAsyncCursor<T>> cursor) =>
			await (await cursor).ToListAsync();

	// /// <summary>
	// /// 
	// /// </summary>
	// /// <typeparam name="TSource">Entity type</typeparam>
	// /// <param name="query">The source IMongoCollection</param>
	// /// <param name="filters">Filters string with right format</param>
	// /// <returns>The source queryable with filters aplied</returns>
	// public static Query<TSource> Filter<TSource>(this IMongoCollection<TSource> query, string filters)
	// {
	// 	if (string.IsNullOrEmpty(filters) || string.IsNullOrWhiteSpace(filters)) return query;
	//
	// 	foreach (var filter in filters.Split(',', StringSplitOptions.RemoveEmptyEntries))
	// 	{
	// 		if (string.IsNullOrWhiteSpace(filter))
	// 			continue;
	//
	// 		var term = new Term(filter);
	// 		var parameter = Expression.Parameter(typeof(TSource), "x");
	//
	// 		var queryExpression = term.Names.GetQueryExpression(parameter, term);
	//
	// 		var lambdaExpression = Expression.Lambda<Func<TSource, bool>>(queryExpression, parameter);
	//
	// 		query = query.Where(lambdaExpression);
	// 	}
	//
	// 	return query;
	// }

	private static Expression GetQueryExpression(this string[] names, ParameterExpression parameter, Term term) => names.Select(x =>
			{
				var property = GetMemberProperty(x, parameter);
				var propertyType = ((PropertyInfo) property.Member).PropertyType;
				var converter = TypeDescriptor.GetConverter(propertyType);

				var operatorExpressions = term.Values.GetOperatorExpressions(converter, propertyType, property, term);

				return operatorExpressions.Aggregate(Expression.OrElse);
			})
			.Aggregate(Expression.OrElse);

	private static MemberExpression GetMemberProperty(string strProperty, ParameterExpression parameter)
	{
		MemberExpression property = null;
		if (strProperty.Contains('.'))
		{
			var propNames = strProperty.Split(".");
			foreach (var propName in propNames)
			{
				if (property == null)
					property = Expression.PropertyOrField(parameter, propName);
				else
					property = Expression.PropertyOrField(property, propName);
			}
		}
		else
			property = Expression.PropertyOrField(parameter, strProperty);

		return property;
	}

	private static IEnumerable<Expression> GetOperatorExpressions(
			this string[] values,
			TypeConverter converter,
			Type propertyType,
			MemberExpression property,
			Term term) => values.Select(x =>
	{
		var filterValue = converter.ConvertFromInvariantString(x);
		var constantValue = Expression.Constant(filterValue);
		var valueExpression = Expression.Convert(constantValue, propertyType);
		return term.ParsedOperator.GetOperatorExpression(property, valueExpression);
	});

	private static Expression GetOperatorExpression(this EFilterOperator @operator, MemberExpression property, UnaryExpression valueExpression)
	{
		switch (@operator)
		{
			case EFilterOperator.GreaterThan:
				return Expression.GreaterThan(property, valueExpression);
			case EFilterOperator.GreaterThanOrEqualTo:
				return Expression.GreaterThanOrEqual(property, valueExpression);
			case EFilterOperator.LessThan:
				return Expression.LessThan(property, valueExpression);
			case EFilterOperator.LessThanOrEqualTo:
				return Expression.LessThanOrEqual(property, valueExpression);
			case EFilterOperator.Equals:
				return Expression.Equal(property, valueExpression);
			case EFilterOperator.NotEquals:
				return Expression.NotEqual(property, valueExpression);
			case EFilterOperator.StartsWith:
				return GetMethodExpression("StartsWith", typeof(string), property, valueExpression);
			case EFilterOperator.NotStartsWith:
				return Expression.Not(GetMethodExpression("StartsWith", typeof(string), property, valueExpression));
			case EFilterOperator.Contains:
				return GetMethodExpression("Contains", typeof(string), property, valueExpression);
			case EFilterOperator.NotContains:
				return Expression.Not(GetMethodExpression("Contains", typeof(string), property, valueExpression));
			default:
				return Expression.Equal(property, valueExpression);
		}
	}

	private static Expression GetMethodExpression(string methodName, Type methodType, MemberExpression property, UnaryExpression valueExpression)
	{
		var method = methodName.GetMethodInfo(methodType, 1, false);
		return Expression.Call(property, method, valueExpression);
	}

	/// <summary>
	/// Paginates an IQueryable
	/// </summary>
	/// <param name="query">The IQueriable</param>
	/// <param name="page">Currrent page</param>
	/// <param name="size">Number of data to get</param>
	/// <typeparam name="T">Type of the data</typeparam>
	/// <returns></returns>
	public static async Task<IPagination<IEnumerable<T>>> PaginateAsync<T>(this Query<T> query, int page, int size)
			where T : class
	{
		page = page <= 0 ? 1 : page;

		// var entries = query.Skip((page - 1) * size).Take(size);
		// var count = await query.CountAsync();
		// var totalPages = (int) Math.Ceiling(count / (float) size);
		//
		// var firstPage = 1;
		// var lastPage = totalPages;
		// var prevPage = page > firstPage ? page - 1 : firstPage;
		// var nextPage = page < lastPage ? page + 1 : lastPage;
		// return new Pagination<IList<T>>(await entries.ToListAsync(),
		// 		totalPages,
		// 		page,
		// 		size,
		// 		prevPage,
		// 		nextPage,
		// 		count);
		return null;
	}

	// public static HttpContext SetPaginationHeader<T>(this HttpContext httpContext, string route, Pagination<T> pagination){
	//     httpContext.Response.Headers.Add ("X-Total-Count", pagination.TotalPages.ToString());
	//     httpContext.Response.Headers.Add ("Link",
	//         $"<{route}&page={pagination.CurrentPage}>; rel=\"first\", <{route}&page={pagination.NextPage}>; rel=\"next\", <{route}&page={pagination.TotalDataCount}>; rel=\"last\""
	//     );
	//     return httpContext;
	// }
}