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

        [Given(@"a systolic reading of (.*)")]
        public void GivenASystolicReadingOf(int systolic) => _bp.Systolic = systolic;

        [Given(@"a diastolic reading of (.*)")]
        public void GivenADiastolicReadingOf(int diastolic) => _bp.Diastolic = diastolic;

        [When(@"I ask for the blood pressure category")]
        public void WhenIAskForTheBloodPressureCategory() => _result = _bp.Category;

        [Then(@"the category should be ""(.*)""")]
        public void ThenTheCategoryShouldBe(string expected) =>
            Assert.AreEqual(Enum.Parse<BPCategory>(expected), _result);
    }
}
