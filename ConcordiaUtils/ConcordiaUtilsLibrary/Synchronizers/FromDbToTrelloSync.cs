namespace ConcordiaUtilsLibrary.Syncronizers;

using TrelloDotNet.Model;
using Microsoft.EntityFrameworkCore;

using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary.Gateways;
using ConcordiaDBLibrary.Gateways.Classes;

using ConcordiaTrelloLibrary.Models;
using ConcordiaTrelloLibrary.Gateways;

public class FromDbToTrelloSync
{

    private readonly TrelloGateway _gateway;
    private readonly ScientistsGateway _scientists;
    private readonly StatesGateway _states;
    private readonly ExperimentsGateway _experiments;
    private readonly RemarksGateway _remarks;
    public FromDbToTrelloSync( 
        TrelloGateway gateway, 
        ScientistsGateway scientists, 
        StatesGateway states, 
        ExperimentsGateway experiments, 
        RemarksGateway remarks)
    {
          _gateway = gateway;
          _scientists = scientists;
          _states = states;
          _experiments = experiments;   
          _remarks = remarks;
    }


    public async Task SyncFromDb(string boardId)
    {
        var cardsId = await _gateway.GetCardIds(boardId);
        var allExperiments = _experiments.GetAll();
        var allRemarks = _remarks.GetAll();


        foreach (var cardId in cardsId)
        {
            var experiment = allExperiments.FirstOrDefault(e => e.Code == cardId);

            if (experiment != null)
            {
                var trelloCard = await _gateway.GetExperimentData(cardId);
                var lastRemark = allRemarks
                    .Where(r => r.ExperimentId == experiment.Id)
                    .OrderByDescending(r => r.Date)
                    .FirstOrDefault();

                if (lastRemark != null)
                {
                    var lastScientistId = lastRemark.ScientistId;
                    var lastScientistFromDb = _scientists.GetById(lastScientistId);
                    var lastScientist = new TrelloScientist(lastScientistFromDb.Code, lastScientistFromDb.FullName);
                    var newRemark = new TrelloRemark(default, lastRemark.Text, lastRemark.Date, lastScientist);

                    var lastCommentOnCard = trelloCard.TRemarks
                        .OrderByDescending(r => r.Date)
                        .FirstOrDefault();

                    if (lastCommentOnCard?.Text != newRemark.Text)
                    {
                        trelloCard.TRemarks.Add(newRemark);
                    }
                    else
                    {
                        Console.WriteLine($"Nessun commento da aggiornare nella card {cardId}");
                    }
                }

                var lastState = experiment.StateId;
                var getState = _states.GetById(lastState);
                trelloCard.TState.Code = getState.Code;

                await _gateway.UpdateTrelloCard(trelloCard);
            }
        }
    }

}