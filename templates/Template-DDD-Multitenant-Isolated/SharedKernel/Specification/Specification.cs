using System.Linq.Expressions;

namespace SharedKernel.Specification;

public abstract class Specification<TDomain> : ISpecification<TDomain>
{
    public Expression<Func<TDomain, bool>> Criteria { get; private set; }
    public List<Expression<Func<TDomain, object>>> Includes { get; } = [];
    
    public bool IsSatisfiedBy(TDomain domain)
    {
        Func<TDomain, bool> predicate = Criteria.Compile();
        return predicate(domain);
    }

    protected Specification(Expression<Func<TDomain, bool>> criteria) => Criteria = criteria;

    protected void AddCriteria(Expression<Func<TDomain, bool>> additionalCriteria)
    {
        var parameter = Expression.Parameter(typeof(TDomain));
        
        var currentBody = new ParameterReplacer(parameter).Visit(Criteria.Body);
        var additionalBody = new ParameterReplacer(parameter).Visit(additionalCriteria.Body);
        
        var combinedBody = Expression.AndAlso(currentBody, additionalBody);
        Criteria = Expression.Lambda<Func<TDomain, bool>>(combinedBody, parameter);
    }

    protected void AddInclude(Expression<Func<TDomain, object>> include) => Includes.Add(include);
}

public class ParameterReplacer : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    public ParameterReplacer(ParameterExpression parameter) => _parameter = parameter;

    protected override Expression VisitParameter(ParameterExpression node) =>
        node.Type == _parameter.Type ? _parameter : base.VisitParameter(node);
}