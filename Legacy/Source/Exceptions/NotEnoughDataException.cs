using System;

namespace Legacy.Exceptions;

/// <summary>
/// Exception thrown when a ByteQueue doesn't have enough data to complete a read operation.
/// </summary>
internal class NotEnoughDataException : Exception
{
    public NotEnoughDataException() : base("Not enough data in buffer to complete the read operation")
    {
    }

    public NotEnoughDataException(string message) : base(message)
    {
    }

    public NotEnoughDataException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
