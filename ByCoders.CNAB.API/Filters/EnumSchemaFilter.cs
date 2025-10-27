
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ByCoders.CNAB.API.Filters;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var value in Enum.GetValues(context.Type))
            {
                schema.Enum.Add(new OpenApiString(value.ToString()));
            }
            schema.Type = "string";
            schema.Format = null;
        }
    }
}