namespace Luizanac.MongoDB.QueryExtensions;

public static class ReflectionExtensions
{
	public static Type GetPropertyType(this Type entityType, string[] properties)
	{
		if (properties.Length > 1)
			return entityType.GetProperty(properties[0])?.PropertyType.GetProperty(properties[1])
					?.PropertyType;

		return entityType.GetProperty(properties[0])?.PropertyType;
	}



	public static MethodInfo GetMethodInfo(this string methodName, Type type, int parametersCount, bool genericMethodDefinition = true)
	{
		var method = type.GetMethods()
				.Where(m => m.Name == methodName && m.IsGenericMethodDefinition == genericMethodDefinition)
				.Where(m =>
				{
					var parameters = m.GetParameters().ToList();
					return parameters.Count == parametersCount;
				}).First();

		return method;
	}

	public static string[] GetProperties(this string str, char splitSeparator = '.', ECaseType caseType = ECaseType.PascalCase)
	{
		var properties = str.Split(splitSeparator, StringSplitOptions.RemoveEmptyEntries);
		return properties.Select(x => x.ConvertCase(caseType)).ToArray();
	}
}