
using TestApp;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TradeApp
{

    internal class Program
    {
        static List<Trade> list;
        static Trade trade;
        static FileSystemWatcher watcher = new FileSystemWatcher();
        static bool isWatching = true;
        static string csvFile = @"C:/temp/Trades.csv";

        static void Main(string[] args)
        {
            watcher = new FileSystemWatcher();
            list = new List<Trade>();

            ExecuteFileOperation();
            while (true)
            {
                if (isWatching)
                {
                    WatchFile();
                }
                System.Threading.Thread.Sleep(20000);
            }

            Console.ReadLine();
        }

        private static void ExecuteFileOperation()
        {
            try
            {
                if (File.Exists(csvFile))
                {
                    list.Clear();
                    Console.WriteLine("Reading csv file");
                    string[,] values = LoadCSV(csvFile);
                    int num_rows = values.GetUpperBound(0) + 1;

                    for (int r = 1; r < num_rows; r++)
                    {
                        trade = new Trade(values[r, 0], values[r, 2], values[r, 2]);
                        list.Add(trade);
                    }

                    Console.WriteLine("Saving data to database");
                    ITradesRepository tradesRepository = new TradesRepository(new TradesContext());
                    tradesRepository.DeleteAll();
                    foreach (var item in list)
                    {
                        Console.WriteLine(item.TradeID + "\t" + item.ISIN + "\t" + item.Notional + "\t");
                        tradesRepository.AddTrade(item);
                    }

                    MoveFile();
                }
                else
                {
                    Console.WriteLine("Waiting for csv file");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
                Console.WriteLine("Error while accessing file.. Trying again...");
                System.Threading.Thread.Sleep(30000);
                ExecuteFileOperation();
            }
        }

        private static void WatchFile()
        {
            watcher.Path = @"C:/temp";
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "Trades.csv";

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);

            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            isWatching = false;

            WatcherChangeTypes wct = e.ChangeType;
            Console.WriteLine("File dropped");
            if (wct.ToString().ToLower() == "created")
            {
                StopWatchFile();
                ExecuteFileOperation();
                Console.WriteLine("Waiting for csv file");
                StartWatchFile();
            }
        }

        private static void StopWatchFile()
        {
            watcher.EnableRaisingEvents = false;

            watcher.Changed -=
               new FileSystemEventHandler(OnChanged);
            watcher.Dispose();
        }

        private static void StartWatchFile()
        {
            watcher = new FileSystemWatcher();
            isWatching = true;
        }

        private static string[,] LoadCSV(string filename)
        {
            string whole_file;
            string[] lines;
            int num_rows;
            int num_cols;
            string[,] values;
            using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    whole_file = reader.ReadToEnd();
                    whole_file = whole_file.Replace('\n', '\r');
                    lines = whole_file.Split(new char[] { '\r' },
                        StringSplitOptions.RemoveEmptyEntries);

                    num_rows = lines.Length;
                    num_cols = lines[0].Split(',').Length;

                    values = new string[num_rows, num_cols];

                    for (int r = 0; r < num_rows; r++)
                    {
                        string[] line_r = lines[r].Split(',');
                        for (int c = 0; c < num_cols; c++)
                        {
                            values[r, c] = line_r[c];
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
                fileStream.Close();
                fileStream.Dispose();
            }

            //string whole_file = System.IO.File.ReadAllText(filename);

            //whole_file = whole_file.Replace('\n', '\r');
            //string[] lines = whole_file.Split(new char[] { '\r' },
            //    StringSplitOptions.RemoveEmptyEntries);

            //int num_rows = lines.Length;
            //int num_cols = lines[0].Split(',').Length;

            //string[,] values = new string[num_rows, num_cols];

            //for (int r = 0; r < num_rows; r++)
            //{
            //    string[] line_r = lines[r].Split(',');
            //    for (int c = 0; c < num_cols; c++)
            //    {
            //        values[r, c] = line_r[c];
            //    }
            //}

            return values;
        }

        private static void MoveFile()
        {
            string timeSpan = DateTime.Now.ToString().Replace("/", "_");
            timeSpan = timeSpan.Replace(":", "-");

            string sourceFilePath = @"C:/temp/Trades.csv"; ;
            string destinationFilePath = @$"C:/temp/archive/Trades{timeSpan}.csv";
            Console.WriteLine("File Moved");
            File.Move(sourceFilePath, destinationFilePath, true);
        }

    }
}
