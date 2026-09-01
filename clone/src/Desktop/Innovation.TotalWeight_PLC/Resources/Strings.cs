using System.Globalization;
using System.Resources;

namespace Innovation.TotalWeight_PLC.Resources;

// Hand-written accessor over Strings.resx rather than relying on Visual
// Studio's ResXFileCodeGenerator "single file generator" - that tool only
// runs during a VS design-time build, not `dotnet build`/`dotnet test`, and
// this whole clone is meant to build from the CLI alone (README §8.1).
public static class Strings
{
    private static readonly ResourceManager Manager =
        new("Innovation.TotalWeight_PLC.Resources.Strings", typeof(Strings).Assembly);

    public static string Get(string name, params object[] args)
    {
        var format = Manager.GetString(name, CultureInfo.CurrentUICulture) ?? name;
        return args.Length == 0 ? format : string.Format(format, args);
    }

    public static string BarcodeNotFound(string barcode) => Get(nameof(BarcodeNotFound), barcode);
    public static string WeightOutOfRange(decimal min, decimal max) => Get(nameof(WeightOutOfRange), min, max);
    public static string SaveSuccess => Get(nameof(SaveSuccess));
    public static string TotalWeightAlreadyExists => Get(nameof(TotalWeightAlreadyExists));
    public static string StepNotAccepted => Get(nameof(StepNotAccepted));
    public static string RmBalNotFound(string barcode) => Get(nameof(RmBalNotFound), barcode);
    public static string FeeddoorStepNotConfigured => Get(nameof(FeeddoorStepNotConfigured));
    public static string AutoFeedDbWriteFailed => Get(nameof(AutoFeedDbWriteFailed));
    public static string AutoFeedSuccess => Get(nameof(AutoFeedSuccess));
    public static string InvalidCredentials => Get(nameof(InvalidCredentials));
    public static string NoStepsWeighed => Get(nameof(NoStepsWeighed));
}
