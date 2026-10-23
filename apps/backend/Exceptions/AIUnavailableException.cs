public sealed class AIUnavailableException : Exception
{
    public AIUnavailableException()
        : base("The AI is not currently active.")
    {
    }
}
