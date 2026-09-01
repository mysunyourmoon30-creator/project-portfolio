namespace Innovation.Hardware;

public class PlcConnectionException(string message) : Exception(message);

public class PlcTimeoutException(string message) : Exception(message);
