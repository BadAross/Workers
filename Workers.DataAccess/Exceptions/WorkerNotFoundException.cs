namespace Workers.DataAccess.Exceptions;

public class WorkerNotFoundException(string message) : Exception(message);