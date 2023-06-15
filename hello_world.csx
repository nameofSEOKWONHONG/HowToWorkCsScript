#r "System.Net.Http.dll"
#r "System.Linq.dll"
#r "System.Collections.dll"
#r "System.dll"
#r "System.Runtime.dll"

using System.Net.Http;
using System.Linq;
using System.Collections;
using System;
using System.Diagnostics;

Console.WriteLine("hello dotnet-script");

for(var i = 0; i < 10; i++) {
    for(var j = 0; j < 10; j++ ) {
        Console.Write("*");
    }
    Console.WriteLine();
}

var sw = new System.Diagnostics.Stopwatch();
sw.Start();
var client = new HttpClient();
HttpResponseMessage res = await client.GetAsync("https://www.naver.com");
sw.Stop();
foreach(var header in res.Headers)
{
    Console.WriteLine(header.Key);
    Console.WriteLine(string.Join(',', header.Value));
}
Console.WriteLine(sw.Elapsed.TotalSeconds);
