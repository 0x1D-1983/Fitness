using System;
namespace ClientGateway.Domain
{
	public class StepCount
    {
        //public StepCount(int value, DateTime dateTime) : base(value, dateTime) { }
        public int Value { get; }
        public DateTime DateTime { get; }

        public StepCount(int value, DateTime dateTime)
        {
            Value = value;
            DateTime = dateTime;
        }
    }
}

