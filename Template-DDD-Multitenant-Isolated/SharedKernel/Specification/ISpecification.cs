using System.Linq.Expressions;

namespace SharedKernel.Specification;

public interface ISpecification<TDomain>
{
    Expression<Func<TDomain, bool>> Criteria { get; }
    List<Expression<Func<TDomain, object>>> Includes { get; }
    bool IsSatisfiedBy(TDomain domain);
}