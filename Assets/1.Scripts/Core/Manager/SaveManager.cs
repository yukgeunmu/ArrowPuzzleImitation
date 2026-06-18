using UnityEngine;

public class SaveManager
{
    public void SaveBestRecord(Difficulty difficulty, float clearTime)
    {
        string key = $"BEST_TIME_{difficulty}";

        float bestTime =  PlayerPrefs.GetFloat( key, float.MaxValue);

        if (clearTime < bestTime)
        {
            PlayerPrefs.SetFloat(key, clearTime);

            PlayerPrefs.Save();
        }
    }

    public float GetBestRecord(Difficulty difficulty)
    {
        string key = $"BEST_TIME_{difficulty}";

        return PlayerPrefs.GetFloat(key, -1f);
    }
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Expert,
    Master
}