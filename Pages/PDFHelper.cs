using System;
using System.Collections.Generic;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using sheep.Data;

public static class PdfHelper
{
    public static void ExportSheepListToPdf(string outputPath, List<SheepEntity> sheeps)
    {
        using var pdfDoc = new PdfDocument(new PdfWriter(outputPath));
        var document = new Document(pdfDoc);

        // 日本語フォント
        var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "msgothic.ttc");
        PdfFont font = PdfFontFactory.CreateFont(fontPath + ",0", PdfEncodings.IDENTITY_H);

        document.SetFont(font);

        // 表作成（列2つ）
        Table table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();

        // ヘッダーセルは Bold フォントを指定
        table.AddHeaderCell(new Cell().Add(new Paragraph("名前").SetFont(font)));
        table.AddHeaderCell(new Cell().Add(new Paragraph("色").SetFont(font)));

        // データ行
        foreach (var sheep in sheeps)
        {
            table.AddCell(new Cell().Add(new Paragraph(sheep.Name).SetFont(font)));
            table.AddCell(new Cell().Add(new Paragraph(sheep.Color).SetFont(font)));
        }

        document.Add(table);
        document.Close();
    }
}