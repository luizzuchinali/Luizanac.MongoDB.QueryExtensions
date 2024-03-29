﻿namespace Luizanac.MongoDB.QueryExtensions.Shared.Seeds;

public static class UserSeed
{
	public static void Seed(this IMongoCollection<User> collection)
	{
		var aux = 0;
		const int size = 100;
		var users = new List<User>(size);
		while (aux != size)
		{
			var person = new Bogus.Person();
			var bAddress = new Bogus.Faker().Address;
			var address = new Address(bAddress.City(),
					bAddress.StreetName(),
					bAddress.BuildingNumber(),
					bAddress.State());
			var user = new User(person.FullName, person.Email, aux, address);
			users.Add(user);
			aux++;
		}

		collection.InsertMany(users);
	}
}