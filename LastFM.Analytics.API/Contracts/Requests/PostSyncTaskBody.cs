using LastFM.Analytics.Data.Enums;

namespace LastFM.Analytics.API.Contracts.Requests;

public record PostSyncTaskBody(SyncTaskType Type, string UserName);