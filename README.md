# About

This is a sample function app that uses OpenTelemetry to feed a 
locally running Signoz instance.

# Prequisites

Make sure you have a locally running Signoz instance. You can
easily do this as follows:

1. `git clone https://github.com/SigNoz/signoz.git`
2. `cd signoz/deploy/docker`
3. `docker compose up -d`
4. When all containers are up and running, you can access 
  [the Signoz UI](http://localhost:8080)

# Run the function app

1. Run the function app (e.g. `func start`)
2. Send some HTTP requests to the function app on this endpoint:
   `http://localhost:7071/api/SomeHttpTrigger`
3. Check the Signoz UI to see the traces:
  3.1 Go to [the Signoz UI](http://localhost:8080)
  3.2 Click on the `traces` tab