﻿using FloppyBot.WebApi.Base.Exceptions;
using FloppyBot.WebApi.V1Compatibility.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FloppyBot.WebApi.V1Compatibility.Controllers.v1;

[ApiController]
[Route("api/v1/health/bots")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IReadOnlyDictionary<string, HealthCheckData> GetHealthCheck()
    {
        throw this.NotImplemented();
    }

    [HttpDelete("{hostName}/{pid}")]
    public IActionResult RestartInstance(
        [FromRoute] string hostName,
        [FromRoute] int pid)
    {
        throw this.NotImplemented();
    }
}
