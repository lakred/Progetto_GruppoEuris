namespace ConcordiaUtilsLibrary.Syncronizers;

using TrelloDotNet.Model;
using Microsoft.EntityFrameworkCore;

using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary.Gateways;
using ConcordiaDBLibrary.Gateways.Classes;

using ConcordiaTrelloLibrary.Models;
using ConcordiaTrelloLibrary.Gateways;

public class FromTrelloToDbSync
{
    private readonly TrelloGateway _trelloGateway;
    private readonly ExperimentsGateway _experimentsGateway;
    private readonly ParticipantsGateway _participantsGateway;
    private readonly PrioritiesGateway _prioritiesGateway;
    private readonly RemarksGateway _remarksGateway;
    private readonly ScientistsGateway _scientistsGateway;
    private readonly StatesGateway _statesGateway;

    public FromTrelloToDbSync(
        TrelloGateway trelloGateway,
        ExperimentsGateway experimentsGateway,
        ParticipantsGateway participantsGateway,
        PrioritiesGateway prioritiesGateway,
        RemarksGateway remarksGateway,
        ScientistsGateway scientistsGateway,
        StatesGateway statesGateway)
    {
        _trelloGateway = trelloGateway;
        _experimentsGateway = experimentsGateway;
        _participantsGateway = participantsGateway;
        _prioritiesGateway = prioritiesGateway;
        _remarksGateway = remarksGateway;
        _scientistsGateway = scientistsGateway;
        _statesGateway = statesGateway;
    }

    public async Task SyncData(string boardId)
    {
        var missingCardsInDb = await GetMissingCardsInDb(boardId);

        if (missingCardsInDb.Count > 0)
        {
            foreach (var cardId in missingCardsInDb)
            {
                var experimentData = await _trelloGateway.GetExperimentData(cardId);
                if (experimentData is not null)
                {
                    var insertedPriority = GetOrCreatePriority(experimentData.TPriority);
                    var insertedState = GetOrCreateState(experimentData.TState);
                    var experiment = new Experiment(null, experimentData.Code, experimentData.Name, experimentData.Description, true, experimentData.StartDate,
                        experimentData.DueDate, insertedPriority.Id.Value, insertedState.Id.Value);
                    var insertedExperiment = _experimentsGateway.Insert(experiment);
                    var scientistList = GetOrCreateScientists(experimentData.TScientists.ToList());
                    var remarkList = GetOrCreateRemarks(experimentData.TRemarks.ToList(), insertedExperiment.Id.Value);
                    var participantList = CreateParticipants(scientistList, insertedExperiment.Id.Value);
                    insertedExperiment.PriorityId = insertedPriority.Id.Value;
                    insertedExperiment.StateId = insertedState.Id.Value;
                    insertedExperiment.Participants = participantList;
                    insertedExperiment.Remarks = remarkList;
                    var updatedExperiment = _experimentsGateway.Update(insertedExperiment);
                    Console.WriteLine($"Esperimento inserito: id = {updatedExperiment.Id} - titolo = {updatedExperiment.Name}");
                }
            }
        }
        else
        {
            Console.WriteLine("I Dati nel Database sono già sincronizzati");
        }
    }

    private async Task<List<string>> GetMissingCardsInDb(string boardId)
    {
        var cardIdsInTrello = await _trelloGateway.GetCardIds(boardId);
        var existingCodesInDb = _experimentsGateway.GetAll().Select(n => n.Code).ToList();
        return cardIdsInTrello.Except(existingCodesInDb).ToList();
    }

    private Priority GetOrCreatePriority(TrelloPriority trelloPriority)
    {
        var existingPriority=_prioritiesGateway.GetByCode(trelloPriority.Code);
        if(existingPriority is null) 
        { 
            var newPriority=new Priority(null, trelloPriority.Code, trelloPriority.Name, trelloPriority.Color);
            return _prioritiesGateway.Insert(newPriority);
        }
        return existingPriority;
    }

    private State GetOrCreateState(TrelloState trelloState)
    {
        var existingState = _statesGateway.GetByCode(trelloState.Code);
        if(existingState is null)
        {
            var newState = new State(null, trelloState.Code, trelloState.Name);
            return _statesGateway.Insert(newState);
        }
        return existingState;
    }

    private List<Scientist> GetOrCreateScientists(List<TrelloScientist> trelloScientists)
    {
        var scientistList = new List<Scientist>();
        foreach (var scientist in trelloScientists)
        {
            var existingScientist = _scientistsGateway.GetByCode(scientist.Code);
            Scientist insertedScientist;
            if (existingScientist is null)
            {
                var newScientist = new Scientist(null, scientist.Code, scientist.FullName);
                insertedScientist = _scientistsGateway.Insert(newScientist);
            }
            else
            {
                insertedScientist = existingScientist;
            }
            scientistList.Add(insertedScientist);
        }
        return scientistList;
    }

    private List<Remark> GetOrCreateRemarks(List<TrelloRemark> trelloRemarks, int experimentId)
    {
        var remarkList = new List<Remark>();
        foreach (var remark in trelloRemarks)
        {
            if (!string.IsNullOrWhiteSpace(remark.Code))
            {
                var insertedScientist = GetOrCreateScientist(remark.TScientist);
                var newRemark = new Remark(null, remark.Code, remark.Text, remark.Date, experimentId, insertedScientist.Id.Value);
                var insertedRemark = _remarksGateway.Insert(newRemark);
                remarkList.Add(insertedRemark);
            }
        }
        return remarkList;
    }

    private Scientist GetOrCreateScientist(TrelloScientist trelloScientist)
    {
        var existingScientist=_scientistsGateway.GetByCode(trelloScientist.Code);
        if(existingScientist is null)
        {
            var newScientist = new Scientist(null,trelloScientist.Code,trelloScientist.FullName);
            return _scientistsGateway.Insert(newScientist);
        }
        return existingScientist;
    }

    private List<Participant> CreateParticipants(List<Scientist> scientistsList, int experimentId)
    {
        var participantList = new List<Participant>();
        foreach (var insertedScientist in scientistsList)
        {
            var participant = new Participant(null, experimentId, insertedScientist.Id.Value);
            var insertedParticipant = _participantsGateway.Insert(participant);
            participantList.Add(insertedParticipant);
        }
        return participantList;
    }
}