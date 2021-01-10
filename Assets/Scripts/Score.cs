using System;

public static class Score
{
    private static ulong _value;
    public static ulong Value
    {
        get => _value;
        set
        {
            _value = value;
            ScoreUpdated?.Invoke(_value);
        }
    }
    
    public static event Action<ulong> ScoreUpdated;
}