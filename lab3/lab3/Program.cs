//#define SEQ

using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Labs
{
    public class SearchForPrimeNumbers
    {
        #region Работа с простыми числами

        /// <summary>
        /// Первое простое число
        /// </summary>
        private const int beginPrimeNumber = 2;
        /// <summary>
        /// Значение, указывающее, что число не является простым
        /// </summary>
        private const int notPrimeNumberValue = -1;

        /// <summary>
        /// Представляет простое число
        /// </summary>
        private struct PrimeNumber
        {
            /// <summary>
            /// Значение простого числа
            /// </summary>
            public int Value;
            /// <summary>
            /// Является ли число базовым простым
            /// </summary>
            public bool IsBase;
            /// <summary>
            /// Является ли число простым
            /// </summary>
            public bool IsPrime => Value != notPrimeNumberValue;

            public PrimeNumber(int value)
            {
                Value = value;
                IsBase = false;
            }

            public override string ToString() => IsPrime ? Value.ToString() : null;

            public static implicit operator int(PrimeNumber primeNumber) => primeNumber.Value;
            public static implicit operator PrimeNumber(int value) => new PrimeNumber(value);
        }
        /// <summary>
        /// Все простые числа
        /// </summary>
        private class PrimeNumbers
        {
            private readonly int n;

            private readonly PrimeNumber[] primeNumbers;
            /// <summary>
            /// Получает или задает значение простого числа
            /// </summary>
            public PrimeNumber this[int primeNumber]
            {
                get => primeNumbers[primeNumber - beginPrimeNumber];
                set => primeNumbers[primeNumber - beginPrimeNumber] = value;
            }
            /// <summary>
            /// Получает массив базовых простых чисел
            /// </summary>
            public int[] GetBasePrimeNumbers() => primeNumbers.Where(prime => prime.IsBase).Select(basePrime => basePrime.Value).ToArray();
            /// <summary>
            /// Отмечает требуемое простое число как базовое простое
            /// </summary>
            /// <param name="primeNumber">Простое число, которое является базовым простым</param>
            private void SetPrimeIsBase(int primeNumber)
            {
                primeNumbers[primeNumber - beginPrimeNumber].IsBase = true;
            }

            /// <summary>
            /// Создает обертку для работы с массивом простых чисел
            /// </summary>
            /// <param name="n">Общее количество чисел</param>
            public PrimeNumbers(int n)
            {
                primeNumbers = new PrimeNumber[n - beginPrimeNumber + 1];

                this.n = n;
            }
            /// <summary>
            /// Заполнение массива числами от 2 до N
            /// </summary>
            public void Init()
            {
                for (int i = beginPrimeNumber; i <= n; i++)
                {
                    this[i] = i;
                }
            }

            /// <summary>
            /// Преобразование массива в читаемую строку
            /// </summary>
            /// <param name="showBasePrimeNumbers">Если true, то в строку заносятся только базовые простые числа, иначе - все простые числа</param>
            public string ToString(bool showBasePrimeNumbers)
            {
                int newLineCounter = 0;

                StringBuilder sb = new StringBuilder();
                foreach (PrimeNumber primeNumber in primeNumbers)
                {
                    if (primeNumber.IsPrime)
                    {
                        if (showBasePrimeNumbers && !primeNumber.IsBase)
                        {
                            continue;
                        }

                        if (newLineCounter % 10 == 0)
                        {
                            newLineCounter = 0;
                            sb.AppendLine().Append("\t");
                        }
                        sb.AppendFormat("{0,5}", primeNumber.ToString());

                        newLineCounter++;
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// Реализация последовательного алгоритма Эратосфена
            /// </summary>
            /// <param name="upperBound">Верхняя граница алгоритма поиска простых чисел</param>
            public void Eratosthenes(int upperBound)
            {
                for (int p = beginPrimeNumber; p <= upperBound; p++)
                {
                    if (this[p].IsPrime)
                    {
                        OtherPrimeNumbersChecking(p, p, upperBound);
                        SetPrimeIsBase(p);
                    }
                }
            }
            /// <summary>
            /// Проверка на разложимость по базовым простым числам
            /// </summary>
            /// <param name="primeBegin">Простое число, с которого начинается проверка</param>
            /// <param name="primeEnd">Простое число, на котором проверка будет закончена (включительно)</param>
            /// <param name="otherBegin">Нижняя граница диапазона чисел для проверки (не включительно)</param>
            /// <param name="otherEnd">Верхняя граница диапазона чисел для проверки (включительно)</param>
            public void DecompositionEratosthenes(int primeBegin, int primeEnd, int otherBegin, int otherEnd)
            {
                for (int p = primeBegin; p <= primeEnd; p++)
                {
                    if (this[p].IsBase)
                    {
                        OtherPrimeNumbersChecking(p, otherBegin, otherEnd);
                    }
                }
            }
            /// <summary>
            /// Выполнение проверки диапазона чисел на разложимость по простому числу <paramref name="currentPrime"/>
            /// </summary>
            /// <param name="currentPrime">Текущее простое число</param>
            /// <param name="beginCheckingPrime">Нижняя граница проверки (не включительно)</param>
            /// <param name="upperBound">Верхняя граница проверки (включительно)</param>
            private void OtherPrimeNumbersChecking(int currentPrime, int beginCheckingPrime, int upperBound)
            {
                for (int otherPrime = beginCheckingPrime + 1; otherPrime <= upperBound; otherPrime++)
                {
                    if (this[otherPrime].IsPrime)
                    {
                        this[otherPrime] = (otherPrime % currentPrime != 0 ? otherPrime : notPrimeNumberValue);
                    }
                }
            }
        }

        #endregion

        public int SeqId => 2;

        private int n, m;
        private Thread[] threads;
        /// <summary>
        /// Массив простых чисел
        /// </summary>
        private PrimeNumbers primeNumbers;

        /// <summary>
        /// Граница для применения модифицированного алгоритма Эратосфена
        /// </summary>
        private int SqrtN => (int)Math.Sqrt(n);

        private int processingTraceCounter;

        public void Run(string[] args)
        {
            //n = args.GetNumberFrom(0, 100000);
            //m = args.GetNumberFrom(1, Environment.ProcessorCount);
            n = 100;
            m = 4;
            Console.WriteLine($"NumbersCount: {n}{Environment.NewLine}ThreadsCount: {m}{Environment.NewLine}");

            threads = new Thread[m];
            primeNumbers = new PrimeNumbers(n);
#if SEQ
			Processing(FullEratosthenes, false);
#endif
            Processing(ModifiedEratosthenes);
            Processing(DataDecompositionEratosthenes);
            Processing(BasePrimeDecompositionEratosthenes);
            Processing(EveryBasePrimeDecompositionEratosthenes);
            Processing(ParallelSerialHandleBasePrimeDecompositionEratosthenes);
        }

        /// <summary>
        /// Выполнение действия <paramref name="action"/> и вывод на экран затраченного времени
        /// </summary>
        /// <param name="showBasePrimeNumbers">Вывод базовых простых чисел</param>
        private void Processing(Action action, bool showBasePrimeNumbers = true)
        {
            processingTraceCounter++;

            // Сброс массива от предыдущей итерации
            primeNumbers.Init();

            Stopwatch sw = Stopwatch.StartNew();

            action();

            sw.Stop();

            //Console.WriteLine($"{processingTraceCounter.ToString()} -->\tPrime Numbers: {primeNumbers.ToString(false)}");
            if (showBasePrimeNumbers)
            {
                Console.WriteLine($"{processingTraceCounter.ToString()} -->\tBase Prime Numbers: {primeNumbers.ToString(true)}");
            }

            Console.WriteLine($"{processingTraceCounter.ToString()} -->\t[{action.Method.Name}] Wasted {sw.Elapsed.ToString()}{Environment.NewLine}");
        }
        /// <summary>
        /// Проверка на разложимость по базовым простым числам
        /// </summary>
        /// <param name="primeBegin">Простое число, с которого начинается проверка</param>
        /// <param name="primeEnd">Простое число, на котором проверка будет закончена (включительно)</param>
        /// <param name="otherBegin">Нижняя граница диапазона чисел для проверки (не включительно)</param>
        /// <param name="otherEnd">Верхняя граница диапазона чисел для проверки (включительно)</param>
        private void DecompositionEratosthenes(
            int primeBegin = beginPrimeNumber,
            int? primeEnd = default,
            int? otherBegin = default,
            int? otherEnd = default)
        {
            primeNumbers.DecompositionEratosthenes(primeBegin, primeEnd ?? SqrtN, otherBegin ?? SqrtN, otherEnd ?? n);
        }
        /// <summary>
        /// Получает массив базовых простых чисел
        /// </summary>
        private int[] GetBasePrimeNumbers()
        {
            int[] basePrimes = primeNumbers.GetBasePrimeNumbers();
            //Console.WriteLine($"BasePrimeNumbersCount {basePrimes.Length}");

            return basePrimes;
        }

        /// <summary>
        /// Последовательный алгоритм Эратосфена
        /// </summary>
        private void FullEratosthenes()
        {
            primeNumbers.Eratosthenes(n);
        }
        /// <summary>
        /// Модифицированный алгоритм Эратосфена
        /// </summary>
        private void ModifiedEratosthenes()
        {
            primeNumbers.Eratosthenes(SqrtN);
            DecompositionEratosthenes();
        }
        /// <summary>
        /// Параллельный алгоритм №1: декомпозиция по данным
        /// </summary>
        private void DataDecompositionEratosthenes()
        {
            primeNumbers.Eratosthenes(SqrtN);

            double count = (n - SqrtN) / (double)m;
            for (int i = 0; i < m; i++)
            {
                int startIndex = SqrtN + (int)Math.Round(count * i);
                int finalIndex = SqrtN + (int)Math.Round(count * (i + 1));

                //Console.WriteLine($"Thread {i}) {count} {startIndex} {finalIndex}");

                threads[i] = new Thread(() =>
                {
                    DecompositionEratosthenes
                    (
                        otherBegin: startIndex,
                        otherEnd: finalIndex
                    );
                });
                threads[i].Start();
            }
            Array.ForEach(threads, thread => thread.Join());
        }
        /// <summary>
        /// Параллельный алгоритм №2: декомпозиция набора простых чисел
        /// </summary>
        private void BasePrimeDecompositionEratosthenes()
        {
            primeNumbers.Eratosthenes(SqrtN);

            int[] basePrimes = GetBasePrimeNumbers();
            double count = basePrimes.Length / (double)m;
            for (int i = 0; i < m; i++)
            {
                int startPrimeIndex = (int)Math.Round(count * i);
                int finalPrimeIndex = (int)Math.Round(count * (i + 1));

                //Console.WriteLine($"Thread {i}) {count} {startPrimeIndex} {finalPrimeIndex}");

                threads[i] = new Thread(() =>
                {
                    for (int k = startPrimeIndex; k < finalPrimeIndex; k++)
                    {
                        int prime = basePrimes[k];
                        DecompositionEratosthenes
                        (
                            primeBegin: prime,
                            primeEnd: prime
                        );
                    }
                });
                threads[i].Start();
            }
            Array.ForEach(threads, thread => thread.Join());
        }
        /// <summary>
        /// Параллельный алгоритм №3: применение пула потоков
        /// </summary>
        private void EveryBasePrimeDecompositionEratosthenes()
        {
            primeNumbers.Eratosthenes(SqrtN);

            int[] basePrimes = GetBasePrimeNumbers();
            using (CountdownEvent wait = new CountdownEvent(basePrimes.Length))
            {
                for (int i = 0; i < basePrimes.Length; i++)
                {
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        int idx = (int)param;

                        DecompositionEratosthenes
                        (
                            primeBegin: basePrimes[idx],
                            primeEnd: basePrimes[idx]
                        );

                        wait.Signal();
                    },
                    i);
                }
                wait.Wait();
            }
        }
        /// <summary>
        /// Параллельный алгоритм №4: последовательный перебор простых чисел
        /// </summary>
        private void ParallelSerialHandleBasePrimeDecompositionEratosthenes()
        {
            primeNumbers.Eratosthenes(SqrtN);

            int[] basePrimes = GetBasePrimeNumbers();

            int currentAvailableBasePrimeIndex = 0;
            for (int i = 0; i < m; i++)
            {
                threads[i] = new Thread(idx =>
                {
                    int index = (int)idx;

                    while (true)
                    {
                        int currentBasePrimeIndex = Interlocked.Increment(ref currentAvailableBasePrimeIndex) - 1;

                        if (currentBasePrimeIndex >= basePrimes.Length)
                        {
                            //Console.WriteLine($"Thread {index} is done");
                            break;
                        }
                        else
                        {
                            //Console.WriteLine($"Thread {index}) {currentBasePrimeIndex} {basePrimes[currentBasePrimeIndex]}");
                        }

                        DecompositionEratosthenes
                        (
                            primeBegin: basePrimes[currentBasePrimeIndex],
                            primeEnd: basePrimes[currentBasePrimeIndex]
                        );

                    }
                });
                threads[i].Start(i);
            }
            Array.ForEach(threads, thread => thread.Join());
        }
        static void Main(string[] args)
        {
            SearchForPrimeNumbers searchForPrimeNumbers = new SearchForPrimeNumbers();
            searchForPrimeNumbers.Run(args);
        }
    }
}
