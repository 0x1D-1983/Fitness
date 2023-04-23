using System;
namespace ClientGateway.Domain
{
	public class HeartRate
	{
		public int Value { get; }
		public DateTime DateTime { get; }

		public HeartRate(int value, DateTime dateTime)
		{
			Value = value;
			DateTime = dateTime;
		}
    }
}

