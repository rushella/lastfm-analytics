using LastFM.Analytics.API.Contracts.Enums;
using LastFM.Analytics.API.Contracts.Requests;
using LastFM.Analytics.API.Contracts.Responses;
using LastFM.Analytics.API.SyncTasks;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncTasksController(ISchedulerFactory schedulerFactory) : ControllerBase
{
	[HttpPost("/sync-tasks")]
	public async Task<ActionResult<PostSyncTaskResponse>> Post([FromBody]PostSyncTaskRequest request)
	{
		var scheduler = await schedulerFactory.GetScheduler();
		
		switch (request.Type)
		{
			case SyncTaskType.UserInfoSync:
				break;
			case SyncTaskType.UserScrobblesSync:
				break;
			case SyncTaskType.FullSync:
				await scheduler.TriggerJob(new JobKey(nameof(FullSyncJob)), new JobDataMap { { "LastFmUserName", request.UserName } });
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		
		return Ok();
	}
}