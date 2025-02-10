using LastFM.Analytics.API.Contracts.Enums;

namespace LastFM.Analytics.API.Contracts.Requests;

public record PostSyncTaskRequest(SyncTaskType Type, string UserName);