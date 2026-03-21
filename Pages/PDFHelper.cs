using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using sheep.Data;
using System.Collections.Generic;
using System.IO;

public static class PdfHelper
{
    public static void ExportSheepListToPdf(string outputPath, List<SheepEntity> sheeps)
    {
        if (sheeps == null || sheeps.Count == 0)
            throw new ArgumentException("出力する羊データがありません");

        using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // 日本語フォント
            BaseFont bf;
            try
            {
                // プロジェクト内 Fonts フォルダに msgothic.ttc を置く
                bf = BaseFont.CreateFont(
                    Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "msgothic.ttc") + ",0",
                    BaseFont.IDENTITY_H,
                    BaseFont.EMBEDDED
                );
            }
            catch
            {
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            }

            var font = new iTextSharp.text.Font(bf, 12);

            // 表を作る
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;

            // ヘッダー
            table.AddCell(new PdfPCell(new Phrase("名前", font)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("色", font)) { BackgroundColor = BaseColor.LIGHT_GRAY });

            // データ行
            foreach (var sheep in sheeps)
            {
                table.AddCell(new PdfPCell(new Phrase(sheep.Name ?? "-", font)));
                table.AddCell(new PdfPCell(new Phrase(sheep.Color ?? "-", font)));
            }

            doc.Add(table);
            doc.Close();
        }
    }
}