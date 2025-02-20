using LastFM.Analytics.Data.Enums;

namespace LastFM.Analytics.Data.Entities;

public class User
{
	public long Id { get; set; }
	public required string Name { get; set; }
	
	public string? Country { get; set; }
	public string? PictureLinks { get; set; }
	public Uri? Url { get;set; }
	public DateTime? RegisteredAt { get; set; }
	public DateTime? LastSyncedAt { get; set; }
	public SyncStatus? SyncStatus { get; set; }
}