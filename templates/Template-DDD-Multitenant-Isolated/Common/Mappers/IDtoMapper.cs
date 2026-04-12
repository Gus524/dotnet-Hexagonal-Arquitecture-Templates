namespace Common.Mappers;

public interface IDtoMapper<TSource, TDto>
{
    TDto Map(TSource source);
}