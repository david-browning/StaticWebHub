using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace StaticActivityHub.Api.Functions;

public sealed class JsonRequestFunction
{
   public JsonRequestFunction(ILogger<JsonRequestFunction> logger)
   {
      _logger = logger;
   }

   [Function("JsonRequest")]
   public async Task<HttpResponseData> RunAsync(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "jsonRequest")]
      HttpRequestData request)
   {
      var document = await JsonDocument.ParseAsync(request.Body);
      var received = document.RootElement.Clone();
      var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
      await response.WriteAsJsonAsync(
         new
         {
            accepted = true,
            message = "Received and processed!",
            received,
         });

      return response;
   }

   private readonly ILogger<JsonRequestFunction> _logger;
}
