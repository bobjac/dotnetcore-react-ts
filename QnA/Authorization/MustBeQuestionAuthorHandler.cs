using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QandA.Data;
namespace QandA.Authorization
{
    public class MustBeQuestionAuthorHandler :
      AuthorizationHandler<MustBeQuestionAuthorRequirement>
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeQuestionAuthorHandler(
             IDataRepository dataRepository,
             IHttpContextAccessor httpContextAccessor)

        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        protected async override Task
             HandleRequirementAsync(
               AuthorizationHandlerContext context,
               MustBeQuestionAuthorRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var questionId = _httpContextAccessor.HttpContext.Request.RouteValues["questionId"];
            int questionIdAsInt = Convert.ToInt32(questionId);

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var question = await _dataRepository.GetQuestionAsync(questionIdAsInt);
            if (question == null)
            {
                context.Succeed(requirement);
                return;
            }

            if (question.UserId != userId)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);

            // TODO - check that the user is authenticated
            // TODO - get the question id from the request
            // TODO - get the user id from the name identifier claim
            // TODO - get the question from the data repository
            // TODO - if the question can't be found go to the next piece of middleware
            // TODO - return failure if the user id in the question from the data repository is different to the user id in the request

            // TODO - return success if we manage to get here
        }
    }
}

