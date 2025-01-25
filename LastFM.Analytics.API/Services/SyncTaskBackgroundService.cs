using System.Text.Json;
using IF.Lastfm.Core.Api;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using LastFM.Analytics.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Services
{
	public class SyncTaskBackgroundService(LastfmClient lastfmClient, IServiceScopeFactory serviceScopeFactory) : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var scope = serviceScopeFactory.CreateScope();
			var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

			while (!cancellationToken.IsCancellationRequested)
			{
				var newSyncTask = await dataContext.SyncTasks.OrderBy(x => x.Id)
					.Where(x => x.Status == SyncTaskStatus.Scheduled)
					.FirstOrDefaultAsync(cancellationToken);

				if (newSyncTask == null)
				{
					await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
					continue;
				}

				newSyncTask.Status = SyncTaskStatus.InProgress;

				await dataContext.SaveChangesAsync(cancellationToken);

				switch (newSyncTask.Type)
				{
					case SyncTaskType.UserInfoSync:
						var userInDatabase = await dataContext.Users.SingleOrDefaultAsync(x => x.Name == newSyncTask.UserName, cancellationToken);

						var userInfo = await lastfmClient.User.GetInfoAsync(newSyncTask.UserName);

						if (!userInfo.Success)
						{
							newSyncTask.Status = SyncTaskStatus.Failed;
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
							dataContext.Add(newUser);
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
						await dataContext.SaveChangesAsync(cancellationToken);
						break;
				}
			}
		}
	}
}