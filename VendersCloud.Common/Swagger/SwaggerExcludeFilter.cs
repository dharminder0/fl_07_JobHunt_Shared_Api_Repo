using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using VendersCloud.Common.Attributes;
using VendersCloud.Common.Helpers;

namespace VendersCloud.Common.Swagger
{
    public class SwaggerExcludeFilter : ISchemaFilter{
        public void Apply(OpenApiSchema schema, SchemaFilterContext schemaFilterContext) {
            if (schema?.Properties == null || schemaFilterContext.Type == null)
                return;

            
            var excludedProperties = schemaFilterContext.Type.GetProperties()
                .Where(t => 
                    t.GetCustomAttribute<SwaggerExcludeAttribute>() 
                    != null);

            foreach (var excludedProperty in excludedProperties) {
                if (schema.Properties.ContainsKey(excludedProperty.Name.ToCamelCase()))
                    schema.Properties.Remove(excludedProperty.Name.ToCamelCase());
            }
        }
        
    }
    
    public class OpenApiParameterIgnoreFilter :IOperationFilter {
        public void Apply(OpenApiOperation operation,OperationFilterContext context) {
            if (operation == null || context == null || context.ApiDescription?.ParameterDescriptions == null)
                return;

            var parametersToHide = context.ApiDescription.ParameterDescriptions
                .Where(parameterDescription => ParameterHasIgnoreAttribute(parameterDescription))
                .ToList();

            if (parametersToHide.Count == 0)
                return;

            foreach (var parameterToHide in parametersToHide) {
                var parameter = operation.Parameters.FirstOrDefault(parameter => string.Equals(parameter.Name, parameterToHide.Name, System.StringComparison.Ordinal));
                if (parameter != null)
                    operation.Parameters.Remove(parameter);
            }
        }

        private static bool ParameterHasIgnoreAttribute(ApiParameterDescription parameterDescription) {
            if (parameterDescription.ModelMetadata is DefaultModelMetadata metadata) {   
                return metadata.Attributes.Attributes != null &&
                       metadata.Attributes.Attributes.Any(attribute => attribute.GetType() == typeof(SwaggerExcludeAttribute));
                
            }

            return false;
        }
    }
}