using Pixelplacement;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundButtonUI : MonoBehaviour
{
    [SerializeField] private Sprite _soundOn;
    [SerializeField] private Sprite _soundOff;

    private Button _btn;
	private bool _state = true;

	private static event Action<bool> _onSoundSwitched;

	private void Awake()
	{
		_btn = GetComponent<Button>();
	}

	private void OnEnable()
	{
		_onSoundSwitched += SynchronizeAllSoundButton;
	}

	private void OnDisable()
	{
		_onSoundSwitched -= SynchronizeAllSoundButton;
	}

	private void Start()
	{
		_btn.onClick.AddListener(SwitchSound);
	}

	private void SynchronizeAllSoundButton(bool state) => _state = state;

	private void SwitchSound()
	{
		_state = !_state;
		AudioListener.volume = _state ? 1f : 0f;
		_btn.image.sprite = _state ? _soundOn : _soundOff;
		Tween.LocalScale(transform, Vector3.one * 0.8f, Vector3.one, 0.2f, 0, Tween.EaseBounce);
		_onSoundSwitched.Invoke(_state);
	}
}
