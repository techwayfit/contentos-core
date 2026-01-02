namespace TechWayFit.ContentOS.Infrastructure.Persistence.Conventions;

/// <summary>
/// Database naming conventions (provider-agnostic)
/// </summary>
public static class NamingConventions
{
    /// <summary>
    /// Convert PascalCase to snake_case for database identifiers
    /// Example: ContentItemId → content_item_id
    /// </summary>
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        result.Append(char.ToLowerInvariant(input[0]));

        for (int i = 1; i < input.Length; i++)
        {
            char c = input[i];
            if (char.IsUpper(c))
            {
                result.Append('_');
                result.Append(char.ToLowerInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Standard table name suffix for Row entities
    /// </summary>
    public const string RowEntitySuffix = "Row";

    /// <summary>
    /// Convert entity name to table name
    /// Example: ContentItemRow → content_items
    /// </summary>
    public static string ToTableName(string entityName)
    {
        // Remove "Row" suffix
        if (entityName.EndsWith(RowEntitySuffix))
        {
            entityName = entityName.Substring(0, entityName.Length - RowEntitySuffix.Length);
        }

        // Pluralize (simple English pluralization)
        var snakeCase = ToSnakeCase(entityName);
        
        if (snakeCase.EndsWith("s") || snakeCase.EndsWith("x") || 
            snakeCase.EndsWith("ch") || snakeCase.EndsWith("sh"))
        {
            return snakeCase + "es";
        }
        else if (snakeCase.EndsWith("y") && snakeCase.Length > 1 && 
                 !IsVowel(snakeCase[snakeCase.Length - 2]))
        {
            return snakeCase.Substring(0, snakeCase.Length - 1) + "ies";
        }
        else
        {
            return snakeCase + "s";
        }
    }

    private static bool IsVowel(char c)
    {
        return "aeiou".Contains(char.ToLowerInvariant(c));
    }
}
