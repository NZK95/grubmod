namespace grubmod
{
    internal record OptionFields(
         string OptionType,
         string VarName,
         string VarOffset,
         string VarStoreId,
         string VarSectionName,
         string VarSize,
         string VarDescription,
         string VarBIOSDefaultValue,
         int VarIndex,
         List<string> VarValues
     );
}
