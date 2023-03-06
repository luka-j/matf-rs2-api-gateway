using Microsoft.AspNetCore.Mvc;

namespace ApiGatewayApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloWorldController : ControllerBase
{
    private readonly ILogger<HelloWorldController> _logger;

    public HelloWorldController(ILogger<HelloWorldController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "HelloWorld")]
    public IEnumerable<string> Get()
    {
        return new string[] { "Hello", "World" };
    }
}