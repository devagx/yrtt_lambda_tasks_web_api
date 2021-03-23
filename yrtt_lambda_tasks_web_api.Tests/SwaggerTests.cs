using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace yrtt_lambda_tasks_web_api.Tests
{
    public class SwaggerTests
    {
        [Fact]
        public async Task TestSwaggerLoad()
        {
            var lambdaFunction = new LambdaEntryPoint();

            var requestStr = File.ReadAllText("./TestRequests/Swagger-Get.json");
            var request = JsonConvert.DeserializeObject<APIGatewayProxyRequest>(requestStr);
            var context = new TestLambdaContext();
            var response = await lambdaFunction.FunctionHandlerAsync(request, context);

            Assert.Equal(200, response.StatusCode);
            Assert.True(response.Body.Length > 0);
            //Assert.Equal("application/json", response.Headers["Content-Type"]);
        }
    }
}
