using ClinicaVistaalegre.Client;
using ClinicaVistaalegre.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ClinicaVistaalegre.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ClinicaVistaalegre.ServerAPI"));

builder.Services.AddAuthorizationCore();
builder.Services.AddApiAuthorization();

builder.Services.TryAddEnumerable(
    ServiceDescriptor.Singleton<
        IPostConfigureOptions<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>>,
        ApiAuthorizationOptionsConfiguration>());

await builder.Build().RunAsync();
