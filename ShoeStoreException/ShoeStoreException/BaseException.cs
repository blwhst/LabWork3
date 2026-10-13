namespace ShoeStoreException;

public class BaseException(string message, Exception? innerException = null)
    : Exception(message, innerException);