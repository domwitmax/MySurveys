using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySurveys.Server.Interfaces.Services;
using MySurveys.Shared.Models.Questions;
using MySurveys.Shared.Models.Responses;
using System.Security.Claims;

namespace MySurveys.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService surveyService;
        public SurveyController(ISurveyService surveyService)
        {
            this.surveyService = surveyService;
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType<int?>(200)]
        [ProducesResponseType<int?>(400)]
        [ProducesResponseType<int?>(401)]
        public async Task<IActionResult> AddSurvey(Survey survey)
        {
            string? userName = User.FindFirstValue(ClaimTypes.Name);
            if (userName is null)
                return Unauthorized(null);
            if (!surveyService.CheckSurvey(survey))
                return BadRequest((int?)null);
            int? result = await surveyService.AddSurvey(survey, userName);
            if (result is null)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet]
        [ProducesResponseType<Survey>(200)]
        [ProducesResponseType<Survey>(400)]
        public IActionResult GetSurvey(int surveyId)
        {
            Survey? result = surveyService.GetSurvey(surveyId);
            if (result is null)
                return BadRequest((Survey?)null);
            if (!surveyService.CheckSurvey(result))
                return BadRequest((Survey?)null);
            return Ok(result);
        }
    }
}
