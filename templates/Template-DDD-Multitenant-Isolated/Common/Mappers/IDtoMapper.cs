namespace Common.Mappers;

public interface IDtoMapper<in TPersistence, out TDto>
{
    TDto Map(TPersistence persistence);
}