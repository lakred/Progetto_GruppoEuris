namespace ConcordiaUtilsLibrary;

using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary;

using ConcordiaTrelloLibrary;
using ConcordiaTrelloLibrary.Gateways;
using Microsoft.Extensions.Configuration;

using ConcordiaUtilsLibrary.Syncronizers;
using ConcordiaUtilsLibrary.Reporters;

public class Startup
{
    public async Task RunAsync()
    {
        string boardId = TrelloSettings.GetBoardCode();
        var trelloGateway = TrelloSettings.GetBoardGateway();
        var context = new ConcordiaContext();
        var experimentsGateway = new ExperimentsGateway(context);
        var participantsGateway = new ParticipantsGateway(context);
        var prioritiesGateway = new PrioritiesGateway(context);
        var remarksGateway = new RemarksGateway(context);
        var scientistsGateway = new ScientistsGateway(context);
        var statesGateway = new StatesGateway(context);
        var fromTrelloToDbSync = new FromTrelloToDbSync(trelloGateway, experimentsGateway, participantsGateway,
                                                        prioritiesGateway, remarksGateway,scientistsGateway,statesGateway);
        await fromTrelloToDbSync.SyncData(boardId);
        var fromDbToTrelloProgram = new FromDbToTrelloSync(trelloGateway, scientistsGateway, statesGateway, 
	                                                       experimentsGateway, remarksGateway);
        await fromDbToTrelloProgram.SyncFromDb(boardId);
        var reportGenerator = new ReportGenerator(experimentsGateway, participantsGateway, scientistsGateway, statesGateway);
        var reportSender = UtilsSettings.GetReportSender();
        await reportSender.SendReportAsync(reportGenerator.GenerateReport());
    }
}