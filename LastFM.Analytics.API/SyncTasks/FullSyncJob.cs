using System.Text.Json;
using IF.Lastfm.Core.Api;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Enums;
using Quartz;

namespace LastFM.Analytics.API.SyncTasks;

public class FullSyncJob(DatabaseContext databaseContext, LastfmClient lastFmClient) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userId = long.Parse(context.MergedJobDataMap["UserId"].ToString()!);
        var user = await databaseContext.Users.FindAsync(userId, context.CancellationToken)!;

        if (user == null)
        {
            throw new NotImplementedException();
        }
        
        var lastFmResponse = await lastFmClient.User.GetInfoAsync(user.Name);

        if (!lastFmResponse.Success)
        {
            throw new NotImplementedException();
        }

        user.Name = lastFmResponse.Content.Name;
        user.PictureLinks = JsonSerializer.Serialize(lastFmResponse.Content.Avatar);
        user.Url = new Uri("https://last.fm/" + lastFmResponse.Content.Name);
        user.RegisteredAt = lastFmResponse.Content.TimeRegistered;
        user.LastSyncedAt = DateTime.UtcNow;
        user.SyncStatus = SyncStatus.Finished;

        await databaseContext.SaveChangesAsync(context.CancellationToken);
    }
}