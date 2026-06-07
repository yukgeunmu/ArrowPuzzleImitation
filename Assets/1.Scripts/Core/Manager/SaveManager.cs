using UnityEngine;

public static class SaveManager
{
    private const string CurrentStageKey = "CurrentStage";

    public static int CurrentStage
    {
        get => PlayerPrefs.GetInt(CurrentStageKey, 0);

        set
        {
            PlayerPrefs.SetInt(CurrentStageKey, value);
            PlayerPrefs.Save();
        }
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(CurrentStageKey);
    }
}