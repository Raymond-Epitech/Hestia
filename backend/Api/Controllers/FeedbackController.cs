using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Input;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController(IFeedbackService feedbackServic) : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<FeedbackOutput>>> GetAllFeedback()
        {
            return Ok(await feedbackServic.GetAllFeedbacks());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Guid>> SendFeedback(FeedbackInput input)
        {
            return Ok(await feedbackServic.AddFeedback(input));
        }
    }
}