namespace Cocktails.Api.Application.Behaviors;

using Microsoft.OpenApi.Any;
using System.Text.Json;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class OpenApiExampleDocAttribute<T> : OpenApiUntypedExampleDocAttribute
{
    private readonly JsonNamingPolicy policy = JsonNamingPolicy.CamelCase;

    public OpenApiExampleDocAttribute(T example)
    {
        this.Example = example;
    }

    public OpenApiExampleDocAttribute(params T[] example)
    {
        this.ExampleArray = example;
    }

    public T Example { get; }

    public T[] ExampleArray { get; }

    public override IOpenApiAny GetExampleOpenApi() => this.GetArrayExampleOpenApi() ?? this.GetExampleOpenApiAny(this.Example);

    private OpenApiArray GetArrayExampleOpenApi()
    {
        if (this.ExampleArray == null)
        {
            return null;
        }

        var array = new OpenApiArray();
        array.AddRange(this.ExampleArray.Select(x => this.GetExampleOpenApiAny(x)));
        return array;
    }

    private IOpenApiAny GetExampleOpenApiAny(object value)
    {
        var type = value.GetType();

        if (type == typeof(float))
        {
            return new OpenApiFloat((float)value);
        }

        if (type == typeof(int))
        {
            return new OpenApiInteger((int)value);
        }

        if (type == typeof(bool))
        {
            return new OpenApiBoolean((bool)value);
        }

        if (type.IsEnum)
        {
            return new OpenApiString(this.policy.ConvertName(value.ToString()));
        }

        return new OpenApiString(value.ToString());
    }
}

public abstract class OpenApiUntypedExampleDocAttribute : Attribute
{
    public abstract IOpenApiAny GetExampleOpenApi();
}