namespace LastFM.Analytics.API.Utils;

public class HttpRequestRateLimiter(RateLimiter rateLimiter) 
	: DelegatingHandler(new HttpClientHandler())
{
	protected override async Task<HttpResponseMessage> SendAsync(
		HttpRequestMessage request, CancellationToken cancellationToken)
	{
		await rateLimiter.WaitForAvailability(cancellationToken);
		return await base.SendAsync(request, cancellationToken);
	}
}