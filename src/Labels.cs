namespace grubmod
{
    public struct Labels
    {
        public static readonly List<string> VarSectionNameLabels = new List<string>() { "Name", "VarStore Guid:" };

        public const string NORMAL_OPTION_DEFINITION = "OneOf Prompt";
        public const string NUMERIC_OPTION_DEFINITION = "Numeric Prompt";
        public const string CHECKBOX_OPTION_DEFINITION = "CheckBox Prompt";
        public const string VAROFFSET_DEFINITION = "VarOffset";
        public const string VARSTOREID_DEFINITION = "VarStoreId";
        public const string DESCRIPTION_DEFINITION = "Help";
        public const string VALUE_DEFINITION = "OneOfOption Option";
        public const string SIZE_DEFINITION = "Size";
    }
}
