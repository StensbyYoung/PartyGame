using System;

namespace TestGame;

public class Functions
{
    private static readonly Random getrandom = new ();

    public static int GetRandomNumber(int min, int max)
    {
        lock(getrandom)
        {
            return getrandom.Next(min, max);
        }
    }
}