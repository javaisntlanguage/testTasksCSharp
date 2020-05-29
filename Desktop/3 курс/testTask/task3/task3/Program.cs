using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace task3
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = "table.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path)) //читаем файл
                {
                    string line;
                    List<dynamic[]> textList = new List<dynamic[]>();


                    int i = 0;
                    //проходим по каждой строке файла
                    while ((line = sr.ReadLine()) != null)
                    {
                        //разделяем строку на слова с удалением пустых строк
                        string[] words = line.Split(new string[] { "\t\t", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                        //если строка не пустая
                        if (words.Length != 0) {
                            dynamic[] wordsAnyType = new dynamic[words.Length];
                            //проходим по словам
                            for(int j=0;j<words.Length;j++)
                            {
                                //задаем правильный тип и заменяем символы '_' на пробел
                                wordsAnyType[j] = Parser.TryParse(words[j].Replace('_',' '));
                                //если не заголовок и не номер, то выводим с типом
                                if (i != 0 && j != 0) 
                                { 
                                    Console.Write($"{wordsAnyType[j]} - {wordsAnyType[j].GetType().Name} "); 
                                }
                                else//иначе выводим без типа
                                {
                                    Console.Write($"{wordsAnyType[j]} ");
                                }

                            }
                            Console.WriteLine();
                            textList.Add(wordsAnyType); 
                        }
                        i++;
                    }
                    //сортируем по номеру с помощью LINQ и лямбда функции
                    var sortedTable =  textList.GetRange(1,textList.Count-1).OrderBy(c => (long)c[0]).ToList();
                    textList.RemoveRange(1, textList.Count - 1);
                    textList.AddRange(sortedTable);

                    //выводим отсортированную таблицу
                    Console.WriteLine("\nотсортированная таблица\n");
                    foreach(dynamic str in textList)
                    {
                        foreach(dynamic word in str)
                        {
                            Console.Write($"{word} ");
                        }
                        Console.WriteLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
