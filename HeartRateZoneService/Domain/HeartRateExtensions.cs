using System;
namespace HeartRateZoneService.Domain
{
    public static class HeartRateExtensions
    {
        private const double Zone1Threshold = 0.5;
        private const double Zone2Threshold = 0.6;
        private const double Zone3Threshold = 0.7;
        private const double Zone4Threshold = 0.8;
        private const double Zone5Threshold = 0.9;

        public static HeartRateZone GetHeartRateZone(this HeartRate heartRate, int maxHeartRate)
        {
            double percentage = (Double)heartRate.Value / (Double)maxHeartRate;

            return GetHeartRateZoneInner(percentage);
        }

        private static HeartRateZone GetHeartRateZoneInner(double percentage) => percentage switch
        {
            (>= Zone1Threshold) and (< Zone2Threshold) => HeartRateZone.Zone1,
            (>= Zone2Threshold) and (< Zone3Threshold) => HeartRateZone.Zone2,
            (>= Zone3Threshold) and (< Zone4Threshold) => HeartRateZone.Zone3,
            (>= Zone4Threshold) and (< Zone5Threshold) => HeartRateZone.Zone4,
            (>= Zone5Threshold) => HeartRateZone.Zone5,
            _ => HeartRateZone.None
        };
    }
}