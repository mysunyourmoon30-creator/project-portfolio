namespace Innovation.Services.Errors;

// Typed exceptions replacing catch(ex){throw ex;} / StatusCode(500, ex)
// (Backend ROADMAP §5.5 / §13). The API's ProblemDetailsExceptionMapper
// maps each to a specific HTTP status and RFC 7807 type.
public class BarcodeNotFoundException(string barcode) : Exception($"Barcode not found: {barcode}");

public class RmBalNotFoundException(string barcode) : Exception($"RM_BAL has no balance for barcode: {barcode}");

public class SettingNotFoundException(string settingName) : Exception($"Required setting not found: {settingName}");

public class TotalWeightAlreadyExistsException(int kbTogetherId)
    : Exception($"TotalWeight already exists for KbTogether {kbTogetherId}");

public class StepNotAcceptedException(int stepNo)
    : Exception($"Step {stepNo} weight must be submitted before Accept");

public class InvalidCredentialsException() : Exception("Invalid username or password");
