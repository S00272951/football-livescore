using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

internal class Program
{
    static async Task Main(string[] args)
    {
        var client = new HttpClient();

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://livescore6.p.rapidapi.com/competitions/get-table?CompId=65"),
        };

        request.Headers.Add("x-rapidapi-key", "6c976f25eamsh741f773fbd19b37p1");
        request.Headers.Add("x-rapidapi-host", "livescore6.p.rapidapi.com");

        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            Console.WriteLine(body);
            Console.ReadLine();
        }
    }
}
