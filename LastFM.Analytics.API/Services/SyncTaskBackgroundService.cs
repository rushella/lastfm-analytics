using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Services
{
	public class SyncTaskBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
	{
		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var scope = serviceScopeFactory.CreateScope();
			var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();
			var lastFmSyncService = scope.ServiceProvider.GetRequiredService<LastFmSyncService>();

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
						var userSyncResult = await lastFmSyncService.SyncUserInfo(newSyncTask.UserName, cancellationToken);
						if (userSyncResult.IsSucceded)
						{
							newSyncTask.Status = SyncTaskStatus.Finished;
						}
						else 
						{
							newSyncTask.Status = SyncTaskStatus.Failed;
						}
						break;
				}
				
				await dataContext.SaveChangesAsync(cancellationToken);
			}
		}
	}
}