using System;

public static class Score
{
    private static ulong _value;
    public static ulong Value => _value;

    public static void Add(ulong val)
    {
        _value += val;
        ScoreChanged();
    }

    public static void Reset()
    {
        _value = 0;
        ScoreChanged();
    }

    private static void ScoreChanged()
    {
        ScoreUpdated?.Invoke(_value);
    }

    public static event Action<ulong> ScoreUpdated;
}