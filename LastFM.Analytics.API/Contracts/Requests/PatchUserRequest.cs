using LastFM.Analytics.Data.Enums;

namespace LastFM.Analytics.API.Contracts.Requests;

public record PatchUserRequest(SyncStatus SyncStatus);