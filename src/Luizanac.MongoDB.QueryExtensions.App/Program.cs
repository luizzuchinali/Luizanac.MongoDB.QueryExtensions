using Luizanac.MongoDB.QueryExtensions.Extensions;

var client = new MongoClient("mongodb://root:root@localhost:27017/");
var collection = client.GetDatabase("luizanac-query-extensions").GetCollection<User>("Users");

var serializerOptions = new JsonSerializerOptions
{
		WriteIndented = true,
		PropertyNamingPolicy =
				JsonNamingPolicy.CamelCase
};

if (collection.CountDocuments(FilterDefinition<User>.Empty) == 0)
	collection.Seed();
var watcher = new Stopwatch();
//api/clients?sort=asc,name&filter=name@=lui,email!@=@gmail.com,age!=20,age>19"

//?sort=asc,name&filters=age>=16,name@=leffler,name_=h

// >, <, >=, <=, ==, !=  Comparison operators
// @= Contains / !@= NotContains = generate Like/ILike %value%
// _= StartsWith / !_= NotStartsWith = generate Like/ILike value%
//var filters = "age>=18,email@=hotmail.com|gmail.com,name|email_=l,address.city_=lake,address.number==199";
const string filters = "name_=aaron,address.city==Hillsstad";

//SORT
//?sort=adress.number,asc will orderBy address.number ascending
//?sort=address.number,asc|name,asc will orderBy address.number ascending then by name ascending
// | is "then by"
const string sort = "name,asc";
watcher.Start();

var data = await collection.Query().Filter(filters).OrderBy(sort).ToListAsync();

watcher.Stop();
WriteLine(JsonSerializer.Serialize(data, serializerOptions));
WriteLine($"elapsed {watcher.ElapsedMilliseconds}ms");