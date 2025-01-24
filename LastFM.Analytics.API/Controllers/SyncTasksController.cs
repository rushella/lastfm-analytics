using LastFM.Analytics.API.Contracts.Requests;
using LastFM.Analytics.Data.Entities;
using LastFM.Analytics.Data;
using Microsoft.AspNetCore.Mvc;
using LastFM.Analytics.Data.Enums;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncTasksController(DataContext dataContext) : ControllerBase
{
	[HttpPost("/sync-tasks")]
	public async Task<IActionResult> Post([FromBody]PostSyncTaskBody requestBody)
	{
		var syncTask = new SyncTask
		{
			UserName = requestBody.UserName,
			Type = requestBody.Type,
			Status = SyncTaskStatus.Scheduled
		};

		await dataContext.SaveChangesAsync();

		return Ok(syncTask);
	}
}