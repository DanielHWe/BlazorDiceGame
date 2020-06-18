using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazored.Modal;
using DiceGame.Model;
using Blazored.Toast;
using DiceGame.Services;

namespace DiceGame
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredModal();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<GameService>();
            builder.Services.AddSingleton<NotificationService>();

            await builder.Build().RunAsync();
        }
    }
}
