namespace ConcordiaDBLibrary.Gateways.Abstract;

using Models.Abstract;

public interface ITrelloEntityGateway<TTrelloEntity> : IEntityGateway<TTrelloEntity> where TTrelloEntity : TrelloEntity
{
    public TTrelloEntity? GetByCode(int code);
    public IEnumerable<TTrelloEntity>? GetByCodeMulti(IEnumerable<string>? codes);
}