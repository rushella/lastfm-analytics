using LastFM.Analytics.API.Contracts.Enums;
using LastFM.Analytics.API.SyncTasks;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(DatabaseContext databaseContext, ISchedulerFactory schedulerFactory) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<User>> Get([FromQuery]string username)
	{
		var user = await databaseContext.Users.Where((x) => x.Name == username).FirstOrDefaultAsync();
		
		if (user == null)
		{
			return NotFound();
		}
		
		return Ok(user);
	}
	
	[HttpPost("/{username}/full-sync")]
	public async Task<ActionResult<User>> Post(string username)
	{
		var user = await databaseContext.Users.Where((x) => x.Name == username).FirstOrDefaultAsync();
		
		if (user == null)
		{
			return NotFound();
		}
		
		var scheduler = await schedulerFactory.GetScheduler();
		
		await scheduler.TriggerJob(new JobKey(nameof(FullSyncJob)), new JobDataMap { { "UserId", user.Id } });
		
		return Ok(user);
	}
}