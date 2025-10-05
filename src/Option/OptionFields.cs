namespace grubmod
{
    public record OptionFields(
         string OptionType,
         string VarName,
         string VarOffset,
         string VarStoreId,
         string VarSectionName,
         string VarSize,
         string VarDescription,
         string VarBIOSDefaultValue,
         List<string> VarValues
     );
}
