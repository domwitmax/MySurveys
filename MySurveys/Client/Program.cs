using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MySurveys.Client;
using MySurveys.Client.Interface.Service;
using MySurveys.Client.Services;
using Radzen;
using System.Collections;
using System.Collections.Generic;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("MySurveys.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MySurveys.ServerAPI"));
builder.Services.AddScoped<ISurveysService, SurveysService>();

builder.Services.AddRadzenComponents();

await builder.Build().RunAsync();
