namespace SharedKernel.Ports.Out.Shared
{
    /// <summary>
    /// Abstrae el acceso al reloj del sistema para facilitar el testing unitario y la predicibilidad del tiempo.
    /// </summary>
    /// <remarks>
    /// Nota de diseño: En versiones modernas de .NET (8+), se recomienda utilizar la clase abstracta 
    /// <c>System.TimeProvider</c> nativa del framework en lugar de esta interfaz personalizada, 
    /// ya que ofrece capacidades superiores para el manejo de tiempo y tareas asíncronas en tests.
    /// </remarks>
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
    }
}
