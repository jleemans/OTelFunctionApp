using System.Diagnostics.Metrics;

namespace OTelFunctionApp;

public static class Meters
{
    public const string Name = "OTelFunctionApp.Metrics";
    
    public static readonly Meter AppMeter = new(Name, "1.0.0");
}