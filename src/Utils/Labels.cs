namespace grubmod
{
    internal struct Labels
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

        public const string END_OF_OPTION_SPACE = "End";
        public const string NOT_FOUND = "N/A";
        public const string VALUE = "Value:";
        public const string DEFAULT = "Default";
        public const string DEFAULTID = "Default DefaultId";
        public const string MFG_DEFAULT = "Default, MfgDefault";
        public const string NAME = "Name:";

        public const string SECTION_PE32 = "Section_PE32";
        public const string CONFIG = "Config";

        public const string SCRIPT_COMMAND_PREFIX = "setup_var.efi";
        public const string END_OF_SCRIPT = "setup_var.efi 0xA2F -n Setup -r";
        public const string SCRIPT_TEMPLATE = $"# {SCRIPT_COMMAND_PREFIX} VarOffset HexValue -s VarSize -n VarSectionName";

        public const string GRUBMOD_LINK = "https://www.github.com/NZK95/grubmod";
        public const string AUTHOR_LINK = "https://www.github.com/NZK95";
        public const string AUTHOR_WATERMARK = $"# ({AUTHOR_LINK})";
        public const string GRUBMOD_WATERMARK = $"# ({GRUBMOD_LINK})";

        public const string SEARCH = "Search..";
        public const string GOOGLE_SEARCH_PATTERN = " BIOS setting.";
    }
}
