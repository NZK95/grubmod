using System.ComponentModel;

namespace grubmod
{
    internal class Option : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public OptionFields Fields { get; }

        private string _varSelectedValue;
        public string VarSelectedValue
        {
            get => _varSelectedValue;
            set
            {
                if (string.Equals(_varSelectedValue, value))
                    return;

                _varSelectedValue = value;
                OnPropertyChanged(nameof(VarSelectedValue));
                Logger.Log($"Value of {this.Fields.VarName} is changed to - {value}", LogType.Information);

                if (Helpers.AllNotNull(Fields.VarName, Fields.VarOffset, BIOSFileParser.ExtractHexValue(this), Fields.VarSize, Fields.VarSectionName))
                    Grub.LogChanges(Fields.VarName, Fields.VarOffset, BIOSFileParser.ExtractHexValue(this), _varSelectedValue, Fields.VarSize, Fields.VarSectionName);
            }
        }

        public Option(OptionFields fields)
        {
            Fields = fields;
            _varSelectedValue = GetSelectedValueForOption();
        }

        private string GetSelectedValueForOption() => Fields.OptionType switch
        {
            Labels.NORMAL_OPTION_DEFINITION => Fields.VarValues.Contains(Fields.VarBIOSDefaultValue)
                ? Fields.VarBIOSDefaultValue
                : Fields.VarValues.FirstOrDefault() ?? string.Empty,

            Labels.NUMERIC_OPTION_DEFINITION => !string.IsNullOrEmpty(Fields.VarBIOSDefaultValue) && Fields.VarBIOSDefaultValue != Labels.NOT_FOUND
                ? Fields.VarBIOSDefaultValue
                : string.Empty,

            Labels.CHECKBOX_OPTION_DEFINITION => !string.IsNullOrEmpty(Fields.VarBIOSDefaultValue) && Fields.VarBIOSDefaultValue != Labels.NOT_FOUND
                ? (Fields.VarBIOSDefaultValue.Equals("Enabled") ? "True" : "False")
                : string.Empty,

            _ => string.Empty
        };

        public override bool Equals(object? obj) => 
            obj is Option { Fields.VarName: var varName, Fields.VarOffset: var varOffset, Fields.VarStoreId: var varStoreId, Fields.VarDescription: var varDescription }
            && (varName, varOffset, varStoreId, varDescription) == (this.Fields.VarName, this.Fields.VarOffset, this.Fields.VarStoreId, this.Fields.VarDescription);

        public override int GetHashCode() => (this.Fields.VarName, this.Fields.VarOffset, this.Fields.VarStoreId, this.Fields.VarDescription).GetHashCode();
    }
}
