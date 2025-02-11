using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySurveys.Server.Interfaces.Services;
using System.Security.Claims;

namespace MySurveys.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnswerController : ControllerBase
{
    private readonly IAnswersService answerService;
    public AnswerController(IAnswersService answerService)
    {
        this.answerService = answerService;
    }
    [HttpPost("{surveyId:int}")]
    [ProducesResponseType<int>(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AddAnswer([FromBody] string answer, [FromRoute] int surveyId)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        int? result = await answerService.AddAnswers(surveyId, answer, userName);
        if(result is not null)
            return Ok(result);
        return BadRequest();
    }
    [HttpGet("{surveyId:int}")]
    [Authorize]
    [ProducesResponseType<string>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetAnswer([FromRoute] int surveyId)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if(userName is null) 
            return Unauthorized();
        string? result = await answerService.GetAnswer(surveyId, userName);
        if (result is not null)
            return Ok(result);
        return BadRequest();
    }
    [HttpGet("{surveyId:int}/allAnswers")]
    [Authorize]
    [ProducesResponseType<string[]>(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetAllAnswers([FromRoute] int surveyId)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized();
        string[] results = await answerService.GetAnswers(surveyId, userName);
        if(results.Length == 0)
            return BadRequest();
        return Ok(results);
    }
    [HttpPut("{surveyId:int}")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UpdateAnswer([FromRoute] int surveyId, [FromBody] string answer)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized();
        bool? result = await answerService.UpdateAnswer(surveyId, userName, answer);
        if (result is null)
            return StatusCode(StatusCodes.Status500InternalServerError);
        if (!result.Value)
            return BadRequest();
        return Ok();
    }
    [HttpDelete("{surveyId:int}")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteAnswer([FromRoute] int surveyId)
    {
        string? userName = User.FindFirstValue(ClaimTypes.Name);
        if (userName is null)
            return Unauthorized();
        bool? result = await answerService.RemoveAnswer(surveyId, userName);
        if (result is null)
            return StatusCode(StatusCodes.Status500InternalServerError);
        if (!result.Value)
            return BadRequest();
        return Ok();
    }
}