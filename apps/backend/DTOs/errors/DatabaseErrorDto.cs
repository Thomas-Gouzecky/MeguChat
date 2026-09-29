public class DatabaseErrorDto
{
    public IEnumerable<DetailItem> Details { get; set; } = new List<DetailItem>();
}

public class DetailItem
{
    IEnumerable<string> Loc { get; set; } = new List<string>();
    string Msg { get; set; } = string.Empty;
    string Type { get; set; } = string.Empty;
    string? Input { get; set; }
    object? Ctx { get; set; }
}