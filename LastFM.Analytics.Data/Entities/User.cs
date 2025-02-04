namespace LastFM.Analytics.Data.Entities;

public class User
{
	public long Id { get; set; }
	public required string Name { get; set; }
	public required long PlayCount { get; set; }
	public required long ArtistCount { get; set; }
	public required long TrackCount { get; set; }
	public required long AlbumCount { get; set; }
	public string? Country { get; set; }
	public string? ProfilePictureLinks { get; set; }
	public required Uri Url { get;set; }
	public required DateTime RegistrationDate { get; set; }
}