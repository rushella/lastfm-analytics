namespace LastFM.Analytics.API.Utils;

public class OutgoingRequestRateLimiter(int count, TimeSpan timeFrame)
{
	private readonly SemaphoreSlim _semaphore = new(1);
	private readonly Queue<DateTime> _times = new(count);

	public async Task WaitForAvailability(CancellationToken cancellationToken)
	{
		await _semaphore.WaitAsync(cancellationToken);

		var now = DateTime.Now;

		if (_times.Count != count)
		{
			_times.Enqueue(now);
			_semaphore.Release();
		}

		var timeInQueue = _times.Peek();
		var timeDelta = now - timeInQueue;

		if (timeDelta < timeFrame)
		{
			var timeToWait = timeFrame - timeDelta;
			await Task.Delay(timeToWait, cancellationToken);
		}

		_times.Dequeue();
		_times.Enqueue(DateTime.Now);

		_semaphore.Release();
	}
}