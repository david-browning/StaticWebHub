using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace StaticActivityHub.Api.Functions;

public class TestFunction
{
   public TestFunction(ILogger<TestFunction> logger)
   {
      _logger = logger;
   }

   [Function("TestFunction")]
   public IActionResult Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "test")]
      HttpRequest req)
   {
      _logger.LogInformation("C# HTTP trigger function processed a request.");
      return new OkObjectResult("Welcome to Azure Functions!");
   }

   private readonly ILogger<TestFunction> _logger;
}