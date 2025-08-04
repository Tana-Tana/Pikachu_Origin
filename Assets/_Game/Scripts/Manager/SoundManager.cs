using System;
using System.Collections;
using _Game.Extensions;
using _Game.Extensions.DP;
using UnityEngine;

public enum SoundID
{
    BG_CLASSIC = 0,
    BG_GAMEPLAY = 1,
}

public enum FxID
{
    BUTTON = 0,
    SELECTION_ITEM = 1,
    TIME_FREEZE = 2,
    TIME_OUT = 3,
    LEVEL_COMPLETE = 4,
    LOSE_GAME = 5,
    WIN_GAME = 6,
    MATCH = 7,
    CAN_NOT_MATCH = 8,
}


public class SoundManager : Singleton<SoundManager>
{
    //[SerializeField] UserData userData;

    private AudioSource soundSource;
    private AudioSource[] fxSource = new AudioSource[Utilities.GetEnumCount<FxID>()];

    [SerializeField] private AudioClip[] soundAus;
    [SerializeField] private AudioClip[] fxAus;

    private bool isLoaded = false;
    private int indexSound;
    private int soundStatus;
    private int fxStatus;

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);

        soundSource = gameObject.AddComponent<AudioSource>();
        soundSource.loop = true;
        soundSource.volume = 0.8f;
    }

    public void OnInit() 
    {
        Invoke(nameof(OnLoad), 1f);
        soundStatus = GamePrefs.GetSoundStatus();
        fxStatus = GamePrefs.GetFxStatus();

        SetSoundStatus(soundStatus);
        SetFxStatus(fxStatus);
    }


    public void PlaySound(SoundID ID)
    {
        soundSource.clip = soundAus[(int)ID];
        soundSource.Play();
    }

    public void PlayFx(FxID ID)
    {
        if (/*DataManager.GameJsonData.information.IsAmbientsSounds &&*/ isLoaded)
        {
            if (fxSource[(int)ID] == null)
            {
                fxSource[(int)ID] = new GameObject().AddComponent<AudioSource>();
                fxSource[(int)ID].clip = fxAus[(int)ID];
                fxSource[(int)ID].loop = false;
                fxSource[(int)ID].transform.SetParent(transform);
                if (ID == FxID.BUTTON || ID == FxID.SELECTION_ITEM) fxSource[(int)ID].volume = 0.6f;
                SetFxStatus(fxStatus);
            }
            fxSource[(int)ID].PlayOneShot(fxAus[(int)ID]);

            //Debug.Log(ID);
        }
    }

    public void ChangeSound(SoundID ID, float time)
    {
        StartCoroutine(IEChangeSound(ID, time));
    }

    #region Setters
    
    public void SetFxStatus(int status) {
        fxStatus = status;
        GamePrefs.SetFxStatus(fxStatus);
        if (fxStatus == 1)
        {
            UnMuteFx();
        }
        else
        {
            MuteFx();
        }
    }

    public void SetSoundStatus(int status) {
        soundStatus = status;
        GamePrefs.SetSoundStatus(soundStatus);

         if (soundStatus == 1)
        {
            UnMuteSound();
        }
        else
        {
            MuteSound();
        }
    }

    public void SetSoundVolume(float volume) {
        soundSource.volume = volume;
    }
#endregion
    #region Getters

    public int GetFxStatus => fxStatus;
    public int GetSoundStatus => soundStatus;

#endregion

    private void OnLoad()
    {
        if (soundAus.Length > 0)
        {
            isLoaded = true;

            //indexSound = Random.Range(0, soundAus.Length);
            indexSound = 0; // choi nhac classic
            PlaySound((SoundID)indexSound);
        }
    }

    private IEnumerator IEChangeSound(SoundID iD, float time)
    {
        yield return new WaitForSeconds(time);
        PlaySound(iD);
    }

    private void MuteSound()
    {
        soundSource.mute = true;
    }

    private void UnMuteSound()
    {
        soundSource.mute = false;
    }

    private void MuteFx()
    {
        foreach (AudioSource audioSource in fxSource)
        {
            if (audioSource != null)
            {
                audioSource.mute = true;

            }
        }
    }

    private void UnMuteFx()
    {
        foreach (AudioSource audioSource in fxSource)
        {
            if (audioSource != null)
            {
                audioSource.mute = false;
            }
        }
    }
}
