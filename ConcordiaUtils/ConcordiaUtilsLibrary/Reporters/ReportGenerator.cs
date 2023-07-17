namespace ConcordiaUtilsLibrary.Reporters;

using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;

public class ReportGenerator
{
    private readonly ExperimentsGateway _experimentsGateway;
    private readonly ParticipantsGateway _participantsGateway;
    private readonly ScientistsGateway _scientistsGateway;
    private readonly StatesGateway _statesGateway;

    public ReportGenerator(
        ExperimentsGateway experimentsGateway,
        ParticipantsGateway participantsGateway,
        ScientistsGateway scientistsGateway,
        StatesGateway statesGateway)
    {
        _experimentsGateway = experimentsGateway;
        _participantsGateway = participantsGateway;
        _scientistsGateway = scientistsGateway;
        _statesGateway = statesGateway;
    }

    public string GenerateReport()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var reportFilePath = "ReportSincronizzazione.xls";
        using var package = new ExcelPackage(new FileInfo(reportFilePath));
        if (package.Workbook.Worksheets["Report Task"] != null)
        {
            ExcelWorksheet existingWorksheet = package.Workbook.Worksheets["Report Task"];
            package.Workbook.Worksheets.Delete(existingWorksheet);
        }
        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Report Task");
        var AllExperiments = _experimentsGateway.GetAll();
        var AllStates = _statesGateway.GetAll();
        var startStateId = AllStates.FirstOrDefault(x => x.Name == "Start")?.Id;
        var startStates = AllExperiments.Where(x => x.StateId == startStateId).Count();
        var workingStateId = AllStates.FirstOrDefault(x => x.Name == "Working")?.Id;
        var workingStates = AllExperiments.Where(x => x.StateId == workingStateId).Count();
        var finishStateId = AllStates.FirstOrDefault(x => x.Name == "Finish")?.Id;
        var finishStates = AllExperiments.Where(x => x.StateId == finishStateId).Count();
        worksheet.Cells["A1:D1"].Merge = true;
        worksheet.Cells["A1:D1"].Value = "TASKS REPORT";
        worksheet.Cells["A1:D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["A3:D3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        for (int i = 1; i <= 4; i++)
        {
            worksheet.Column(i).Width = 28; 
            worksheet.Cells[2, i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        worksheet.Cells["A2"].Value = "  STATES  ";
        worksheet.Cells["B2"].Value = "  START (DA INIZIARE)  ";
        worksheet.Cells["C2"].Value = "  WORKING (IN LAVORAZIONE)  ";
        worksheet.Cells["D2"].Value = "  FINISH (CONCLUSI)  ";
        worksheet.Cells["A3"].Value = "TASKS";
        worksheet.Cells["B3"].Value = startStates;
        worksheet.Cells["C3"].Value = workingStates;
        worksheet.Cells["D3"].Value = finishStates;
        var AllScientist = _scientistsGateway.GetAll().ToList();
        var AllPartecipant = _participantsGateway.GetAll();
        var countScientist = AllScientist.Count();
        List<(Scientist scientist, int finishExpCount, int TotalExpCount, double completedPercentage)> scientistsData = new();
        for (int i = 0; i < countScientist; i++)
        {
            var scientist = AllScientist[i];
            int startExpCount = AllPartecipant.Where(x => x.Experiment.StateId == startStateId && x.ScientistId == scientist.Id).Count();
            int workingExpCount = AllPartecipant.Where(x => x.Experiment.StateId == workingStateId && x.ScientistId == scientist.Id).Count();
            int finishExpCount = AllPartecipant.Where(x => x.Experiment.StateId == finishStateId && x.ScientistId == scientist.Id).Count();
            int TotalExpCount = startExpCount + workingExpCount + finishExpCount;
            double completedExpPercentage = TotalExpCount > 0 ? (double)finishExpCount / TotalExpCount * 100 : 0;
            scientistsData.Add((scientist, finishExpCount, TotalExpCount, completedExpPercentage));
        }
        scientistsData = scientistsData.OrderByDescending(x => x.completedPercentage).ToList();
        worksheet.Cells["A4:D4"].Merge = true;
        worksheet.Cells["A4:D4"].Value = "";
        worksheet.Cells["A4:D4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["A5:D5"].Merge = true;
        worksheet.Cells["A5:D5"].Value = "SCIENTIST REPORT";
        worksheet.Cells["A5:D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        worksheet.Cells["A6"].Value = "CODE";
        worksheet.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["B6"].Value = "FULL NAME";
        worksheet.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["C6"].Value = "DATA";
        worksheet.Cells["C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        worksheet.Cells["D6"].Value = "PERCENTAGE";
        worksheet.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        for (int i = 0; i < scientistsData.Count; i++)
        {
            int rowNumber = i + 7;
            var (scientist, finishExpCount, TotalExpCount, completedExpPercentage) = scientistsData[i];
            worksheet.Cells[$"A{rowNumber}"].Value = scientist.Code;
            worksheet.Cells[$"B{rowNumber}"].Value = scientist.FullName;
            worksheet.Cells[$"c{rowNumber}"].Value = "finished " + finishExpCount + " over " + TotalExpCount + " assigned.";
            if (TotalExpCount == 0)
            {
                worksheet.Cells[$"D{rowNumber}"].Value = "NULL";
            }
            else
            {
                worksheet.Cells[$"D{rowNumber}"].Value = completedExpPercentage + "%";
            }
            
            worksheet.Cells[$"D{rowNumber}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            string colorBackgroundCell = completedExpPercentage switch
            {
                double p when p >= 60 => "#008000",
                double p when p >= 30 => "#ffa500",
                double p when p==0 && TotalExpCount== 0 => "#808080",
                _ => "#FF0000",
            };
            worksheet.Cells[$"D{rowNumber}"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(colorBackgroundCell));
        }
        package.SaveAs(new FileInfo(reportFilePath));
        return reportFilePath;
    }
}