public static class Helper
{
    public static string CleanInput(string input) => input
        .Replace("\u000B", "")
        .Replace("\u200B", "")
        .Replace("\uFEFF", "")
        .Replace("\u00A0", " ")
        .Replace("\t", " ")
        .Trim();

    public static bool HasSemanticErrors(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return true;

        var lines = code.Split('\n');
        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
            bool isControl = line.StartsWith("if") || line.StartsWith("for") || line.StartsWith("while");
            bool isBlock = line.EndsWith("{") || line.EndsWith("}");
            if (!line.EndsWith(";") && !isControl && !isBlock) return true;
        }

        int parens = 0, braces = 0;
        foreach (char c in code)
        {
            if (c == '(') parens++;
            else if (c == ')') parens--;
            else if (c == '{') braces++;
            else if (c == '}') braces--;
        }

        return parens != 0 || braces != 0;
    }
}