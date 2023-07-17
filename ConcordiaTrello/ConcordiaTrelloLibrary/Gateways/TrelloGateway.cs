namespace ConcordiaTrelloLibrary.Gateways;

using ConcordiaTrelloLibrary.Models;
using ConcordiaTrelloLibrary.Services;

public class TrelloGateway
{
    private TrelloService _service;

    public TrelloGateway(string key, string token)
    {
        _service = new TrelloService(key, token);
    }

    public async Task<TrelloExperiment> GetExperimentData(string cardId)
    {
        var card = await _service._client.GetCardAsync(cardId);

        if (card == null)
        {
            throw new ArgumentException("Invalid cardId.");
        }
        var experiment = await _service.GetTaskDetails(card);

        return experiment;
    }

    public async Task UpdateCardListId(string cardId, string newListId)
    {
        await _service.ChangeCardListId(cardId, newListId);
    }

    public async Task AddCommentToCard(string cardId, string commentText)
    {
        await _service.AddCommentToCard(cardId, commentText);
    }

    public async Task<IEnumerable<string>> GetCardIds(string boardId)
    {
        var lists = await _service._client.GetListsOnBoardAsync(boardId);
        var cardIds = new List<string>();
        foreach (var list in lists)
        {
            var cards = await _service._client.GetCardsInListAsync(list.Id);
            cardIds.AddRange(cards.Select(card => card.Id));
        }
        return cardIds;
    }

    public async Task UpdateTrelloCard(TrelloExperiment trelloCard)
    {
        string cardId = trelloCard.Code;
        var card = await _service._client.GetCardAsync(cardId);

        if (trelloCard.TRemarks.Any(x => x != null))
        {
            var lastRemark = trelloCard.TRemarks
                .OrderByDescending(r => r.Date)
                .FirstOrDefault();

            bool remarkExist = await _service.RemarkExistInCard(cardId, lastRemark.Text, lastRemark.Date);

            if (string.IsNullOrEmpty(lastRemark.Code) && !remarkExist)
                await _service.AddCommentToCard(cardId, lastRemark.Text);
        }

        card.ListId = trelloCard.TState.Code;
        await _service._client.UpdateCardAsync(card);
    }
}