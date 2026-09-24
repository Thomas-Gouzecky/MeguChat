public class ValidateObject
{

    public static bool Validate(IGroupChatRequest request)
    {
        ValidateName(request.Name);

        return true;
    }
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Group chat name cannot be empty.");
        }
    }
}