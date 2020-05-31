using System;
using System.Collections.Generic;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Globalization;

namespace task4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Считываем таблицу");
            //открываем excel для чтения
            string path = Path.Combine(Directory.GetCurrentDirectory(),"ФайлСИсходнымиДанными.xls");
            var ObjExcel = new Excel.Application();
            Excel.Workbook workBook = ObjExcel.Workbooks.Open(path, Type.Missing, true, Type.Missing, Type.Missing,
                                                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                              Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                                              Type.Missing, Type.Missing);
            Excel.Worksheet sheet = (Excel.Worksheet)workBook.Sheets[1];
            //получаем последнюю ячейку
            Excel.Range last = sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);
            //получаем все ячейки 
            var range = sheet.get_Range("A1", last).Cells;
            //получаем заголовки
            string[] titles = new string[] {range[1,1].Value,
                                            $"{range[2, 3].Value} {range[1, 3].Value.ToLower()}",
                                            $"{range[2, 4].Value} {range[1, 3].Value.ToLower()}",
                                            $"{range[2, 5].Value} {range[1, 5].Value.ToLower()}",
                                            $"{range[2, 6].Value} {range[1, 5].Value.ToLower()}"
            };

            Console.WriteLine("Создаем шапку XML");
            var document = new XDocument(new XDeclaration("1.0", "windows-1251",null),
               new XElement("RootXml",
                    new XElement("SchemaVersion",
                        new XAttribute("Number", "2"),
                        new XElement("Period",
                            new XAttribute("Date", "2014-02-06"),
                            new XElement("Source",
                                new XAttribute("ClassCode", "ДМС"),
                                new XAttribute("Code", "819"),
                                new XElement("Form",
                                    new XAttribute("Code","178"),
                                    new XAttribute("Name", "Счета в кредитных организациях"),
                                    new XAttribute("Status","0")
                                )
                            )
                        )

                    )
                  
               )
            );
            Console.WriteLine("заполняем XML");
            for (int i = 0; i < titles.Length; i++)
            {
                document.XPathSelectElement("./RootXml/SchemaVersion/Period/Source/Form").Add(
                    new XElement("Column",
                        new XAttribute("Num", (i + 1).ToString()),
                        new XAttribute("Name", titles[i])
                    )
                );
            }
            var secondColumnDistinct = new List<string>();
            var secondColumn = new List<string>();
            var SumColumns = new Dictionary<string, double[]>();
            var culture = CultureInfo.CreateSpecificCulture("en-US");
            for (int i=5;i<=last.Row;i++)
            {
                //записываем номер строки в трехзначном формате
                string billAcc = $"1{range[i, 2].Value.ToString().Substring(0, range[i, 2].Value.ToString().Length - 3)}000";
                if (!secondColumnDistinct.Contains(billAcc))
                {
                    secondColumnDistinct.Add(billAcc);
                    document.XPathSelectElement("./RootXml/SchemaVersion/Period/Source/Form").Add(
                    new XElement("Document",
                    //ко второму столбцу добавляем единицу в начало и обнуляем последние 3 разряда числа
                        new XAttribute("ПлСч11", billAcc)
                    ));
                }
                secondColumn.Add(billAcc);
                string billAccCount = secondColumn.Count(c => c == billAcc).ToString("D3");
                document.XPathSelectElement($"./RootXml/SchemaVersion/Period/Source/Form/Document[@ПлСч11={billAcc}]").Add(
                    new XElement("Data",
                        new XAttribute("СТРОКА", billAccCount)
                    )
                );
                if(!SumColumns.ContainsKey(billAcc))
                {
                    SumColumns[billAcc] = new double[4];
                }
                for (int j = 1; j <= last.Column; j++)
                {

                    if (j != 2)
                    {
                        if (j >= 3)
                        {
                            SumColumns[billAcc][j-3] += range[i, j].Value;
                            document.XPathSelectElement($"./RootXml/SchemaVersion/Period/Source/Form/" +
                                $"Document[@ПлСч11={billAcc}]/" +
                                $"Data[@СТРОКА='{billAccCount}']").Add(
                        new XElement("Px",
                            new XAttribute("Num", j - 1),
                            new XAttribute("Value", range[i, j].Value.ToString("F",culture))
                            )
                        );
                        }
                        else
                        {
                            document.XPathSelectElement($"./RootXml/SchemaVersion/Period/Source/Form/" +
                                $"Document[@ПлСч11={billAcc}]/" +
                                $"Data[@СТРОКА='{billAccCount}']").Add(
                            new XElement("Px",
                                new XAttribute("Num", j),
                                new XAttribute("Value", range[i, j].Value.ToString())
                                )
                            );
                        }
                    }
                }
            }
            //добавляем итоговые суммы числовых столбцов
            foreach (var billAcc in secondColumnDistinct)
            {
                document.XPathSelectElement($"./RootXml/SchemaVersion/Period/Source/Form/" +
                    $"Document[@ПлСч11={billAcc}]").Add(
                    new XElement("Data",
                        new XAttribute("СТРОКА", "960")
                    )
                );
                for(int j=0;j<4;j++)
                {
                    document.XPathSelectElement($"./RootXml/SchemaVersion/Period/Source/Form/" +
                    $"Document[@ПлСч11={billAcc}]/Data[@СТРОКА=960]").Add(
                        new XElement("Px",
                                new XAttribute("Num", j+2),
                                new XAttribute("Value", SumColumns[billAcc][j].ToString("F", culture))
                                )
                    );
                }
            }
            Console.WriteLine("Сохраняем XML и закрываем EXCEL");
            workBook.Close(false, Type.Missing, Type.Missing);
            ObjExcel.Quit();
            document.Save(Path.Combine(Directory.GetCurrentDirectory(), "ФайлРезультат.xml"));
            Console.ReadKey();
        }
    }
}
