using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using QandA.Data;
using QandA.Data.Models;

namespace QandA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository dataRepository;

        public QuestionsController(IDataRepository dataRepository)
        {
            this.dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return this.dataRepository.GetQuestions();
            }
            else
            {
                return this.dataRepository.GetQuestionsBySearch(search);
            }
        }

        [HttpGet("unanswered")]
        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            return this.dataRepository.GetUnansweredQuestions();
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

        [HttpPost]
        public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            var savedQuestion = this.dataRepository.PostQuestion(questionPostRequest);

            return CreatedAtAction(
                nameof(GetQuestion),
                new { questionId = savedQuestion.QuestionId },
                savedQuestion);
        }
    }
}
