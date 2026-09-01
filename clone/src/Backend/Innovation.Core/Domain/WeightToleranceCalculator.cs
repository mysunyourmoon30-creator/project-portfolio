namespace Innovation.Core.Domain;

// Preserves an exact business rule from the original Delphi-derived system
// (README.md §4.1): the acceptable weight range is normally
// target ± Application_Setting[4]/[5], EXCEPT:
//   - step 2, when the kanban's bundled count (Number) != 1
//   - step 3, when Number == 1
// those two cases use a hardcoded ±0.02 instead of the configured setting.
// This quirk must NOT be "fixed" - operators are trained against it and it
// mirrors the original Delphi behavior byte for byte.
public static class WeightToleranceCalculator
{
    private const decimal FixedStepTolerance = 0.02m;

    public static (decimal Min, decimal Max) Calculate(
        int stepNo,
        int kbTogetherNumber,
        decimal target,
        decimal minToleranceSetting,
        decimal maxToleranceSetting)
    {
        bool usesFixedTolerance =
            (stepNo == 2 && kbTogetherNumber != 1) ||
            (stepNo == 3 && kbTogetherNumber == 1);

        decimal minOffset = usesFixedTolerance ? FixedStepTolerance : minToleranceSetting;
        decimal maxOffset = usesFixedTolerance ? FixedStepTolerance : maxToleranceSetting;

        return (target - minOffset, target + maxOffset);
    }
}
