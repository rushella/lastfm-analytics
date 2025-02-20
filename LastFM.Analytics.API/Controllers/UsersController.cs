using LastFM.Analytics.API.Contracts.Requests;
using LastFM.Analytics.API.SyncTasks;
using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using LastFM.Analytics.Data.Enums;
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
	
	[HttpPost]
	public async Task<ActionResult<User>> Post([FromBody]PostUserRequest request)
	{
		var existingUser = await databaseContext.Users.Where((x) => x.Name == request.UserName).FirstOrDefaultAsync();
		
		if (existingUser != null)
		{
			return Conflict();
		}

		var newUser = new User { Name = request.UserName, SyncStatus = SyncStatus.Scheduled };

		await databaseContext.Users.AddAsync(newUser);
		await databaseContext.SaveChangesAsync();
		
		var scheduler = await schedulerFactory.GetScheduler();
		await scheduler.TriggerJob(new JobKey(nameof(FullSyncJob)), new JobDataMap { { "UserId", newUser.Id } });
		
		return Ok(newUser);
	}
	
	[HttpPatch]
	public async Task<ActionResult<User>> Patch(string userName, [FromBody]PatchUserRequest request)
	{
		var existingUser = await databaseContext.Users.Where((x) => x.Name == userName).FirstOrDefaultAsync();
		
		if (existingUser == null)
		{
			return NotFound();
		}

		existingUser.SyncStatus = request.SyncStatus;
		
		await databaseContext.SaveChangesAsync();
		
		var scheduler = await schedulerFactory.GetScheduler();
		await scheduler.TriggerJob(new JobKey(nameof(FullSyncJob)), new JobDataMap { { "UserId", existingUser.Id } });
		
		return Ok(existingUser);
	}
}