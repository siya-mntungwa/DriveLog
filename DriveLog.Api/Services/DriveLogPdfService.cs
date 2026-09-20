using DriveLog.Api.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DriveLog.Api.Services;

public class DriveLogPdfService
{
    public byte[] GenerateReport(
        string reportTitle,
        string driverName,
        string employeeId,
        string vehicleInfo,
        List<DriveLogDto> driveLogs)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("DriveLog")
                            .FontSize(24)
                            .Bold();

                        column.Item()
                            .Text(reportTitle)
                            .FontSize(16)
                            .SemiBold();

                        column.Item()
                            .Text($"Generated: {DateTime.Now:dd MMMM yyyy HH:mm}");
                    });

                page.Content()
                    .PaddingTop(20)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text($"Driver: {driverName}");

                        column.Item()
                            .Text($"Employee ID: {employeeId}");

                        column.Item()
                            .Text($"Vehicle: {vehicleInfo}");

                        column.Item()
                            .Text($"Total completed drives: {driveLogs.Count}");

                        column.Item()
                            .PaddingTop(15)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.2f);
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(HeaderStyle).Text("Start");
                                    header.Cell().Element(HeaderStyle).Text("End");
                                    header.Cell().Element(HeaderStyle).Text("Start Location");
                                    header.Cell().Element(HeaderStyle).Text("End Location");
                                    header.Cell().Element(HeaderStyle).Text("Purpose");
                                    header.Cell().Element(HeaderStyle).Text("Duration");
                                });

                                foreach (var drive in driveLogs)
                                {
                                    var duration = drive.EndTime.HasValue
                                        ? drive.EndTime.Value - drive.StartTime
                                        : TimeSpan.Zero;

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(drive.StartTime.ToLocalTime()
                                            .ToString("dd/MM/yyyy HH:mm"));

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(drive.EndTime?.ToLocalTime()
                                            .ToString("dd/MM/yyyy HH:mm") ?? "-");

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(drive.StartLocation);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(drive.EndLocation ?? "-");

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(drive.Purpose);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text($"{(int)duration.TotalHours}h {duration.Minutes}m");
                                }
                            });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("DriveLog • Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer HeaderStyle(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Padding(5);
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
    }
}