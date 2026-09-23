class UnauthenticatedAccessException : Exception
{
    public UnauthenticatedAccessException(string message)
        : base(message)
    {
    }
}