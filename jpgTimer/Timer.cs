using System.Diagnostics;
using System.Linq;

namespace jpgTimer;

public static class Timer
{
    static string[] notationArray = { "U", "D", "F", "B", "R", "L", "U'", "D'", "F'", "B'", "R'", "L'", "U2", "D2", "F2", "B2", "R2", "L2" };
    public static List<string> notation = new List<string>();
    public static List<TimeSpan> lastFive = new List<TimeSpan>();
    public static List<TimeSpan> totalTime = new List<TimeSpan>();
    private static string ao5;
    public static TimeSpan Average(this IEnumerable<TimeSpan> timeSpans)
    {
        IEnumerable<long> ticksPerTimeSpan = timeSpans.Select(t => t.Ticks);
        double averageTicks = ticksPerTimeSpan.Average();
        long averageTicksLong = Convert.ToInt64(averageTicks);
        TimeSpan averageTimeSpan = TimeSpan.FromTicks(averageTicksLong);
        return averageTimeSpan;
    }
    public static void initScramble()
    {
        List<string> scramble = new List<string>();
        Random rand = new Random();
        int index = 0;

        //populate list with array values for easier usability, probably doing this wrong
        foreach (string not in notationArray)
        {
            notation.Add(not);
        }

        //generate 20 moves from notation list
        for (int i = 0; i < 20; i++)
        {
            restart:
            index = rand.Next(notation.ToArray().Length);
            switch (i)
            {
                case 0:
                    scramble.Add(notation.ToArray()[index]);
                    break;
                case 1:
                    if (notation.ToArray()[index].Substring(0, 1) == scramble.ToArray()[i - 1].Substring(0, 1)){
                        goto restart;
                        //if new move equals old move, try again
                    }
                    scramble.Add(notation.ToArray()[index]);
                    break;
                default:
                    if (notation.ToArray()[index].Substring(0, 1) == scramble.ToArray()[i - 1].Substring(0, 1)
                        || notation.ToArray()[index].Substring(0, 1) == scramble.ToArray()[i - 2].Substring(0, 1)){
                        goto restart;
                        //if new move equals either previous 2 moves, try again
                    }
                    scramble.Add(notation.ToArray()[index]);
                    break;
            }
        }
        
        //foreach string in list of generated scramble, print to console
        scramble.ForEach(delegate(string scram)
        {
            Console.Write(scram + " ");
        });
        startTimer();
    }

    static void startTimer()
    {
        Stopwatch stopwatch = new Stopwatch();
        Console.WriteLine("\nPress Space to begin...");
        Console.ReadKey();
        stopwatch.Start();
        Console.ReadKey();
        stopwatch.Stop();
        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}",
            ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

        lastFive.Add(ts);
        if (lastFive.Count > 5)
        {
            lastFive.RemoveAt(0);
        }
        if (lastFive.Count == 5)
        {
            int minutes = ((lastFive.ToArray()[0].Minutes
                          + lastFive.ToArray()[1].Minutes
                          + lastFive.ToArray()[2].Minutes 
                          + lastFive.ToArray()[3].Minutes
                          + lastFive.ToArray()[4].Minutes) / 5);
            int seconds = ((lastFive.ToArray()[0].Seconds
                          + lastFive.ToArray()[1].Seconds
                          + lastFive.ToArray()[2].Seconds
                          + lastFive.ToArray()[3].Seconds
                          + lastFive.ToArray()[4].Seconds) / 5);
            int milliseconds = (((lastFive.ToArray()[0].Milliseconds
                           + lastFive.ToArray()[1].Milliseconds
                           + lastFive.ToArray()[2].Milliseconds
                           + lastFive.ToArray()[3].Milliseconds
                           + lastFive.ToArray()[4].Milliseconds) / 5) / 10);
            ao5 = String.Format("{0:00}:{1:00}.{2:00}",
                minutes, seconds,
                milliseconds);
        }
        
        totalTime.Add(ts);

        TimeSpan average = totalTime.Average();
        TimeSpan best = totalTime.Min();
        
        string averageTime = String.Format("{0:00}:{1:00}.{2:00}",
            average.Minutes, average.Seconds,
            average.Milliseconds / 10);
        string bestTime = String.Format("{0:00}:{1:00}.{2:00}",
            best.Minutes, best.Seconds,
            best.Milliseconds / 10);
        
        Console.WriteLine(elapsedTime);
        Console.WriteLine("Best: " + bestTime + "; Avg: " + averageTime + "; Ao5: " + ao5);
        using (StreamWriter sw = File.AppendText("times.txt"))
        {
            sw.WriteLine(elapsedTime);
        }
        using (StreamWriter sw = File.AppendText("avgs.txt"))
        {
            sw.WriteLine("Best: " + bestTime + "; Avg: " + averageTime + "; Ao5: " + ao5);
        }
        Console.WriteLine("Continue?");
        Console.ReadKey();
        Console.Clear();
        initScramble();

    }
}