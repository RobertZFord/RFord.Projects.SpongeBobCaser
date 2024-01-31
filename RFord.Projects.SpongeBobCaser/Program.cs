using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace RFord.Projects.SpongeBobCaser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet(
                pattern: "/{key}",
                handler: (string key) => Enumerable.Aggregate(
                    source: key,
                    seed: (new StringBuilder(), false),
                    func: (acc, c) => (acc.Item1.Append(char.IsLetter(c) ? ((acc.Item2 = !acc.Item2) ? char.ToUpper(c) : char.ToLower(c)) : c), acc.Item2),
                    resultSelector: acc => acc.Item1.ToString()
                )
            );

            app.Run();
        }
    }
}
