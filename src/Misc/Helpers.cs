namespace grubmod
{
    internal class Helpers
    {
        public static bool AllNotNull(params object?[] values) => values.All(v => v is not null);

        public static string ConvertDecimalToHex(string? decimalValue) =>
            !string.IsNullOrEmpty(decimalValue) ? "0x" + Convert.ToString(int.Parse(decimalValue), 16) : string.Empty;
    }
}
