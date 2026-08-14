using BPCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BPCalculator.UnitTests
{
    [TestClass]
    public class MeanArterialPressureTests
    {
        private static BloodPressure Bp(int systolic, int diastolic) =>
            new() { Systolic = systolic, Diastolic = diastolic };

        [DataTestMethod]
        [DataRow(120, 80, 93.3)]     // 80 + 40/3
        [DataRow(150, 90, 110.0)]    // 90 + 60/3
        [DataRow(90, 60, 70.0)]      // 60 + 30/3 - exactly on the Normal boundary
        [DataRow(70, 40, 50.0)]      // lowest permitted reading
        [DataRow(190, 100, 130.0)]   // highest permitted reading
        public void MeanArterialPressure_IsCalculatedAndRoundedToOneDecimal(
            int systolic, int diastolic, double expected) =>
            Assert.AreEqual(expected, Bp(systolic, diastolic).MeanArterialPressure, 0.05);

        [DataTestMethod]
        [DataRow(190, 100, MapCategory.High)]    // 130.0
        [DataRow(150, 90, MapCategory.High)]     // 110.0
        [DataRow(130, 85, MapCategory.High)]     // 100.0 - lower limit is inclusive
        [DataRow(129, 85, MapCategory.Normal)]   // 99.7 - just below the boundary
        [DataRow(120, 80, MapCategory.Normal)]   // 93.3
        [DataRow(90, 60, MapCategory.Normal)]    // 70.0 - lower limit is inclusive
        [DataRow(89, 60, MapCategory.Low)]       // 69.7 - just below the boundary
        [DataRow(70, 40, MapCategory.Low)]       // 50.0
        public void MeanArterialPressureCategory_UsesInclusiveLowerLimits(
            int systolic, int diastolic, MapCategory expected) =>
            Assert.AreEqual(expected, Bp(systolic, diastolic).MeanArterialPressureCategory);

        [TestMethod]
        public void MeanArterialPressure_EqualsDiastolic_WhenThereIsNoPulsePressure()
        {
            // Degenerate but arithmetically meaningful: MAP collapses onto diastolic.
            var bp = Bp(80, 80);
            Assert.AreEqual(80.0, bp.MeanArterialPressure, 0.05);
        }

        [TestMethod]
        public void MeanArterialPressure_IsAlwaysBetweenDiastolicAndSystolic()
        {
            for (var systolic = BloodPressure.SystolicMin; systolic <= BloodPressure.SystolicMax; systolic += 7)
            {
                for (var diastolic = BloodPressure.DiastolicMin; diastolic <= BloodPressure.DiastolicMax; diastolic += 5)
                {
                    if (diastolic > systolic)
                    {
                        continue;
                    }

                    var map = Bp(systolic, diastolic).MeanArterialPressure;
                    Assert.IsTrue(map >= diastolic && map <= systolic,
                        $"MAP {map} outside [{diastolic}, {systolic}]");
                }
            }
        }
    }
}
