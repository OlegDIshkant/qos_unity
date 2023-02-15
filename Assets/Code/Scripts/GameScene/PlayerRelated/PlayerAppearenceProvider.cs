using Assets.Code.Scripts.AppCore.CommonTools_submodule.CommonTools;
using CommonTools;
using System;
using System.Collections.Generic;


/// <summary>
/// √енерирует внешность дл€ игровых персонаж.
/// </summary>
public interface IPlayerAppearenceGeneration
{
    PlayerAppearence CreateNew();
}


/// <summary>
/// √енерирует внешность дл€ игровых персонаж в пор€дке очередности.
/// </summary>
public class PlayerAppearenceProvider : IPlayerAppearenceGeneration
{
    private IEnumerator<PlayerAppearence> _appearenceGenerator;


    public PlayerAppearenceProvider()
    {
        _appearenceGenerator = AllPossibleAppearences().IterateInCycle();
    }


    public PlayerAppearence CreateNew()
    {
        _appearenceGenerator.MoveNext();
        return _appearenceGenerator.Current;
    }


    private IEnumerable<PlayerAppearence> AllPossibleAppearences()
    {
        foreach (var t in Enum.GetValues(typeof(PlayerAppearence.Types)))
        {
            foreach (var c in Enum.GetValues(typeof(PlayerAppearence.Colors)))
            {
                yield return new PlayerAppearence((PlayerAppearence.Types)t, (PlayerAppearence.Colors)c);
            }
        }
    }
}


/// <summary>
/// √енерирует внешность дл€ игровых персонаж в случайном пор€дке.
/// </summary>
public class RndPlayerAppearenceProvider : IPlayerAppearenceGeneration
{
    private HashSet<PlayerAppearence> _generatedAppearences = new HashSet<PlayerAppearence>();
    private System.Random _rnd = new System.Random();



    public PlayerAppearence CreateNew()
    {
        for (int i = 0; i < 100; i++)
        {
            var rndAppearence = RandomAppearence();
            if (!WasGeneratedOnce(rndAppearence))
            {
                Remember(rndAppearence);
                return rndAppearence;
            }
        }

        throw new Exception("√енерируем слишком долго.");
    }


    private PlayerAppearence RandomAppearence()
    {
        var t = EnumExtensions.GetRandom<PlayerAppearence.Types>(_rnd);
        var c = EnumExtensions.GetRandom<PlayerAppearence.Colors>(_rnd);
        return new PlayerAppearence(t, c);
    }


    private bool WasGeneratedOnce(PlayerAppearence appearence) =>
        _generatedAppearences.Contains(appearence);


    private void Remember(PlayerAppearence appearence) =>
        _generatedAppearences.Add(appearence);

}


/// <summary>
/// ќписание внешности игрового персонажа.
/// </summary>
public struct PlayerAppearence
{
    public enum Types { ARMBEE, BIRD,  }
    public enum Colors { GREEN, YELOW, RED, BLUE }

    public Types Type { get; private set; }
    public Colors Color { get; private set; }


    public PlayerAppearence(Types type, Colors color)
    {
        Type = type;
        Color = color;
    }
}