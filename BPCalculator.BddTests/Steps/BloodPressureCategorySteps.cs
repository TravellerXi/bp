using System;
using BPCalculator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reqnroll;

namespace BPCalculator.BddTests.Steps
{
    [Binding]
    public class BloodPressureCategorySteps
    {
        private readonly BloodPressure _bp = new();
        private BPCategory _result;
        private double _map;
        private MapCategory _mapBand;

        [Given(@"a systolic reading of (.*)")]
        public void GivenASystolicReadingOf(int systolic) => _bp.Systolic = systolic;

        [Given(@"a diastolic reading of (.*)")]
        public void GivenADiastolicReadingOf(int diastolic) => _bp.Diastolic = diastolic;

        [When(@"I ask for the blood pressure category")]
        public void WhenIAskForTheBloodPressureCategory() => _result = _bp.Category;

        [Then(@"the category should be ""(.*)""")]
        public void ThenTheCategoryShouldBe(string expected) =>
            Assert.AreEqual(Enum.Parse<BPCategory>(expected), _result);

        [When(@"I ask for the mean arterial pressure")]
        public void WhenIAskForTheMeanArterialPressure() => _map = _bp.MeanArterialPressure;

        [Then(@"the mean arterial pressure should be (.*)")]
        public void ThenTheMeanArterialPressureShouldBe(double expected) =>
            Assert.AreEqual(expected, _map, 0.05);

        [When(@"I ask for the mean arterial pressure band")]
        public void WhenIAskForTheMeanArterialPressureBand() => _mapBand = _bp.MeanArterialPressureCategory;

        [Then(@"the mean arterial pressure band should be ""(.*)""")]
        public void ThenTheMeanArterialPressureBandShouldBe(string expected) =>
            Assert.AreEqual(Enum.Parse<MapCategory>(expected), _mapBand);
    }
}
