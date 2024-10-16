#region
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vonage.DeviceStatus;
using Vonage.DeviceStatus.GetConnectivityStatus;
using Vonage.DeviceStatus.GetRoamingStatus;
using Vonage.Extensions;
using Vonage.SimSwap;
using Vonage.SimSwap.Check;
using Vonage.SimSwap.GetSwapDate;
#endregion

var serviceCollection = new ServiceCollection();
var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
serviceCollection.AddSingleton(configuration);
serviceCollection.AddSingleton<IConfiguration>(configuration);
serviceCollection.AddVonageClientScoped(configuration);
await CheckSimSwap(serviceCollection.BuildServiceProvider().GetRequiredService<ISimSwapClient>(), "+990000000000", 24);
await GetSwapDate(serviceCollection.BuildServiceProvider().GetRequiredService<ISimSwapClient>(), "+990000000000");
await GetConnectivityStatus(serviceCollection.BuildServiceProvider().GetRequiredService<IDeviceStatusClient>(), "+990000000000");
await GetRoamingStatus(serviceCollection.BuildServiceProvider().GetRequiredService<IDeviceStatusClient>(), "+990000000000");
return;



static async Task CheckSimSwap(ISimSwapClient client, string phoneNumber, int period)
{
    Console.WriteLine("---------- SimSwap - Check");
    var result = await client.CheckAsync(CheckRequest.Build().WithPhoneNumber(phoneNumber).WithPeriod(period).Create());
    Console.WriteLine(result.Match(
        success => $"Success -- SimSwap: {success}", 
        failure => $"Failure -- {failure.GetFailureMessage()}"));
}

static async Task GetSwapDate(ISimSwapClient client, string phoneNumber)
{
    Console.WriteLine("---------- SimSwap - Swap date");
    var result = await client.GetSwapDateAsync(GetSwapDateRequest.Parse(phoneNumber));
    Console.WriteLine(result.Match(
        success => $"Success -- Swap date: {success}", 
        failure => $"Failure -- {failure.GetFailureMessage()}"));
}

static async Task GetConnectivityStatus(IDeviceStatusClient client, string phoneNumber)
{
    Console.WriteLine("---------- DeviceStatus - Connectivity status");
    var result = await client.GetConnectivityStatusAsync(GetConnectivityStatusRequest.Build().WithPhoneNumber(phoneNumber).Create());
    Console.WriteLine(result.Match(
        success => $"Success -- Connectivity status: {success.Status}", 
        failure => $"Failure -- {failure.GetFailureMessage()}"));
}

static async Task GetRoamingStatus(IDeviceStatusClient client, string phoneNumber)
{
    Console.WriteLine("---------- DeviceStatus - Roaming status");
    var result = await client.GetRoamingStatusAsync(GetRoamingStatusRequest.Build().WithPhoneNumber(phoneNumber).Create());
    Console.WriteLine(result.Match(
        success => $"Success -- Roaming: {success.IsRoaming}, Country: {string.Join(", ", success.CountryName)}, CountryCode: {success.CountryCode}", 
        failure => $"Failure -- {failure.GetFailureMessage()}"));
}
