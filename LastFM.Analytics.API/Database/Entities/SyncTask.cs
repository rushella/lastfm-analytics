using LastFM.Analytics.API.Database.Enums;

namespace LastFM.Analytics.API.Database.Entities;

public class SyncTask
{
	public long Id { get; set; }
	public long UserId { get; set; }
	public SyncTaskType Type { get; set; }
	public SyncTaskStatus Status { get; set; }

	public virtual User? User { get; set; }
}
