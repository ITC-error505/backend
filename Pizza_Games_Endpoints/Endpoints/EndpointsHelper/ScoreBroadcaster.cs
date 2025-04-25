namespace Pizza_Games_Endpoints.Endpoints.EndpointsHelper
{
    public static class ScoreBroadcaster
    {
        public static event Func<Task>? OnScorePosted;

        public static async Task NotifyNewTopScore()
        {
            if (OnScorePosted != null)
            {
                foreach (var consumer in OnScorePosted.GetInvocationList())
                {
                    await ((Func<Task>)consumer)();
                }
            }
        }
    }
}
