using System.ComponentModel.DataAnnotations.Schema;
using Dapper;

namespace Workers.DataAccess.Extensions.DapperAttributeMapper;

public class ColumnAttributeTypeMapper<T>() : FallBackTypeMapper(new SqlMapper.ITypeMap[]
{
    new CustomPropertyTypeMap(typeof(T),
        (type, columnName) =>
            type.GetProperties().FirstOrDefault(prop =>
                prop.GetCustomAttributes(false)
                    .Where(a => a is ColumnAttribute || a is ForeignKeyAttribute)
                    .Any(attribute => attribute.GetType() == typeof(ColumnAttribute)
                        ? ((ColumnAttribute)attribute).Name == columnName
                        : ((ForeignKeyAttribute)attribute).Name == columnName)
            )
    ),
    new DefaultTypeMap(typeof(T))
});