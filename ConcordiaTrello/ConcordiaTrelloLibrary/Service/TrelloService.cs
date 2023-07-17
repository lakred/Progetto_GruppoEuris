namespace ConcordiaTrelloLibrary.Services;

using ConcordiaTrelloLibrary.Models;
using TrelloDotNet;

public class TrelloService
{
    internal TrelloClient _client;
    private HttpClient _httpClient;

    public TrelloService(string key, string token)
    {
        _client = new TrelloClient(key, token);
        _httpClient = new HttpClient();
    }

    internal async Task<TrelloExperiment> GetTaskDetails(TrelloDotNet.Model.Card card)
    {
        TrelloPriority tPriority = null;
        if (card.Labels != null && card.Labels.Count > 0)
        {
            tPriority = new TrelloPriority(
                 card.Labels[0].Id,
                card.Labels[0].Name,
                card.Labels[0].Color);
        }
        var lists = await GetLists(card.BoardId);
        string stateName = lists.FirstOrDefault(list => list.Id == card.ListId).Name;
        var tState = new TrelloState(card.ListId, stateName);
        var tScientists = await GetScientistsForCardAsync(card.Id);
        var lastComment = await GetLastCommentAsync(card.Id);
        var remarks = new List<TrelloRemark> { lastComment };
        var experiment = new TrelloExperiment(
            card.Id,
            card.Name,
            card.Description,
            card.Start,
            card.Due,
            tPriority,
            tState,
            tScientists ?? new List<TrelloScientist>(),
            remarks ?? new List<TrelloRemark>()
        );
        return experiment;
    }

    public async Task<IEnumerable<(string Id, string Name)>> GetLists(string boardId)
    {
        var lists = await _client.GetListsOnBoardAsync(boardId);
        var listIds = lists.Select(list => (list.Id, list.Name));
        return listIds;
    }

    public async Task<IEnumerable<string>> GetCardIds(string boardId)
    {
        var lists = await _client.GetListsOnBoardAsync(boardId);
        var cardIds = new List<string>();
        foreach (var list in lists)
        {
            var cards = await _client.GetCardsInListAsync(list.Id);
            cardIds.AddRange(cards.Select(card => card.Id));
        }
        return cardIds;
    }

    private async Task<TrelloRemark?> GetLastCommentAsync(string cardId)
    {
        var cardActions = await _client.GetActionsOnCardAsync(cardId);
        var comments = cardActions
            .Where(a => a.Type == "commentCard")
            .OrderByDescending(a => a.Date)
            .ToList();
        if (comments.Count > 0)
        {
            var lastComment = comments[0];
            var scientist = new TrelloScientist(
                lastComment.MemberCreatorId,
                lastComment.MemberCreator.FullName);
            return new TrelloRemark(
                lastComment.Id,
                lastComment.Data.Text,
                lastComment.Date,
                scientist);
        }
         return new TrelloRemark(default, string.Empty, default, null);
    }

    private async Task<IEnumerable<TrelloScientist>> GetScientistsForCardAsync(string cardId)
    {
        var members = await _client.GetMembersOfCardAsync(cardId);
        var scientists = members
            .Select(member => new TrelloScientist(member.Id, member.FullName));
        return scientists;
    }

    public async Task AddCommentToCard(string cardId, string commentText)
    {
        var comment = new TrelloDotNet.Model.Comment { Text = commentText };
        if(commentText != string.Empty)
        {
        await _client.AddCommentAsync(cardId, comment);
        Console.WriteLine("Comment added successfully.");
        }
    }

    public async Task ChangeCardListId(string cardId, string newListId)
    {
        var card = await _client.GetCardAsync(cardId);
        if (card != null)
        {
            card.ListId = newListId;
            await _client.UpdateCardAsync(card);
            Console.WriteLine("Card list ID changed successfully.");
        }
        else
        {
            Console.WriteLine("Card not found.");
        }
    }

    public async Task<bool> RemarkExistInCard(string cardId, string commentText, DateTimeOffset? commentDate)
    {
        var remarks = await _client.GetAllCommentsOnCardAsync(cardId);
        return remarks.Any(remark => remark.Data.Text == commentText && remark.Date == commentDate);
    }
}