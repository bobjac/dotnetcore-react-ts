using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Dapper;
using QandA.Data.Models;

namespace QandA.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string connectionString;

        public DataRepository(IConfiguration configuration)
        {
            this.connectionString = configuration["ConnectionStrings:DetaultConnection"];
        }

        public AnswerGetResponse GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.QueryFirstOrDefault<AnswerGetResponse> (
                    @"EXEC dbo.Answer_Get_ByAnswerId @AnswerId = @AnswerId",
                    new { AnswerId = answerId }
                );
            }
        }

        public QuestionGetSingleResponse GetQuestion(int questionId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var question = connection.QueryFirstOrDefault<QuestionGetSingleResponse> (
                    @"EXEC dbo.Question_GetSingle @QuestionId = @QuestionId",
                    new { QuestionId = questionId}
                );
                
                if (question != null)
                {
                    question.Answers = connection.Query<AnswerGetResponse> (
                        @"EXEC dbo.Answer_Get_BuQuestionId",
                        new { QuestionId = questionId }
                    );
                }
                return question;
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestions()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany"
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetQuestionsBySearch(string search)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.Query<QuestionGetManyResponse>(
                    @"EXEC dbo.Question_GetMany_BySearch @Search = @Search",
                    new { Search = search }
                );
            }
        }

        public bool QuestionExist(int questionId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return connection.QueryFirst<bool> (
                    @"EXEC dbo.Question_Exists @QuestionId = @QuestionId",
                    new { QuestionId = questionId }
                );
            }
        }

        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<QuestionGetManyResponse> (
                    "EXEC dbo.Question_GetUnanswered"
                );
            }
        }
    }
}