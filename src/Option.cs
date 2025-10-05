using System.ComponentModel;

namespace grubmod
{
    public class Option : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string prop) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        public string OptionType { get; private set; }
        public string VarName { get; private set; }
        public string VarOffset { get; private set; }
        public string VarStoreId { get; private set; }
        public string VarSectionName { get; private set; }
        public string VarSize { get; private set; }
        public string VarDescription { get; private set; }
        public string VarBIOSDefaultValue { get; private set; }
        public List<string> VarValues { get; private set; }

        private string _varSelectedValue;
        public string VarSelectedValue
        {
            get => _varSelectedValue;
            set
            {
                if (string.Equals(_varSelectedValue, value)) return;

                _varSelectedValue = value;
                OnPropertyChanged(nameof(VarSelectedValue));

                if (!_varSelectedValue.Equals(VarBIOSDefaultValue) && _varSelectedValue is not null && VarName
                is not null && VarOffset is not null && VarSectionName is not null && VarSize is not null)
                    Grub.LogChanges(VarName, VarOffset, VarSelectedValue, VarSize, VarSectionName);
            }
        }

        public Option(string optionType, string varName, string varOffset, string varStoreId,
                      string varSectionName, string varSize, string varDescription, string varBIOSDefaultValue,
                      List<string> varValues)
        {
            OptionType = optionType;
            VarName = varName;
            VarOffset = varOffset;
            VarStoreId = varStoreId;
            VarSectionName = varSectionName;
            VarSize = varSize;
            VarDescription = varDescription;
            VarBIOSDefaultValue = varBIOSDefaultValue;
            VarValues = varValues;
            _varSelectedValue = GetSelectedValueForOption();
        }

        private string GetSelectedValueForOption() => OptionType switch
        {
            Labels.NORMAL_OPTION_DEFINITION => VarValues.Contains(VarBIOSDefaultValue) ? VarBIOSDefaultValue : VarValues.FirstOrDefault(),
            Labels.NUMERIC_OPTION_DEFINITION => (!string.IsNullOrEmpty(VarBIOSDefaultValue) || !VarBIOSDefaultValue.Equals("N/A")) ? VarBIOSDefaultValue : string.Empty,
            Labels.CHECKBOX_OPTION_DEFINITION => (!string.IsNullOrEmpty(VarBIOSDefaultValue) || !VarBIOSDefaultValue.Equals("N/A")) ? string.Equals(VarBIOSDefaultValue, "Enabled") ? "True" : "False" : string.Empty,
            _ => string.Empty
        };

        public override bool Equals(object? obj) => obj is Option { VarName: var varName, VarOffset: var varOffset, VarStoreId: var varStoreId, VarDescription: var varDescription }
        && (varName, varOffset, varStoreId, varDescription) == (VarName, VarOffset, VarStoreId, VarDescription);

        public override int GetHashCode() => (VarName, VarOffset, VarStoreId, VarDescription).GetHashCode();
    }
}
