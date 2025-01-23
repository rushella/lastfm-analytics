using LastFM.Analytics.API.Database;
using LastFM.Analytics.API.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(DataContext dataContext) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<User>> Get([FromQuery]string name)
	{
		var user = await dataContext.Users.Where((x) => x.Name == name).FirstOrDefaultAsync();
		
		if (user == null)
		{
			return NotFound();
		}
		
		return Ok(user);
	}
}