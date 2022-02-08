using System.Collections.Generic;

namespace Luizanac.MongoDB.QueryExtensions.Tests;

public static class TestDbContextExtensions
{
	public static async Task<IList<T>> GetAll<T>(this IMongoCollection<T> collection) =>
			await collection.FindAsync(new FilterDefinitionBuilder<T>().Empty).ToListAsync();

	public static async Task<T> GetFirst<T>(this IMongoCollection<T> collection) =>
			(await collection.FindAsync(FilterDefinition<T>.Empty, new FindOptions<T> {Limit = 1})
					.ToListAsync())
			.First();
}