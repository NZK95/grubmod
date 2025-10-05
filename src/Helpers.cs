namespace grubmod
{
    public class Helpers
    {
        public static bool AllNotNull(params object?[] values) => values.All(v => v is not null);
    }
}
