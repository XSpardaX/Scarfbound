using System;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public const int WispsPerLife = 5;
    public const int StartingLives = 3;

    public int Wisps { get; private set; }
    public int Lives { get; private set; } = StartingLives;

    public event Action<int> OnWispsChanged;
    public event Action<int> OnLivesChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        GameObject gameStateObject = new GameObject("GameState");
        Instance = gameStateObject.AddComponent<GameState>();
        DontDestroyOnLoad(gameStateObject);
    }

    public void AddWisps(int amount)
    {
        if (amount <= 0) return;

        Wisps += amount;

        while (Wisps >= WispsPerLife)
        {
            Wisps -= WispsPerLife;
            Lives++;

            if (OnLivesChanged != null)
            {
                OnLivesChanged.Invoke(Lives);
            }
        }

        if (OnWispsChanged != null)
        {
            OnWispsChanged.Invoke(Wisps);
        }
    }

    public void SetWisps(int value)
    {
        Wisps = Mathf.Max(0, value);

        if (OnWispsChanged != null)
        {
            OnWispsChanged.Invoke(Wisps);
        }
    }

    public void LoseLife()
    {
        Lives = Mathf.Max(0, Lives - 1);

        if (OnLivesChanged != null)
        {
            OnLivesChanged.Invoke(Lives);
        }
    }

    public void ResetForNewGame()
    {
        Wisps = 0;
        Lives = StartingLives;

        if (OnWispsChanged != null)
        {
            OnWispsChanged.Invoke(Wisps);
        }

        if (OnLivesChanged != null)
        {
            OnLivesChanged.Invoke(Lives);
        }
    }
}
