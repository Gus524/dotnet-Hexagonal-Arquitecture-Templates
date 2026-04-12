namespace SharedKernel.Ports.Out.Repository;

public interface IDbContextInitializer
{
    Task InitializeAsync();
}