using System;
using System.Threading;

class Program
{
    static void Main()
    {
        TennisCourt court1 = new TennisCourt(1);
        TennisCourt court2 = new TennisCourt(2);

        for (int i = 1; i <= 14; i++)
        {
            Player player;
            if (i % 2 == 0)
                player = new ProfessionalPlayer($"Pro Player {i}");
            else
                player = new NovicePlayer($"Novice Player {i}");

            if (i == 14)
                player.PlayWithNovice(); 

            if (i % 2 == 0)
                court1.AddPlayer(player);
            else
                court2.AddPlayer(player);
        }

        Thread court1Thread = new Thread(court1.StartGame);
        Thread court2Thread = new Thread(court2.StartGame);

        court1Thread.Start();
        court2Thread.Start();

        court1Thread.Join();
        court2Thread.Join();
    }
}

class TennisCourt
{
    private readonly int courtNumber;
    private object locker = new object();

    public TennisCourt(int number)
    {
        courtNumber = number;
    }

    public void AddPlayer(Player player)
    {
        player.SetCourtNumber(courtNumber);
    }

    public void StartGame()
    {
        Console.WriteLine($"Game on Court {courtNumber} is starting.");

        foreach (Player player in Player.Players)
        {
            player.Start();
        }

        foreach (Player player in Player.Players)
        {
            player.Join();
        }

        Console.WriteLine($"Game on Court {courtNumber} is over.");
    }
}

abstract class Player
{
    private readonly string name;
    private Thread thread;
    private static readonly object playerLock = new object();
    private static readonly Random random = new Random();

    public static List<Player> Players { get; } = new List<Player>();

    public Player(string playerName)
    {
        name = playerName;
        thread = new Thread(Play);
        Players.Add(this);
    }

    public void Start()
    {
        thread.Start();
    }

    public void Join()
    {
        thread.Join();
    }

    public abstract void Play();

    public void PlayWithNovice()
    {
        Console.WriteLine($"{name} is willing to play with a Novice Player.");
    }

    public void SetCourtNumber(int courtNumber)
    {
        // Setting court number for visualization
        Console.WriteLine($"{name} is assigned to Court {courtNumber}.");
    }

    protected void WaitForOtherPlayers()
    {
        lock (playerLock)
        {
            Monitor.Pulse(playerLock);
            Monitor.Wait(playerLock);
        }
    }

    protected void PlayTennis()
    {
        Console.WriteLine($"{name} is playing tennis.");
        Thread.Sleep(random.Next(100, 1000));
    }
}

class NovicePlayer : Player
{
    public NovicePlayer(string name) : base(name) { }

    public override void Play()
    {
        WaitForOtherPlayers(); // Wait for other players to be ready
        PlayTennis();
    }
}

class ProfessionalPlayer : Player
{
    public ProfessionalPlayer(string name) : base(name) { }

    public override void Play()
    {
        WaitForOtherPlayers(); 
        PlayTennis();
    }
}

