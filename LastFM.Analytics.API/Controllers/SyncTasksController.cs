using LastFM.Analytics.API.Contracts.Requests;
using LastFM.Analytics.API.Contracts.Responses;
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

		var response = new PostSyncTaskResponse
		{
		};
		
		return Ok();
	}
}