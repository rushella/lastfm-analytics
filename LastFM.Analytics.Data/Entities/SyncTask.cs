using LastFM.Analytics.Data.Enums;

namespace LastFM.Analytics.Data.Entities;

public class SyncTask
{
	public long Id { get; set; }
	public required string UserName { get; set; }
	public required SyncTaskType Type { get; set; }
	public required SyncTaskStatus Status { get; set; }
}