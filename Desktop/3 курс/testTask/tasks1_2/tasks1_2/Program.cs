using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tasks1_2
{
    class Program    
    {
        static void Main(string[] args)
        {
            int n = 4, m = 5;
            int[,] array = new int[n, m];
            Random rand = new Random();
            int min=0, max=0,mainDiagSum=0,reverseDiagSum=0;
            for(int i=0;i<n;i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Console.Write($"{array[i, j] = rand.Next(10)}   ");//устанавливаем рандомные значения в ячейки
                    if(i==j)//главная диагональ
                    {
                        mainDiagSum += array[i, j];
                    }
                    if(i+j==m-1)//побочная диагональ
                    {
                        reverseDiagSum += array[i, j];
                    }

                    if(i==0 && j==0)//устанавливаем изначальные мин. и макс. значения 
                    {
                        min = array[i, j];
                        max = array[i, j];
                    }
                    else//ищем мин. и макс. значения
                    {
                        if (array[i, j] > max) { max = array[i, j]; }
                        else if (array[i, j] < min) { min = array[i, j]; }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine($"\nмаксимальный элемент: {max}  минимальный элемент: {min}");
            Console.WriteLine($"сумма главной диагонали: {mainDiagSum}  сумма побочной диагонали: {reverseDiagSum}");
            Console.ReadKey();
        }
    }
}
