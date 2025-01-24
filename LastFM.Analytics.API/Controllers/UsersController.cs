using LastFM.Analytics.Data;
using LastFM.Analytics.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LastFM.Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(DataContext dataContext) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<User>> Get([FromQuery]string username)
	{
		var user = await dataContext.Users.Where((x) => x.Name == username).FirstOrDefaultAsync();
		
		if (user == null)
		{
			return NotFound();
		}
		
		return Ok(user);
	}
}