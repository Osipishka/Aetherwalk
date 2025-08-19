using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Настройки")]
    public string managerTag = "SoundManager";

    [Header("Аудио источники")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Звуки игры")]
    [SerializeField] private AudioClip GravitySound;
    [SerializeField] private AudioClip DiamondInteractionSound;
    [SerializeField] private AudioClip DiamondWinSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip ClickSound;
    [SerializeField] private AudioClip skinSelectSound;

    private static AudioManager instance;

    private bool musicEnabled = true;
    private bool soundsEnabled = true;

    private List<Button> musicOnButtons = new List<Button>();
    private List<Button> musicOffButtons = new List<Button>();
    private List<Button> soundsOnButtons = new List<Button>();
    private List<Button> soundsOffButtons = new List<Button>();

    public static AudioManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (!gameObject.CompareTag(managerTag))
            {
                gameObject.tag = managerTag;
            }

            SetMusicActive(true);
            SetSoundsActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        SetMusicActive(musicEnabled);
        PlayButtonClick();
    }

    public void SetMusicActive(bool active)
    {
        musicEnabled = active;
        musicSource.mute = !active;
        UpdateAllMusicButtons();
    }

    public bool IsMusicEnabled() => musicEnabled;

    public void ToggleSounds()
    {
        soundsEnabled = !soundsEnabled;
        SetSoundsActive(soundsEnabled);
        PlayButtonClick();
    }

    public void SetSoundsActive(bool active)
    {
        soundsEnabled = active;
        sfxSource.mute = !active;
        UpdateAllSoundButtons();
    }

    public bool IsSoundsEnabled() => soundsEnabled;
    public void RegisterMusicButtons(Button musicOnButton, Button musicOffButton)
    {
        if (musicOnButton != null && !musicOnButtons.Contains(musicOnButton))
            musicOnButtons.Add(musicOnButton);

        if (musicOffButton != null && !musicOffButtons.Contains(musicOffButton))
            musicOffButtons.Add(musicOffButton);

        UpdateMusicButtons(musicOnButton, musicOffButton);
    }

    public void RegisterSoundButtons(Button soundsOnButton, Button soundsOffButton)
    {
        if (soundsOnButton != null && !soundsOnButtons.Contains(soundsOnButton))
            soundsOnButtons.Add(soundsOnButton);

        if (soundsOffButton != null && !soundsOffButtons.Contains(soundsOffButton))
            soundsOffButtons.Add(soundsOffButton);

        UpdateSoundButtons(soundsOnButton, soundsOffButton);
    }

    public void UnregisterMusicButtons(Button musicOnButton, Button musicOffButton)
    {
        if (musicOnButton != null) musicOnButtons.Remove(musicOnButton);
        if (musicOffButton != null) musicOffButtons.Remove(musicOffButton);
    }

    public void UnregisterSoundButtons(Button soundsOnButton, Button soundsOffButton)
    {
        if (soundsOnButton != null) soundsOnButtons.Remove(soundsOnButton);
        if (soundsOffButton != null) soundsOffButtons.Remove(soundsOffButton);
    }

    private void UpdateAllMusicButtons()
    {
        foreach (var button in musicOnButtons)
            if (button != null) button.gameObject.SetActive(musicEnabled);

        foreach (var button in musicOffButtons)
            if (button != null) button.gameObject.SetActive(!musicEnabled);
    }

    private void UpdateAllSoundButtons()
    {
        foreach (var button in soundsOnButtons)
            if (button != null) button.gameObject.SetActive(soundsEnabled);

        foreach (var button in soundsOffButtons)
            if (button != null) button.gameObject.SetActive(!soundsEnabled);
    }

    private void UpdateMusicButtons(Button onButton, Button offButton)
    {
        if (onButton == null || offButton == null) return;

        onButton.gameObject.SetActive(musicEnabled);
        offButton.gameObject.SetActive(!musicEnabled);
    }

    private void UpdateSoundButtons(Button onButton, Button offButton)
    {
        if (onButton == null || offButton == null) return;

        onButton.gameObject.SetActive(soundsEnabled);
        offButton.gameObject.SetActive(!soundsEnabled);
    }

    public void PlayGravity() => PlaySFX(GravitySound);
    public void PlayWin() => PlaySFX(winSound);
    public void PlayLose() => PlaySFX(loseSound);
    public void PlayButtonClick() => PlaySFX(ClickSound);
    public void PlaySkinSelect() => PlaySFX(skinSelectSound);
    public void PlayDiamondInteraction() => PlaySFX(DiamondInteractionSound);
    public void PlayDiamondWin() => PlaySFX(DiamondWinSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null && soundsEnabled)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}