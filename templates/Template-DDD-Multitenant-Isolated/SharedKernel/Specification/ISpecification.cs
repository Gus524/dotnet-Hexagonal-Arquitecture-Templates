namespace SharedKernel.Specification;

public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T aggregate);
}