namespace Workers.DataAccess.Extensions.DapperAttributeMapper;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ColumnAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}