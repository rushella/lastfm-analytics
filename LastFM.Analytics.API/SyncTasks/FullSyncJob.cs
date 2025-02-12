using System.Text.Json;
using IF.Lastfm.Core.Api;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LastFM.Analytics.API.SyncTasks;

public class FullSyncJob(DatabaseContext databaseContext, IUserApi lastFmUserApi) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var userName = context.Get("LastFmUserName")?.ToString();
        
        var userInDatabase = await databaseContext.Users.SingleOrDefaultAsync(x => x.Name == userName, context.CancellationToken);

        var userInfo = await lastFmUserApi.GetInfoAsync(userName);

        if (!userInfo.Success)
        {
            throw new NotImplementedException();
        }

        if (userInDatabase == null)
        {
            var newUser = new User
            {
                AlbumCount = 0,
                ArtistCount = 0,
                PlayCount = userInfo.Content.Playcount,
                Name = userInfo.Content.Name,
                ProfilePictureLinks = JsonSerializer.Serialize(userInfo.Content.Avatar),
                RegistrationDate = userInfo.Content.TimeRegistered,
                TrackCount = 0,
                Url = new Uri("https://last.fm/" + userInfo.Content.Name)
            };
            databaseContext.Add(newUser);
        }
        else
        {
            userInDatabase.AlbumCount = 0;
            userInDatabase.ArtistCount = 0;
            userInDatabase.PlayCount = userInfo.Content.Playcount;
            userInDatabase.Name = userInfo.Content.Name;
            userInDatabase.ProfilePictureLinks = JsonSerializer.Serialize(userInfo.Content.Avatar);
            userInDatabase.RegistrationDate = userInfo.Content.TimeRegistered;
            userInDatabase.TrackCount = 0;
            userInDatabase.Url = new Uri("https://last.fm/" + userInfo.Content.Name);
        }

        await databaseContext.SaveChangesAsync(context.CancellationToken);
    }
}