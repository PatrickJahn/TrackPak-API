using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Monitoring;

public static class LoggingService 
{
    private static readonly string ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
    public static ActivitySource activitySource = new ActivitySource(ServiceName);
    public static TracerProvider tracerProvider;

    public static ILogger Log => Serilog.Log.Logger;
    static LoggingService()
    {
        Console.WriteLine(ServiceName);
        Console.WriteLine(activitySource.Name);

        // Open telemetry config
        tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetSampler(new AlwaysOnSampler()) // ✅ this forces all spans to be collected
            .AddSource(activitySource.Name)
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(activitySource.Name))
            .AddZipkinExporter(options =>
            {
                options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
            })
            .Build();
        
        // Serilog config
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:5341") // Seq running on this address
            .Enrich.FromLogContext()
            .CreateLogger();
        
    }


  
}

