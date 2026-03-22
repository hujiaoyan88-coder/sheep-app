using System;
using System.IO;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using sheep.Data;
using System.Reflection;

public static class PdfHelper
{
    public static void ExportSheepListToPdf(string outputPath, List<SheepEntity> sheeps)
    {
        if (sheeps == null || sheeps.Count == 0)
            throw new ArgumentException("出力する羊データがありません");

        using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            Document doc = new Document(PageSize.A4, 36, 36, 36, 36); // マージン付き
            PdfWriter.GetInstance(doc, fs);
            doc.Open();
       

            // 日本語フォント（絶対に存在するフォントを使用）
            string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "msgothic.ttc");
            if (!File.Exists(fontPath))
                throw new FileNotFoundException("フォントファイルが見つかりません", fontPath);

            BaseFont bf = BaseFont.CreateFont(fontPath + ",0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            Font font = new Font(bf, 12);
            Font headerFont = new Font(bf, 12, Font.BOLD);

            // テーブル作成
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 1f });

            // ヘッダー
            table.AddCell(new PdfPCell(new Phrase("名前", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
            table.AddCell(new PdfPCell(new Phrase("色", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

            // データ行
            foreach (var sheep in sheeps)
            {
                string name = string.IsNullOrWhiteSpace(sheep.Name) ? "-" : sheep.Name;
                string color = string.IsNullOrWhiteSpace(sheep.Color) ? "-" : sheep.Color;

                table.AddCell(new PdfPCell(new Phrase(name, font)));
                table.AddCell(new PdfPCell(new Phrase(color, font)));
            }

            doc.Add(table);
            doc.Close();
        }
    }
}