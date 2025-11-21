using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [Header("Audio Settings")]
    [Tooltip("Audio Source for planting effects")]
    public AudioSource audioSource;
    
    [Tooltip("Audio Source for general effects")]
    public AudioSource effectAudioSource;

    [Tooltip("Audio Source for drone sounds")]
    public AudioSource droneAudioSource;
    
    [Tooltip("Main theme sound")]
    public AudioClip mainThemeSound;
    
    [Tooltip("Drone sound effect")]
    public AudioClip droneSound;
    
    [Tooltip("Sound effect when plantation is ready to recolt")]
    public AudioClip popSound;
    
    [Tooltip("Sound effect when recolting a plantation")]
    public AudioClip recoltingSound;
    
    [Tooltip("Sound effect for watering a plantation")]
    public AudioClip waterSound;
    
    [Tooltip("Sound effect for plant a seed")]
    public AudioClip plantSound;
    
    [Tooltip("Sound effect for hovering over UI elements")]
    public AudioClip hoverSound;
    
    [Tooltip("Sound effect for clicking UI elements")]
    public AudioClip clickSound;
    
    [Tooltip("Sound effect for finish")]
    public AudioClip finishSound;
    
    [Tooltip("Sound effect for explosion")]
    public AudioClip explosionSound;
    
    [Tooltip("Sound effect for changing inventory")]
    public AudioClip changeInventorySound;
    
    [Header("Volume Settings")]
    [Tooltip("Slider for adjusting the game volume")]
    public Slider gameVolumeSlider;
    
    [Tooltip("Slider for adjusting the SFX volume")]
    public Slider sfxVolumeSlider;
    
    [Tooltip("Default game volume level")]
    public float defaultGameVolume = 0.5f;
    
    [Tooltip("Default SFX volume level")]
    public float defaultSfxVolume = 0.5f;
    
    [Header("Source Manager")]
    [Tooltip("Indicates if the Sound Manager is in the game scene.")]
    public bool isSoundManagerInGame;

    [Tooltip("Indicates if the Sound Manager is in the menu scene.")]
    public bool isSoundManagerInMenu;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    private void Start()
    {
        if (PlayerPrefs.HasKey("GameVolume"))
            gameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolume");
        else
            gameVolumeSlider.value = defaultGameVolume;

        if (PlayerPrefs.HasKey("SfxVolume"))
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        else
            sfxVolumeSlider.value = defaultSfxVolume;
        
        if (audioSource)
            audioSource.volume = gameVolumeSlider.value;
        
        if (droneAudioSource)
            droneAudioSource.volume = gameVolumeSlider.value;
        
        if (effectAudioSource)
            effectAudioSource.volume = sfxVolumeSlider.value;
        
        if (isSoundManagerInMenu)
        {
            audioSource.clip = mainThemeSound;
            PlayMainThemeSound();
        }
        
        if (isSoundManagerInGame)
        {
            audioSource.clip = mainThemeSound;
            PlayMainThemeSound();
            
            droneAudioSource.clip = droneSound;
            PlayDroneSound();
        }
    }
    
    public void PlayPopSound()
    {
        if (audioSource && popSound)
        {
            effectAudioSource.PlayOneShot(popSound);
        }
    }
    
    public void PlayRecoltingSound()
    {
        if (audioSource && recoltingSound)
        {
            effectAudioSource.PlayOneShot(recoltingSound);
        }
    }
    
    public void PlayWaterSound()
    {
        if (audioSource && waterSound)
        {
            effectAudioSource.PlayOneShot(waterSound);
        }
    }
    
    public void PlayPlantSound()
    {
        if (audioSource && plantSound)
        {
            effectAudioSource.PlayOneShot(plantSound);
        }
    }
    
    public void PlayHoverSound()
    {
        if (audioSource && hoverSound)
        {
            effectAudioSource.PlayOneShot(hoverSound);
        }
    }
    
    public void PlayClickSound()
    {
        if (audioSource && clickSound)
        {
            effectAudioSource.PlayOneShot(clickSound);
        }
    }
    
    public void PlayFinishSound()
    {
        if (audioSource && finishSound)
        {
            effectAudioSource.PlayOneShot(finishSound);
        }
    }
    
    public void PlayExplosionSound()
    {
        if (audioSource && explosionSound)
        {
            effectAudioSource.PlayOneShot(explosionSound);
        }
    }
    
    public void PlayChangeInventorySound()
    {
        if (audioSource && changeInventorySound)
        {
            effectAudioSource.PlayOneShot(changeInventorySound);
        }
    }
    
    public void PlayMainThemeSound()
    {
        if (audioSource && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    public void PlayDroneSound()
    {
        if (droneAudioSource && !droneAudioSource.isPlaying)
        {
            droneAudioSource.Play();
        }
    }
    
    public void UpdateGameVolume()
    {
        if (audioSource)
            audioSource.volume = gameVolumeSlider.value;
        if (droneAudioSource)
            droneAudioSource.volume = gameVolumeSlider.value;
        PlayerPrefs.SetFloat("GameVolume", gameVolumeSlider.value);
        PlayerPrefs.Save();
    }
    
    public void UpdateSfxVolume()
    {
        effectAudioSource.volume = sfxVolumeSlider.value;
        PlayerPrefs.SetFloat("SfxVolume", sfxVolumeSlider.value);
        PlayerPrefs.Save();
    }
}
