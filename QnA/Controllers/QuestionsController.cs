using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QandA.Data;
using QandA.Data.Models;
using QandA.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace QandA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository dataRepository;

        private readonly IHubContext<QuestionsHub> questionHubContext;

        private readonly IHttpClientFactory clientFactory;
        private readonly string auth0UserInfo;

        public QuestionsController(
            IDataRepository dataRepository,
            IHubContext<QuestionsHub> questionHubContext,
            IHttpClientFactory clientFactory,
            IConfiguration configuration
        )
        {
            this.dataRepository = dataRepository;
            this.questionHubContext = questionHubContext;
            this.clientFactory = clientFactory;
            this.auth0UserInfo = $"{configuration["Auth0:Authority"]}userinfo";
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search, bool includeAnswers, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(search))
            {
                if (includeAnswers)
                {
                    return this.dataRepository.GetQuestionsWithAnswers();
                }
                else
                {
                    return this.dataRepository.GetQuestions();
                }
            }
            else
            {
                return this.dataRepository.GetQuestionsBySearchWithPaging(search, page, pageSize);
            }
        }

        [HttpGet("unanswered")]
        public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestions()
        {
            return await this.dataRepository.GetUnansweredQuestionsAsync();
        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            var question = this.dataRepository.GetQuestion(questionId);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<QuestionGetSingleResponse>> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            var savedQuestion = this.dataRepository.PostQuestion(new QuestionPostFullRequest
            {
                Title = questionPostRequest.Title,
                Content = questionPostRequest.Content,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName = await GetUserName(),
                Created = DateTime.UtcNow
            });

            return CreatedAtAction(
                nameof(GetQuestion),
                new { questionId = savedQuestion.QuestionId },
                savedQuestion);
        }


        [Authorize(Policy = "MustBeQuestionAuthor")]
        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
        {
            var question = this.dataRepository.GetQuestion(questionId);

            if (question == null)
            {
                return NotFound();
            }

            questionPutRequest.Title =
                    string.IsNullOrEmpty(questionPutRequest.Title) ?
                    question.Title :
                    questionPutRequest.Title;

            questionPutRequest.Content =
                    string.IsNullOrEmpty(questionPutRequest.Content) ?
                    question.Content :
                    questionPutRequest.Content;

            var savedQuestion = this.dataRepository.PutQuestion(questionId, questionPutRequest);
            return savedQuestion;
        }

        [Authorize(Policy = "MustBeQuestionAuthor")]
        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = this.dataRepository.GetQuestion(questionId);

            if (question == null)
            {
                return NotFound();
            }

            this.dataRepository.DeleteQuestion(questionId);
            return NoContent();
        }

        [Authorize]
        [HttpPost("answer")]
        public async Task<ActionResult<AnswerGetResponse>> PostAnswer(AnswerPostRequest answerPostRequest)
        {
            var questionExists = this.dataRepository.QuestionExists(answerPostRequest.QuestionId.Value);

            if (!questionExists)
            {
                return NotFound();
            }

            var savedAnswer = await this.dataRepository.PostAnswerAsync(new AnswerPostFullRequest
            {
                QuestionId = answerPostRequest.QuestionId.Value,
                Content = answerPostRequest.Content,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName = await GetUserName(),
                Created = DateTime.UtcNow
            });

            await this.questionHubContext
                .Clients
                .Group($"Question-{answerPostRequest.QuestionId.Value}")
                .SendAsync("ReceiveQuestion", this.dataRepository.GetQuestion(answerPostRequest.QuestionId.Value));

            return savedAnswer;
        }

        private async Task<string> GetUserName()
        {
            var request = new HttpRequestMessage(
              HttpMethod.Get,
              this.auth0UserInfo);
            request.Headers.Add(
              "Authorization",
              Request.Headers["Authorization"].First());
            var client = this.clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent =
                  await response.Content.ReadAsStringAsync();
                var user =
                  JsonSerializer.Deserialize<User>(
                    jsonContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                return user.Name;
            }
            else
            {
                return "";
            }
        }
    }
}
