using LastFM.Analytics.API.Contracts.Requests;
using LastFM.Analytics.Data.Entities;
using LastFM.Analytics.Data;
using Microsoft.AspNetCore.Mvc;
using LastFM.Analytics.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SyncTasksController(DataContext dataContext) : ControllerBase
{
	[HttpPost("/sync-tasks")]
	public async Task<ActionResult<SyncTask>> Post([FromBody]PostSyncTaskBody requestBody)
	{
		var syncTask = new SyncTask
		{
			UserName = requestBody.UserName,
			Type = requestBody.Type,
			Status = SyncTaskStatus.Scheduled
		};

		dataContext.Add(syncTask);

		await dataContext.SaveChangesAsync();

		return Ok(syncTask);
	}

	[HttpGet("/sync-tasks")]
	public async Task<ActionResult<IEnumerable<SyncTask>>> GetAll([FromQuery]Pagination pagination)
	{
		var toSkipCount = pagination.Page * pagination.Count;
		var toTakeCount = pagination.Count;
		
		var syncTasks = await dataContext.SyncTasks
			.Skip(toSkipCount)
			.Take(toTakeCount)
			.ToListAsync();

		return Ok(syncTasks);
	}
}

public record Pagination(int Page, int Count);