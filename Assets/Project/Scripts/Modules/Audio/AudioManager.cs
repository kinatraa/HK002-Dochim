using HaKien;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
public class AudioManager : MonoBehaviour, IMessageHandle 
{
	public static AudioManager Instance { get; private set; }
	//save setting with playerpref


	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this;

			DontDestroyOnLoad(gameObject);

			InitializeAudioSources();
		}
		else if (Instance != this)
		{
			Debug.LogWarning("Duplicate AudioManager instance detected. Destroying the new one.");
			Destroy(gameObject);
		}
	}


	[Header("Audio Settings")]
	[SerializeField] private int _initialPoolSize = 10;
	[SerializeField] private AudioSource _musicSource;
	[SerializeField] private float _musicFadeDuration = 1.0f;

	// [SerializeField] private AudioMixerGroup _musicMixerGroup;
	// [SerializeField] private AudioMixerGroup _sfxMixerGroup;

	[Header("Music Clips")]
	public AudioClip _mainMenuMusic;
	public AudioClip _gamePlayMusic;

	[Header("SFX Clips")]
	public AudioClip _sfxGameOver;
	public AudioClip _sfxButtonClick;
	public AudioClip _sfxVictory;

	private List<AudioSource> _sfxAudioSourcePool;
	private AudioClip _currentMusicClip;

	private float _musicVolume = 1f;
	private float _sfxVolume = 1f;

	public float MusicVolume
	{
		get => _musicVolume;
		set 
		{
			_musicVolume = value;
			_musicSource.volume = value;
		}
	}

	public float SFXVolume
	{
		get => _sfxVolume;
		set 
		{
			_sfxVolume = value;
			foreach (var source in _sfxAudioSourcePool)
			{
				source.volume = value;
			}
		}
	}

	private void InitializeAudioSources()
	{
		Debug.Log("Initializing Audio Sources..."); 
													
		if (_musicSource == null)
		{
			_musicSource = gameObject.AddComponent<AudioSource>();
			Debug.LogWarning("AudioManager: Music Source was not assigned, creating one automatically.");
		}
		_musicSource.loop = true;
		_musicSource.playOnAwake = false;
		_musicSource.volume = _musicVolume;

		_sfxAudioSourcePool = new List<AudioSource>(_initialPoolSize);
		for (int i = 0; i < _initialPoolSize; i++)
		{
			CreateSfxAudioSource();
		}
	}

	private AudioSource CreateSfxAudioSource()
	{
		AudioSource source = gameObject.AddComponent<AudioSource>();
		source.playOnAwake = false;
		_sfxAudioSourcePool.Add(source);
		return source;
	}

	private void Start()
	{
		if (_musicSource != null && _musicSource.clip == null)
		{
			PlayMusic(_mainMenuMusic, true, 1f);
		}
	}

	private void OnEnable()
	{
		// Quan trọng: Vẫn cần đăng ký Message
		SubscribeToMessages();
	}

	private void OnDisable()
	{
		UnsubscribeFromMessages();
		StopAllCoroutines();
	}

	private void SubscribeToMessages()
	{
		if (MessageManager.Instance != null)
		{
			MessageManager.Instance.AddSubcriber(MessageType.OnGameStart, this);
			MessageManager.Instance.AddSubcriber(MessageType.OnGameWin, this);
			MessageManager.Instance.AddSubcriber(MessageType.OnGameLose, this);
			MessageManager.Instance.AddSubcriber(MessageType.OnButtonClick, this);
		}
		else
		{
			Debug.LogError("AudioManager: Cannot subscribe messages because MessageManager.Instance is null on Enable.");
		}
	}

	private void UnsubscribeFromMessages()
	{
		if (MessageManager.Instance != null)
		{
			MessageManager.Instance.RemoveSubcriber(MessageType.OnGameStart, this);
			MessageManager.Instance.RemoveSubcriber(MessageType.OnGameWin, this);
			MessageManager.Instance.RemoveSubcriber(MessageType.OnGameLose, this);
			MessageManager.Instance.RemoveSubcriber(MessageType.OnButtonClick, this);
		}
	}

	public void PlaySfx(AudioClip clip, float volumeScale = 1f, float pitch = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("AudioManager: PlaySfx called with a null clip.");
			return;
		}

		AudioSource source = GetAvailableSfxAudioSource();
		if (source != null)
		{
			source.pitch = pitch;
			source.volume = SFXVolume;
			source.PlayOneShot(clip, volumeScale);
		}
		else
		{
			Debug.LogError("AudioManager: Could not get an available SFX AudioSource.");
		}
	}

	private AudioSource GetAvailableSfxAudioSource()
	{
		foreach (var source in _sfxAudioSourcePool)
		{
			if (!source.isPlaying)
			{
				return source;
			}
		}

		Debug.LogWarning("AudioManager: SFX pool size exceeded. Creating a new temporary AudioSource.");
		AudioSource newSource = CreateSfxAudioSource();
		return newSource;
	}

	public void PlayMusic(AudioClip clip, bool loop = true, float volumeScale = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("AudioManager: PlayMusic called with a null clip.");
			return;
		}
		if (_musicSource == null)
		{
			Debug.LogError("AudioManager: Music Source is null. Cannot play music.");
			return;
		}

		if (_musicSource.isPlaying && _musicSource.clip == clip)
		{
			return;
		}

		StopCoroutine(nameof(FadeMusicCoroutine)); 
		float targetVolume = _musicVolume * Mathf.Clamp01(volumeScale); 
		StartCoroutine(FadeMusicCoroutine(clip, loop, targetVolume, _musicFadeDuration));
	}

	private IEnumerator FadeMusicCoroutine(AudioClip newClip, bool loop, float targetVolume, float duration)
	{
		float startVolume = _musicSource.isPlaying ? _musicSource.volume : 0f; 
		float currentTime = 0f;

		if (_musicSource.isPlaying && startVolume > 0 && duration > 0)
		{
			while (currentTime < duration)
			{
				currentTime += Time.unscaledDeltaTime; 
				_musicSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / duration);
				yield return null;
			}
			_musicSource.Stop();
		}
		else if (_musicSource.isPlaying)
		{
			_musicSource.Stop(); 
		}


		_musicSource.clip = newClip;
		_musicSource.loop = loop;
		_musicSource.volume = 0f;
		_musicSource.Play();
		_currentMusicClip = newClip; 

		currentTime = 0f;
		if (duration > 0)
		{
			while (currentTime < duration)
			{
				currentTime += Time.unscaledDeltaTime;
				_musicSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
				yield return null;
			}
		}


		_musicSource.volume = targetVolume;
	}


	public AudioClip GetCurrentMusicClip()
	{
		return _currentMusicClip;
	}

	public void Handle(Message message)
	{
		switch (message.type)
		{
			// Music
			case MessageType.OnGameStart:
				if (_gamePlayMusic != null) PlayMusic(_gamePlayMusic, true, 1f); 
				break;

			// SFX
			case MessageType.OnGameWin:
				if (_sfxVictory != null) PlaySfx(_sfxVictory);
				break;
			case MessageType.OnGameLose:
				if (_sfxGameOver != null) PlaySfx(_sfxGameOver);
				break;
			case MessageType.OnButtonClick:
				if (_sfxButtonClick != null) PlaySfx(_sfxButtonClick, 0.8f); 
				break;
		}
	}
}