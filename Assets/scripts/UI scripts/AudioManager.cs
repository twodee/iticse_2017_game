// From Unity AudioManager tutorial, with modifications.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private List<AudioSource> sfxSources;
	[SerializeField]
	private AudioSource musicSource;
	public static AudioManager instance;
	private float lowPitch = 0.95f;
	private float highPitch = 1.05f;

	#endregion

	#region Unity Event Functions

	void Awake ()
	{
		if (!PlayerPrefs.HasKey("musicVolume")){
			PlayerPrefs.SetFloat("musicVolume", 0.5f);
			PlayerPrefs.SetFloat("sfxVolume", 0.5f);
		}
		if (instance == null)
			instance = this;
		else if(instance != this) //enforces the singleton pattern so there can only be one instance of the AudioManager
			Destroy(gameObject);

		DontDestroyOnLoad(gameObject); //enforces that our AudioManager doesn't get destroyed when loading new scenes

		ChangeMusicVolume(PlayerPrefs.GetFloat("musicVolume"));
		ChangeSfxVolume(PlayerPrefs.GetFloat("sfxVolume"));
	}

	#endregion

	//Play a new music clip
	public void PlayNewMusic(AudioClip clip)
	{
		if (clip == musicSource.clip)
			return;

		musicSource.Stop();

		musicSource.clip = clip;
		musicSource.Play();
	}

	//Return whether or not music is currently playing
	public bool IsMusicPlaying()
	{
		return musicSource.isPlaying;
	}

	public void StopMusic()
	{
		musicSource.Stop();
	}

	public void StopSfx(AudioClip clip)
	{
		foreach (AudioSource source in sfxSources)
		{
			if (source.clip == clip)
			{
				source.Stop();
				return;
			}
		}
	}

	//Plays a single sound clip
	//Finds an available source to play the effect through
	public void PlaySingle(bool isLooped, AudioClip clip)
	{
		foreach (AudioSource source in sfxSources)
		{
			if (!source.isPlaying)
			{
				source.clip = clip;
				source.loop = isLooped;
				source.Play();
				return;
			}
		}
	}

	//Plays a random sound clip at a random pitch (for variation)
	//Finds an available source to play the effect through
	public void PlayRandomEffect(bool isLooped, params AudioClip[] clips)
	{
		int randomClip = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(lowPitch, highPitch);

		foreach (AudioSource source in sfxSources)
		{
			if (!source.isPlaying)
			{
				source.clip = clips[randomClip];
				source.pitch = randomPitch;
				source.loop = isLooped;
				source.Play();
				return;
			}
		}
	}

	public void SetPitchRange(float low, float high){
		lowPitch = low;
		highPitch = high;
	}

	public void ChangeMusicVolume(float value)
	{
		PlayerPrefs.SetFloat("musicVolume", value);
		musicSource.volume = value;
	}

	public void ChangeSfxVolume(float value)
	{
		foreach (AudioSource source in sfxSources)
		{
			PlayerPrefs.SetFloat("sfxVolume", value);
			source.volume = value;
		}
	}

	public float GetMusicVolume()
	{
		return musicSource.volume;
	}

	public float GetSfxVolume()
	{
		return sfxSources[0].volume;
	}
}

