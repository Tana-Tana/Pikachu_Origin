using UnityEngine;

public static class GamePrefs {
    public const int SOUND_STATUS = 1;
    public const int FX_STATUS = 1;

    public static int GetSoundStatus() {
        return PlayerPrefs.GetInt(GameConfig.SOUND_KEY, SOUND_STATUS);
    }

    public static int GetFxStatus() {
        return PlayerPrefs.GetInt(GameConfig.FX_KEY, FX_STATUS);
    }

    public static void SetSoundStatus(int status) {
        PlayerPrefs.SetInt(GameConfig.SOUND_KEY, status);
    }

    public static void SetFxStatus(int status) {
        PlayerPrefs.SetInt(GameConfig.FX_KEY, status);
    }
}