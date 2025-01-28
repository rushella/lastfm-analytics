using System.Text.Json;
using IF.Lastfm.Core.Api;
using LastFM.Analytics.API.Services.Dtos;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Services;

public class LastFmSyncService(DataContext dataContext, LastfmClient lastfmClient)
{
	public async Task<SyncResult> SyncUserInfo(string username, CancellationToken cancellationToken)
	{
		var userInDatabase = await dataContext.Users.SingleOrDefaultAsync(x => x.Name == username, cancellationToken);

		var userInfo = await lastfmClient.User.GetInfoAsync(username);

		if (!userInfo.Success)
		{
			return new SyncResult { IsSucceded = false };
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
		
		return new SyncResult { IsSucceded = true };
	}
}