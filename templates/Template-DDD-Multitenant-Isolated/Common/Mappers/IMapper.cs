namespace Common.Mappers;

/// <summary>
/// Define el contrato estricto de la Capa Anticorrupción (ACL), forzando una barrera 
/// bidireccional entre los modelos de la infraestructura de datos y las entidades puras de negocio.
/// </summary>
/// <typeparam name="TDomain">El modelo conceptual aislado que encapsula las reglas de negocio.</typeparam>
/// <typeparam name="TPersistence">El modelo de datos estructurado y acoplado a la tecnología del ORM.</typeparam>
/// <remarks>
/// Diseñada para integrarse con generadores de código fuente (Source Generators como Mapperly). 
/// Traslada los errores de mapeo (desajustes de propiedades o tipos) del tiempo de ejecución (runtime) 
/// al tiempo de compilación (compile-time), asegurando que ninguna dependencia de infraestructura 
/// pueda ser persistida sin una política de traducción explícita y segura.
/// </remarks>
public interface IMapper<TDomain, TPersistence>
{
    TDomain MapToDomain(TPersistence persistence);
    TPersistence MapToPersistence(TDomain domain);
    void MapToExistingPersistence(TDomain domain, TPersistence persistence);
}