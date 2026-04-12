using System.Linq.Expressions;

namespace Common.Specification;

public class ExpressionMapper<TDomain, TEntity> : ExpressionVisitor
{
    private ParameterExpression _parameter;
    private readonly Dictionary<string, string> _propertyMappings = new();
    
    public Expression<Func<TEntity, bool>> Map(Expression<Func<TDomain, bool>> domainExpression)
    {
        _parameter = Expression.Parameter(typeof(TEntity), domainExpression.Parameters[0].Name);
        var mappedBody = Visit(domainExpression.Body);
        return Expression.Lambda<Func<TEntity, bool>>(mappedBody, _parameter);
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node.Type == typeof(TDomain) ? _parameter : base.VisitParameter(node);
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression != null && node.Expression.Type == typeof(TDomain))
        {
            var propertyName = node.Member.Name;
            
            var targetPropertyName = _propertyMappings.TryGetValue(propertyName, out var mapped) 
                ? mapped 
                : propertyName;
            
            var member = typeof(TEntity).GetProperty(targetPropertyName, 
                System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);
                
            if (member == null) 
            {
                var availableProperties = string.Join(", ", 
                    typeof(TEntity).GetProperties().Select(p => p.Name));
                    
                throw new InvalidOperationException(
                    $"La propiedad '{propertyName}' del modelo de dominio '{typeof(TDomain).Name}' " +
                    $"no existe en el modelo de entidad '{typeof(TEntity).Name}'. " +
                    $"Propiedades disponibles: [{availableProperties}]"
                );
            }
                
            var targetExp = Visit(node.Expression);
            return Expression.Property(targetExp, member);
        }
        return base.VisitMember(node);
    }
    
    public void AddPropertyMapping(string domainProperty, string entityProperty) =>
        _propertyMappings[domainProperty] = entityProperty;
}

public static class IncludePathExtractor
{
    public static string GetPath<TDomain>(Expression<Func<TDomain, object>> expression)
    {
        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null && expression.Body is UnaryExpression unaryExpression)
        {
            memberExpression = unaryExpression.Operand as MemberExpression;
        }

        if (memberExpression == null)
            throw new ArgumentException("Expresión de Include inválida.");

        return memberExpression.Member.Name;
    }
}
