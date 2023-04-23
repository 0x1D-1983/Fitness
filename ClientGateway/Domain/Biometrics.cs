using System;
namespace ClientGateway.Domain
{
	public class Biometrics
	{
		public Guid DeviceId { get; }
		public List<HeartRate> HeartRates { get; }
		public List<StepCount> StepCounts { get; }
		public int MaxHeartRate { get; }

		public Biometrics(Guid deviceId, List<HeartRate> heartRates, List<StepCount> stepCounts, int maxHeartRate)
		{
			DeviceId = deviceId;
			HeartRates = heartRates;
			StepCounts = stepCounts;
			MaxHeartRate = maxHeartRate;
		}
	}
}

