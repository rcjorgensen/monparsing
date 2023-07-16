using MonParsing.Examples.JsonParser;

var testJson = """
   {
     "foo"   :    "bar"   ,   
     "hello": {
        "world": true
     },
     "stuff":  [  "something" ,   true, null]
}   
""";

var parseResult = Json.Parser(testJson).First();

var propertyCount = parseResult.Result.Value.Count();

Console.WriteLine(
    $"Parsing was successful. A JSON object with {propertyCount} properties was parsed."
);
