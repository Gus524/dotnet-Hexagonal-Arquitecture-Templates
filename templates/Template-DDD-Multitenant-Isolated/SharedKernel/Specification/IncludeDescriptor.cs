using System.Linq.Expressions;

namespace SharedKernel.Specification;

public sealed class IncludeDescriptor<T>
{
    public string RelationName { get; }
    public Expression<Func<T, bool>>? ChildFilter { get; }
    
    public IncludeDescriptor(string relationName, Expression<Func<T, bool>>? childFilter = null)
    {
        ArgumentNullException.ThrowIfNull(relationName);
        RelationName = relationName;
        ChildFilter = childFilter;
    }
}