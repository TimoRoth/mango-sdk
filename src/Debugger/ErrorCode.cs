namespace Mango.Debugger
{
    public enum ErrorCode
    {
        Success = 0,
        Argument = 64,
        ArgumentNull = 65,
        InvalidOperation = 66,
        NotSupported = 67,
        NotImplemented = 68,
        BadImageFormat = 69,
        OutOfMemory = 80,
        Security = 81,
        StackOverflow = 82,
        StackImbalance = 83,
        Arithmetic = 84,
        DivideByZero = 85,
        IndexOutOfRange = 86,
        InvalidProgram = 87,
        NullReference = 88,
        Breakpoint = 110,
        Timeout = 111,
        SystemCall = 112,
    }
}