using BPCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BPCalculator.UnitTests
{
    /// <summary>
    /// The 12 pairs in LecturerAcceptanceCases are the values the lecturer published
    /// in the CA1 feedback announcement (Brightspace announcement 148608, 06/01/2026).
    /// They are treated as a non-negotiable acceptance baseline.
    /// </summary>
    [TestClass]
    public class BloodPressureCategoryTests
    {
        private static BPCategory CategoryFor(int systolic, int diastolic) =>
            new BloodPressure { Systolic = systolic, Diastolic = diastolic }.Category;

        [DataTestMethod]
        [DataRow(150, 90)]
        [DataRow(140, 40)]
        [DataRow(95, 90)]
        public void High_IsReturned_ForLecturerAcceptanceCases(int systolic, int diastolic) =>
            Assert.AreEqual(BPCategory.High, CategoryFor(systolic, diastolic));

        [DataTestMethod]
        [DataRow(130, 70)]
        [DataRow(100, 85)]
        [DataRow(120, 89)]
        public void PreHigh_IsReturned_ForLecturerAcceptanceCases(int systolic, int diastolic) =>
            Assert.AreEqual(BPCategory.PreHigh, CategoryFor(systolic, diastolic));

        [DataTestMethod]
        [DataRow(110, 70)]
        [DataRow(90, 75)]
        [DataRow(80, 65)]
        public void Ideal_IsReturned_ForLecturerAcceptanceCases(int systolic, int diastolic) =>
            Assert.AreEqual(BPCategory.Ideal, CategoryFor(systolic, diastolic));

        [DataTestMethod]
        [DataRow(89, 50)]
        [DataRow(80, 59)]
        [DataRow(70, 40)]
        public void Low_IsReturned_ForLecturerAcceptanceCases(int systolic, int diastolic) =>
            Assert.AreEqual(BPCategory.Low, CategoryFor(systolic, diastolic));

        // ---- Boundary conditions: lower limits are inclusive ----

        [DataTestMethod]
        [DataRow(140, 60, BPCategory.High)]     // systolic on the High boundary
        [DataRow(139, 60, BPCategory.PreHigh)]  // one below it
        [DataRow(100, 90, BPCategory.High)]     // diastolic on the High boundary
        [DataRow(100, 89, BPCategory.PreHigh)]  // one below it
        [DataRow(120, 60, BPCategory.PreHigh)]  // systolic on the PreHigh boundary
        [DataRow(119, 60, BPCategory.Ideal)]    // one below it
        [DataRow(100, 80, BPCategory.PreHigh)]  // diastolic on the PreHigh boundary
        [DataRow(100, 79, BPCategory.Ideal)]    // one below it
        [DataRow(90, 59, BPCategory.Ideal)]     // systolic on the Ideal boundary keeps it out of Low
        [DataRow(89, 59, BPCategory.Low)]       // both below the ideal range
        [DataRow(89, 60, BPCategory.Ideal)]     // diastolic alone keeps it out of Low
        public void Boundaries_AreInclusiveOnTheLowerLimit(int systolic, int diastolic, BPCategory expected) =>
            Assert.AreEqual(expected, CategoryFor(systolic, diastolic));

        // ---- Extremes of the permitted input range ----

        [DataTestMethod]
        [DataRow(BloodPressure.SystolicMin, BloodPressure.DiastolicMin, BPCategory.Low)]
        [DataRow(BloodPressure.SystolicMax, BloodPressure.DiastolicMax, BPCategory.High)]
        public void Extremes_AreClassified(int systolic, int diastolic, BPCategory expected) =>
            Assert.AreEqual(expected, CategoryFor(systolic, diastolic));
    }
}
