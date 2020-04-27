using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QandA.Data.Models;

namespace QandA.Data
{
    public interface IDataRepository
    {
        IEnumerable<QuestionGetManyResponse> GetQuestions();

        IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions();

        IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search);

        QuestionGetSingleResponse GetQuestion(int questionId);

        bool QuestionExist(int questionId);

        AnswerGetResponse GetAnswer(int answerId);
    }
}