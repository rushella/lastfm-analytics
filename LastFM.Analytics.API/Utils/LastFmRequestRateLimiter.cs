namespace LastFM.Analytics.API.Utils;

public class LastFmRequestRateLimiter(OutgoingRequestRateLimiter outgoingRequestRateLimiter) 
	: DelegatingHandler(new HttpClientHandler())
{
	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		await outgoingRequestRateLimiter.WaitForAvailability(cancellationToken);
		return await base.SendAsync(request, cancellationToken);
	}
}