using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TriggerButtonUI : Selectable
{
	[SerializeField] private Sprite _idle;
	[SerializeField] private Sprite _down;

	private Image _image;
	private EventTrigger _trigger;

	protected override void Awake()
	{
		base.Awake();
		_image = GetComponent<Image>();
		_trigger = GetComponent<EventTrigger>();
	}

	protected override void Start()
	{
		base.Start();
		OnDown(() => _image.sprite = _down);
		OnRelease(() => _image.sprite = _idle);
	}

	public void OnClick(Action listener)
	{
		EventTrigger.Entry entry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerClick);

		if (entry != null)
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
		else
		{
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
			_trigger.triggers.Add(entry);
		}
	}

	public void OnDown(Action listener)
	{
		EventTrigger.Entry entry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerDown);

		if (entry != null)
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
		else
		{
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
			_trigger.triggers.Add(entry);
		}
	}

	public void OnRelease(Action listener)
	{
		EventTrigger.Entry entry = _trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerUp);

		if (entry != null)
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
		else
		{
			entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerUp;
			entry.callback.AddListener((baseEvt) => listener?.Invoke());
			_trigger.triggers.Add(entry);
		}
	}
}