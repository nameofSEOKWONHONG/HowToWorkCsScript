using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

public class Script : ICalc {
    public async Task<int> Sum(int a, int b) {
        var sw = new Stopwatch();
        Console.WriteLine("Hello, World!");

        var j = string.Join(",", new[] { "A", "B", "C", "D", });
        Console.WriteLine(j);

        var client = new HttpClient();
        HttpResponseMessage res = await client.GetAsync("https://www.naver.com");

        foreach(var header in res.Headers)
        {
            Console.WriteLine(header.Key);
            Console.WriteLine(string.Join(',', header.Value));
        }
        return 2;
    }
}