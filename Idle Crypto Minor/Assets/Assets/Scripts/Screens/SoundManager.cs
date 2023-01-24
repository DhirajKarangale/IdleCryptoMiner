using UnityEngine.UI;
using UnityEngine;

public class SoundManager : ScreenBase
{
    [Header("Sound Effect")]
    [SerializeField] Slider slideSoundEffect;
    [SerializeField] AudioSource soundEffect;
    private const string saveSoundEffect = "SoundEffect";

    [Header("Clip")]
    [SerializeField] internal AudioClip clipTap;
    [SerializeField] internal AudioClip clipCoin;
    [SerializeField] internal AudioClip clipGift;
    [SerializeField] internal AudioClip clipMeta;
    [SerializeField] internal AudioClip clipRoom;
    [SerializeField] internal AudioClip clipHardware;

    [Header("Background Music")]
    [SerializeField] Slider sliderBackgroundMusic;
    [SerializeField] AudioSource backgroundMusic;
    private const string saveBackgroundMusic = "BackGroundMusic";

    public static SoundManager instance = null;
    public static SoundManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);


        slideSoundEffect.value = PlayerPrefs.GetFloat(saveSoundEffect, 0.5f);
        sliderBackgroundMusic.value = PlayerPrefs.GetFloat(saveBackgroundMusic, 0.5f);
    }

    public void PlaySound(AudioClip audioClip)
    {
        soundEffect.PlayOneShot(audioClip);
    }

    public void SoundEffectSlider(float value)
    {
        slideSoundEffect.value = value;
        soundEffect.volume = value;
        PlayerPrefs.SetFloat(saveSoundEffect, value);
    }

    public void BackGroundMusicSlider(float value)
    {
        sliderBackgroundMusic.value = value;
        backgroundMusic.volume = value;
        PlayerPrefs.SetFloat(saveBackgroundMusic, value);
    }
}
