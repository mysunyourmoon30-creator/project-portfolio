namespace Innovation.Hardware;

// Preserves an exact behavior from the original Delphi-derived system
// (README.md §4.2): weight values written to the PLC are TRUNCATED, never
// rounded - multiply by 100 then floor, matching Delphi's `Trunc`. Using
// Math.Round here would silently change what gets written to the PLC
// register versus the original system for values like 12.345 (truncates to
// 1234 = 12.34kg, would round to 1235 = 12.35kg) - operators are trained
// against the truncating behavior.
public static class PlcWeightConverter
{
    public static int ToPlcValue(decimal weightKg) => (int)Math.Floor(weightKg * 100m);

    public static decimal FromPlcValue(int rawValue) => rawValue / 100m;
}
