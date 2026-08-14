using System.ComponentModel.DataAnnotations;

namespace BPCalculator
{
    // BP categories
    public enum BPCategory
    {
        [Display(Name = "Low Blood Pressure")] Low,
        [Display(Name = "Ideal Blood Pressure")] Ideal,
        [Display(Name = "Pre-High Blood Pressure")] PreHigh,
        [Display(Name = "High Blood Pressure")] High
    };

    public class BloodPressure
    {
        public const int SystolicMin = 70;
        public const int SystolicMax = 190;
        public const int DiastolicMin = 40;
        public const int DiastolicMax = 100;

        // Category boundaries (NHS chart). Lower limits are inclusive.
        public const int HighSystolic = 140;
        public const int HighDiastolic = 90;
        public const int PreHighSystolic = 120;
        public const int PreHighDiastolic = 80;
        public const int IdealSystolic = 90;
        public const int IdealDiastolic = 60;

        [Range(SystolicMin, SystolicMax, ErrorMessage = "Invalid Systolic Value")]
        public int Systolic { get; set; }                       // mmHG

        [Range(DiastolicMin, DiastolicMax, ErrorMessage = "Invalid Diastolic Value")]
        public int Diastolic { get; set; }                      // mmHG

        /// <summary>
        /// Blood pressure category derived from the systolic/diastolic pair.
        /// Either reading on its own is enough to raise the category, but a Low
        /// classification requires both readings to be below the ideal range.
        /// </summary>
        public BPCategory Category
        {
            get
            {
                if (Systolic >= HighSystolic || Diastolic >= HighDiastolic)
                {
                    return BPCategory.High;
                }

                if (Systolic >= PreHighSystolic || Diastolic >= PreHighDiastolic)
                {
                    return BPCategory.PreHigh;
                }

                if (Systolic < IdealSystolic && Diastolic < IdealDiastolic)
                {
                    return BPCategory.Low;
                }

                return BPCategory.Ideal;
            }
        }
    }
}
