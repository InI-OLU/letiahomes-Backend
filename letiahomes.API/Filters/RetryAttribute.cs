using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using letiahomes.Application.Common.Exceptions;

namespace letiahomes.API.Filters
{
    public class RetryAttribute : JobFilterAttribute, IElectStateFilter
    {
        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                if (failedState.Exception is PermanentException)
                {
                    context.CandidateState = new DeletedState
                    {
                        Reason = $"Permanent failure, not retrying: {failedState.Exception.Message}"
                    };
                }
            }
        }
    }
}
