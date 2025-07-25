// Program.cs

using StaticBlaze.Constants;
using StaticBlaze.DbGenerator;

var seed = args.Contains("--seed");

var builder = new DbBuilder(StaticBlazeConfig.BlogDB, Directory.GetCurrentDirectory(), seed);

await builder.RunAsync();
Console.WriteLine("Database Generated successfully.");

return 0;