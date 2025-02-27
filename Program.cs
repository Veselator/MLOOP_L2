using System.Runtime.InteropServices;
using System.Text; // Для utf-8

namespace MLOOP_L2
{
    internal class Program
    {
        // Кольори
        static string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
        static string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
        static string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
        static string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
        static string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
        static string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
        static string YELLOW = Console.IsOutputRedirected ? "" : "\x1b[93m";

        // Змінні для зміни розміра екрану
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        const uint SWP_NOZORDER = 0x0004;
        const uint SWP_NOSIZE = 0x0001;
        const int HWND_TOP = 0;
        
        // Змінні для маніпулюцій із розміром вікна
        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_SIZEBOX = 0x40000;

        static IntPtr handle = GetConsoleWindow();

        static Random rnd = new Random();

        static void PressAnyKeyToContinue()
        {
            Console.WriteLine(" Натисніть на будь-яку клавішу для продовження ");
            Console.ReadKey();
        }

        static void PrintTextFile(string FileName) // Просто вивід текстового файлу
        {
            using (StreamReader readtext = new StreamReader(FileName))
            {
                while (!readtext.EndOfStream)
                {
                    Console.WriteLine(readtext.ReadLine());
                }
            }
        }

        public static void DrawTextImage(int x, int y, string fileName) // Вивід текстового файлу по кооридантам
        {
            if (x >= Console.BufferWidth || y >= Console.BufferHeight)
            {
                return; // Навіщо малювати те, що повністю за межами екрану?
            }

            int currentY = y;
            int lineOffset = 0;

            if (y < 0)
            {
                lineOffset = -y;
                currentY = 0;
            }

            int lineCount = 0;

            using (StreamReader strReader = new StreamReader(fileName))
            {
                string line;

                while (lineCount < lineOffset && (line = strReader.ReadLine()) != null)
                {
                    lineCount++;
                }

                lineCount = 0;

                while ((line = strReader.ReadLine()) != null && currentY < Console.BufferHeight)
                {
                    int charOffset = 0;
                    int displayX = x;

                    if (x < 0)
                    {
                        charOffset = -x;
                        displayX = 0;
                    }

                    if (charOffset < line.Length)
                    {
                        int availableWidth = Console.BufferWidth - displayX;
                        string displayLine = line.Substring(charOffset);

                        if (displayLine.Length > availableWidth)
                        {
                            displayLine = displayLine.Substring(0, availableWidth);
                        }

                        Console.SetCursorPosition(displayX, currentY);
                        Console.Write(displayLine);
                    }

                    currentY++;
                    lineCount++;
                }
            }
        }

        static double GetTask1Number(double a, double x)
        {
            if (x - 1 == 0) { throw new ArgumentException(); }
            return Math.Pow((Math.Pow(x, 2) + a), (x * a) / (x - 1));
        }

        static void Task1()
        {
            double aStart = 0.5, da = 0.5, aMax = 2.0;
            double xStart = 0, xEnd = 0.8, dx = 0.5;
            double currentY;

            Console.WriteLine("\n Маємо наступну формулу:");
            PrintTextFile("formula1.txt");

            for (double currentA = aStart; currentA <= aMax; currentA += da)
            {
                Console.WriteLine($" a = {GREEN}{currentA}{NORMAL}:");
                for (double currentX = xStart; currentX < xEnd; currentX += dx)
                {
                    try
                    {
                        currentY = GetTask1Number(currentA, currentX);
                        Console.WriteLine($" Y({GREEN}{currentX, 4:f}{NORMAL}) = {GREEN}{currentY, 4:f}{NORMAL}");
                    }
                    catch
                    {
                        Console.WriteLine($" Y({GREEN}{currentX, 4:f}{NORMAL}) = {RED}неможливо порахувати{NORMAL}");
                    }
                }
                Console.WriteLine();
            }

            PressAnyKeyToContinue();
        }

        static void Task2()
        {
            Console.WriteLine("\n Маємо наступну формулу:");
            PrintTextFile("formula2.txt"); 
            Console.WriteLine("\n Формула обчислення i + 1 елемента суми:");
            PrintTextFile("formula3.txt");

            double sum; // Сума
            double i; // Номер ітерації
            double u; // Поточний елемент
            double[] possibleXs = { (2 * Math.PI) / 3, (8 * Math.PI) / 3, (28 * Math.PI) / 3 };
            double currentX = 2;
            double y = Math.Cos(currentX / 2);
            double difference;
            string outLine;

            for (int j = 0; j < possibleXs.Length; j++)
            {
                sum = 1 / Math.Sqrt(2);
                i = 1;
                u = 1;
                currentX = possibleXs[j];

                Console.WriteLine(" ----------------------------------------------------------------------");
                Console.WriteLine($" {GREEN}X = {currentX}{NORMAL}");
                Console.WriteLine(" ----------------------------------------------------------------------");

                while (Math.Abs(u) >= 1e-6)
                {
                    u *= (u * Math.Cos(Math.PI * (2 * (i + 1) + 1) / 4) * (currentX - Math.PI / 2)) / (2 * (i + 1) * Math.Cos(Math.PI * (2 * i + 1) / 4));
                    if (i > 100 || Math.Abs(u) == double.PositiveInfinity) { break; }
                    sum += u;
                    Console.WriteLine($" {GREEN}#{i}{NORMAL}:\t Член ряду = \t{GREEN}{u}{NORMAL}\n\t Сума = \t{GREEN}{sum}{NORMAL}");
                    i++;
                }
                difference = sum - y;
                outLine = difference > 0e-6 ? "Sum(x) > y(x)" : (difference < 0e-6 ? "Sum(x) < y(x)" : "Sum(x) == y(x)");
                Console.WriteLine($" Результат: {outLine} (різниця {difference})");
                Console.WriteLine(" ----------------------------------------------------------------------\n");
            }

            PressAnyKeyToContinue();
        }

        static int GetNumOfTotalIterations(int width, int height)
        {
            return 2 * (width > height ? width : height) - 1;
        }

        static void DrawStraightLine(int x1, int y1, int x2, int y2, char fillSymbol = '#', int delayTime = 1, int delayfrequency = 15)
        {
            int bufferWidth = Console.BufferWidth;
            int bufferHeight = Console.BufferHeight;

            int direction;
            if (x1 == x2)
            {
                direction = y2 - y1 > 0 ? 1 : -1;
                for (int i = y1; i != y2 + direction; i += direction)
                {
                    if (x1 >= 0 && x1 < bufferWidth && i >= 0 && i < bufferHeight)
                    {
                        Console.SetCursorPosition(x1, i);
                        Console.Write(fillSymbol);
                    }
                    if (i % delayfrequency == 0) { Thread.Sleep(delayTime); }
                }
                return;
            }
            if (y1 == y2)
            {
                direction = x2 - x1 > 0 ? 1 : -1;
                for (int i = x1; i != x2 + direction; i += direction)
                {
                    if (i >= 0 && i < bufferWidth && y1 >= 0 && y1 < bufferHeight)
                    {
                        Console.SetCursorPosition(i, y1);
                        Console.Write(fillSymbol);
                    }
                    if (i % delayfrequency == 0) { Thread.Sleep(delayTime); }
                }
                return;
            }
        }

        static void FillingAnimation(int width, int height, char fillingChar = '#')
        {
            Console.SetCursorPosition(0, 0);
            int pixelsNeedToFillX = width - 1, pixelsNeedToFillY = height - 1;
            int startX = 0, startY = 0, endX, endY;
            int curDirX, curDirY;
            int[] XDirs = { 1, 0, -1, 0 };
            int[] YDirs = { 0, 1, 0, -1 };

            int totalIterations = GetNumOfTotalIterations(width, height);

            for (int i = 0; i < totalIterations; i++)
            {
                curDirX = XDirs[i % 4];
                curDirY = YDirs[i % 4];

                endX = startX + curDirX * pixelsNeedToFillX;
                endY = startY + curDirY * pixelsNeedToFillY;
                DrawStraightLine(startX, startY, endX, endY, fillingChar, 1);

                startX = endX;
                startY = endY;
                if(curDirX != 0 && i > 1) { pixelsNeedToFillX -= 1; }
                if (curDirY != 0) { pixelsNeedToFillY -= 1; }
            }
        }

        static void ClearInputBuffer() // Вирішує проблему, коли користувач натискає Enter до того, як можна вводити значення
        {
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
        }

        static void WriteTextAt(int x, int y, string text) // Виводить текст на конкретних координатах
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(lines[i]);
            }
        }

        static void WriteTextAt(int x, int y, string text, ref int currentY) // Не тільки виводить текст на конкретних координатах, а й збільшує лічільник стрічок
        {
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                currentY += 1;
                Console.SetCursorPosition(x, y + i);
                Console.Write(lines[i]);
            }
        }

        static void Task3()
        {
            int currentWidth = 180, currentHeight = 60;

            int hp, maxHP = 6;
            int score = 0;
            int round = 1;
            string difficult = $"{GREEN}Легкий{NORMAL}";
            int secretNumber;
            int currentMin = 1, currentMax = 10;
            int userAnswerNumber;
            string userAnswerWord;
            int prevAnswer;
            bool playerWon = false;
            string isUserReadyForHard = "ні";

            int startY = 2;
            int currentY = startY;
            int textX = 100;

            int enemyX = 1;

            int currentEmotion = 0; // Керування емоціями опонента
            string[] emotions = { "enemy_idle.txt", "enemy_happy.txt", "enemy_angry.txt", "enemy_dominating.txt", "enemy_lose.txt" };

            SetWindowPos(handle, (IntPtr)HWND_TOP, 200, 0, 1900, 800, SWP_NOZORDER | SWP_NOSIZE);
            Console.CursorVisible = false;
            Console.SetWindowSize(currentWidth, currentHeight);
            Console.SetBufferSize(currentWidth, currentHeight);

            Console.Write(BLUE);
            FillingAnimation(currentWidth, currentHeight, '#');
            FillingAnimation(currentWidth, currentHeight, ' ');
            Console.Write(NORMAL);

            for (int x = -110; x <= enemyX; x++)
            {
                Console.Clear();
                DrawTextImage(x, 0, emotions[currentEmotion]);
                Thread.Sleep(10);
            }
            Console.SetCursorPosition(0, 0);
            
            void changeEmotion(int newEmotion)
            {
                currentEmotion = newEmotion;
                DrawTextImage(enemyX, 0, emotions[currentEmotion]);
            }

            void redrawScreen()
            {
                currentY = startY;
                Console.Clear();
                DrawTextImage(enemyX, 0, emotions[currentEmotion]);
            }

            WriteTextAt(textX, currentY, $" Привіт! Я {RED}Джеймс{NORMAL}. Я хотів би пограти в одну гру.\n\n", ref currentY);
            Thread.Sleep(4000);
            redrawScreen();
            while (true)
            {
                // Початок раунду
                prevAnswer = int.MinValue;
                secretNumber = rnd.Next(currentMin, currentMax); // Загадане число цього раунду
                hp = maxHP;
                changeEmotion(0);

                WriteTextAt(textX, currentY, $" {difficult} рівень складності, раунд №{GREEN}{round}{NORMAL}\n", ref currentY);
                WriteTextAt(textX, currentY, $" Я загадав число від {GREEN}{currentMin}{NORMAL} до {GREEN}{currentMax}{NORMAL}!\n Спробуй вгадати!\n В тебе залишилось всього {RED}{hp}{NORMAL} здоров'я!\n " +
                    $"Варіанти дії:\n    - написати {YELLOW}help{NORMAL} - порівняти моє число із твоїм останнім (-1 здоров'я);\n    - написати {YELLOW}число{NORMAL} - спробувати вгадати;\n    - написати {RED}capitulate{NORMAL} - здатись.", ref currentY);
                
                // Основна логіка раунду
                while (hp > 0)
                {
                    ClearInputBuffer();
                    WriteTextAt(textX, currentY, " > ", ref currentY);
                    userAnswerWord = Console.ReadLine();
                    if (int.TryParse(userAnswerWord, out userAnswerNumber))
                    {
                        // Користувач ввів число
                        if (userAnswerNumber == secretNumber)
                        {
                            changeEmotion(2);
                            WriteTextAt(textX, currentY, $" {GREEN}Ти вгадав!{NORMAL} Схоже, я погано загадав!", ref currentY);
                            Thread.Sleep(2000);
                            break;
                        }
                        else
                        {
                            changeEmotion(1);
                            prevAnswer = userAnswerNumber;
                            hp--;
                            WriteTextAt(textX, currentY, $" {RED}Неправильна відповідь!\n В тебе залишилось {hp} здоров'я!\n Ха-ха-ха!{NORMAL}", ref currentY);
                            if (userAnswerNumber < currentMin || userAnswerNumber > currentMax)
                            {
                                WriteTextAt(textX, currentY, $" {RED}{userAnswerNumber} навіть не входить в проміжок :/{NORMAL}", ref currentY);
                            }
                        }
                    }
                    else if (userAnswerWord == "help")
                    {
                        if (prevAnswer == int.MinValue)
                        {
                            changeEmotion(0);
                            WriteTextAt(textX, currentY, " Ти ще не вводив ніяких чисел :/", ref currentY);
                            continue;
                        }
                        if (hp == 1)
                        {
                            changeEmotion(0);
                            WriteTextAt(textX, currentY, " Подумай краще - в тебе всього одне здоров'я залишилось", ref currentY);
                            continue;
                        }
                        hp--;
                        WriteTextAt(textX, currentY, (prevAnswer > secretNumber ? " Твоє останнє число більше мого" : " Моє число більше твого останнього") + $"\n {RED}В тебе залишилось {hp} здоров'я!{NORMAL}", ref currentY);
                    }
                    else if (userAnswerWord == "capitulate")
                    {
                        //changeEmotion(3);
                        hp = 0;
                        break;
                    }
                    else
                    {
                        changeEmotion(0);
                        WriteTextAt(textX, currentY, $" Я не розумію тебе", ref currentY);
                    }

                    if (currentHeight - currentY < 4)
                    {
                        redrawScreen();
                    }
                }

                score += difficult == $"{GREEN}Легкий{NORMAL}" ? hp * 5 : hp * 10;
                // Раунд пройшов - або гравець вгадав правильно, або програв
                redrawScreen();
                round += 1;
                if (hp == 0)
                {
                    break;
                }

                if (round == 4 && difficult == $"{GREEN}Легкий{NORMAL}")
                {
                    changeEmotion(0);
                    WriteTextAt(textX, currentY, " Що ж, вітаю! Ти переміг мене на легкій складності.\n Але чи зможеш ти це зробити на складній? так/ні", ref currentY);
                    WriteTextAt(textX, currentY, " > ", ref currentY);
                    isUserReadyForHard = Console.ReadLine().ToLower();
                    if (isUserReadyForHard == "так")
                    {
                        round = 1;
                        difficult = $"{RED}Складний{NORMAL}";
                        maxHP = 25;
                        currentMin = 10;
                        currentMax = 100;
                        redrawScreen();
                    }
                    else
                    {
                        break;
                    }
                }

                if (round == 3 && difficult == $"{RED}Складний{NORMAL}")
                {
                    changeEmotion(2);
                    playerWon = true;
                    break;
                }
            }
            Thread.Sleep(1000);

            if (hp == 0) // Програш
            {
                Console.Write(RED);
                changeEmotion(3);
                Thread.Sleep(1000);
                FillingAnimation(currentWidth, currentHeight, 'X');
                ClearInputBuffer();
                Console.Write(NORMAL);
                WriteTextAt(25, 15," КІНЕЦЬ. ");
                WriteTextAt(25, 17, $" Очки: {score, 2:f} ");
                Console.SetCursorPosition(25, 20);

            } else if (round == 4) // Гравець вирішив не продовжувати після першого раунду
            {
                FillingAnimation(currentWidth, currentHeight, '/');
                ClearInputBuffer();
                WriteTextAt(25, 15, " ВІТАЮ, ПЕРЕМОЖЕЩЬ ЛЕГКОЇ СКЛАДНОСТІ! ");
                WriteTextAt(25, 17,$" Очки: {score, 2:f} ");
                Console.SetCursorPosition(25, 20);

            } else if (playerWon) // Гравець продовжив гру і переміг
            {
                changeEmotion(4);
                Thread.Sleep(1000);
                Console.Write(GREEN);
                FillingAnimation(currentWidth, currentHeight, '$');
                ClearInputBuffer();
                Console.Write(NORMAL);
                WriteTextAt(25, 15, " АБСОЛЮТНИЙ ПЕРЕМОЖЕЦЬ! ");
                WriteTextAt(25, 17, $" Очки: {score,2:f} ");
                Console.SetCursorPosition(25, 20);
            }

            PressAnyKeyToContinue();
            SetWindowPos(handle, (IntPtr)HWND_TOP, 10, 120, 1900, 800, SWP_NOZORDER);
            Console.Write(NORMAL);
            Console.BufferHeight = 500;
        }

        static void Setup()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(NORMAL);
            Console.OutputEncoding = Encoding.UTF8;
            SetWindowPos(handle, (IntPtr)HWND_TOP, 10, 120, 1900, 800, SWP_NOZORDER);
            Console.CursorVisible = false;
            int style = GetWindowLong(handle, GWL_STYLE);
            style &= ~(WS_MAXIMIZEBOX | WS_SIZEBOX);
            SetWindowLong(handle, GWL_STYLE, style);
        }

        static void PrintTitle(string date, int numOfLaboratory, string title)
        {
            Console.WriteLine($"\n " + date);
            Console.WriteLine($" Лабораторна робота №{numOfLaboratory}");
            Console.WriteLine($" Тема: {title}");
            Console.WriteLine(" Виконав Соломка Борис");
            Console.WriteLine(" №24");
        }

        static void Main(string[] args)
        {
            Setup();
            PrintTitle($"27.02.2025", 2, "Керуючі оператори організації циклів");

            bool isRunning = true;
            while (isRunning)
            {
                Console.Write($"\n Введіть відповідний номер:\n {UNDERLINE}0){NOUNDERLINE} Вихід;\n " +
                    $"{UNDERLINE}1){NOUNDERLINE} Завдання №2.1\n " +
                    $"{UNDERLINE}2){NOUNDERLINE} Завдання №2.2\n " +
                    $"{RED}{UNDERLINE}3){NOUNDERLINE} Завдання №2.3{NORMAL}\n > ");
                int userChoice;
                if (!int.TryParse(Console.ReadLine(), out userChoice)) { break; }
                if (userChoice != 3) { Console.Clear(); }

                switch (userChoice)
                {
                    case 0:
                        isRunning = false;
                        break;
                    case 1:
                        Task1();
                        break;
                    case 2:
                        Task2();
                        break;
                    case 3:
                        Task3();
                        break;
                    default:
                        Console.WriteLine(" Введено некоректне число.");
                        break;
                }
                Console.Clear();
            }
        }
    }
}
