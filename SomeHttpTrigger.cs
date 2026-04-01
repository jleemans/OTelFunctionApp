using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Http;

namespace OTelFunctionApp;

/// <summary>
/// Represents an Azure Function HTTP trigger for handling incoming HTTP requests.
/// This class is designed to log, trace, and collect metrics for observability purposes
/// using OpenTelemetry components such as counters, histograms, and activity sources.
/// </summary>
public class SomeHttpTrigger(ILogger<SomeHttpTrigger> logger)
{
    private static readonly ActivitySource ActivitySource = new("OTelFunctionApp");

    private readonly Counter<int> _counter = Meters.AppMeter.CreateCounter<int>("incoming_requests_total");
    private readonly Histogram<double> _histogram = Meters.AppMeter.CreateHistogram<double>("incoming_requests_duration_ms");


    [Function("SomeHttpTrigger")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
    {
        // logging
        logger.LogInformation("Incoming request at {Path} with query {Query}",
            req.Url.AbsolutePath,
            req.Url.Query);

        // tracing
        var stopWatch = Stopwatch.StartNew();
        var spanName = "SomeHttpTrigger.ExpensiveOperation";
        using (var activity = ActivitySource.StartActivity(spanName))
        {
            activity?.SetTag("request.method", req.Method);
            activity?.SetTag("request.url", req.Url.ToString());

            await Task.Delay(100 + Random.Shared.Next(100));
        }
        stopWatch.Stop();    
        
        // metrics
        var elapsedMillis = stopWatch.Elapsed.TotalMilliseconds;
        
        _histogram.Record(elapsedMillis);
        _counter.Add(1);
        
        logger.LogInformation("Completed request in {ElapsedMillis} ms", elapsedMillis);
        
        var res = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await res.WriteStringAsync("Request well received. OTel data sent.");
        return res;
    }
}
