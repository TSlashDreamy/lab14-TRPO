using System;
using System.Diagnostics;
using System.Threading.Tasks;
// lab 5 Сортування даних

namespace lab_14
{
    internal class Program
    {
        static void Main()
        {
            Stopwatch stopwatch = new Stopwatch();
            int threadsCount = 8;

            int[] numbers = { 36, 48, 45, 56, 1, -14, 4, -11, 6, 6, 18, 3, 25, 4, 47, 59 };

            FormatOutput("Unsorted array");
            foreach (var number in numbers)
            {
                Console.Write(number + " ");
            }

            FormatOutput(skipLine: true);
            FormatOutput();

            stopwatch.Start();
            int[] threadsArray = numbers;
            ParallelShellSort(threadsArray, threadsCount);
            stopwatch.Stop();

            double threadsElapsedTime = stopwatch.ElapsedMilliseconds;

            FormatOutput("Sorted array");
            foreach (var number in threadsArray)
            {
                Console.Write(number + " ");
            }

            FormatOutput(skipLine: true);
            FormatOutput();
            FormatOutput("Sorting details");
            Console.WriteLine($"Used {threadsCount} threads");
            Console.WriteLine($"Elapsed time: {threadsElapsedTime}ms");
            FormatOutput();
        }

        static void ParallelShellSort(int[] array, int threadsCount)
        {
            int length = array.Length;
            int chunkSize = (int)Math.Ceiling((double)length / threadsCount);

            // Додаємо бар'єр синхронізації, щоб гарантувати, що всі потоки завершать сортування перед продовженням
            var barrier = new Barrier(threadsCount, _ =>
            {
                Array.Sort(array); // Коли всі потоки завершили сортування своїх "шматків", ми відсортуємо весь масив
            });

            Parallel.For(0, threadsCount, threadIndex =>
            {
                int start = threadIndex * chunkSize;
                int end = Math.Min(start + chunkSize, length);

                for (int gap = end / 2; gap > 0; gap /= 2)
                {
                    for (int i = start + gap; i < end; i++)
                    {
                        int temp = array[i];
                        int j;

                        for (j = i; j >= start + gap && array[j - gap] > temp; j -= gap)
                        {
                            array[j] = array[j - gap];
                        }

                        array[j] = temp;
                    }
                }

                // Сигналізуємо бар'єру про завершення сортування для даного потоку
                barrier.SignalAndWait();
            });
        }

        private static void FormatOutput(string content = "", bool skipLine = false)
        {
            if (skipLine)
            {
                Console.WriteLine();
                return;
            }
            int windowWidth = Console.WindowWidth;

            int offset = Convert.ToBoolean(content.Length) ? 2 : 0;
            int totalLength = windowWidth - content.Length - offset;
            int halfLength = totalLength / 2;

            string line = new string('=', halfLength);
            if (Convert.ToBoolean(content.Length))
            {
                Console.WriteLine($"{line} {content} {line}");
            }
            else
            {
                Console.WriteLine($"{line}{line}");
            }
        }
    }
}
