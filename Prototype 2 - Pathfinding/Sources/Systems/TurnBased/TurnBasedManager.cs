using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Defines the turn based system
/// </summary>
public class TurnBasedManager : Singleton<TurnBasedManager>
{
	#region Fields

	[SerializeField] private float _maxTurnTime;
	[SerializeField] private bool _timerOn;

	private int _playingIndex = 0;
	private bool _isBufferFull = false;
	private List<ATurnBasedEntity> _entities = new List<ATurnBasedEntity>();
	private Stopwatch _turnTimer = new Stopwatch();

	public event Action OnWillEndTurn;
	public event Action<int> OnFullTurnBegin;
	public event Action<ATurnBasedEntity> OnEntityAdded;
	public event Action<ATurnBasedEntity> OnEntityRemoved;

	#endregion

	#region Properties

	public float MaxTurnTime => _maxTurnTime;
	public float ElapsedTime => (float)_turnTimer.Elapsed.TotalSeconds;
	public ATurnBasedEntity CurrentlyPlayingEntity { get; private set; }

	public TeamData[] Teams { get; private set; }
	public int TurnNumber { get; private set; }
	public bool Started { get; private set; }

	#endregion

	#region Unity builtins

	private void Awake()
	{
		Teams = ResourceLoader.GetTeamDatas();
		foreach (ATurnBasedEntity item in GameObject.FindObjectsOfType<ATurnBasedEntity>())
			AddEntity(item);
	}

	private void OnEnable()
	{
		ActionPanel.OnEndTurnButtonPressed += EndTurnHandler;
	}

	private void OnDestroy()
	{
		ActionPanel.OnEndTurnButtonPressed -= EndTurnHandler;
	}

	private void EndTurnHandler() => EndTurn();

    private void Update()
    {
		if (_timerOn)
		{
			if (!_turnTimer.IsRunning) return;

			if (_turnTimer.Elapsed.TotalSeconds >= _maxTurnTime)
				EndTurn();
		}
    }

    #endregion

    /// <summary>
    /// Adds an entity to the system
    /// </summary>
    /// <param name="entity"></param>
    public void AddEntity(ATurnBasedEntity entity)
	{
        if (_entities.Contains(entity)) return;

		_entities.Add(entity);
		OnEntityAdded?.Invoke(entity);
	}

	/// <summary>
	/// Removes an entity from the system
	/// </summary>
	/// <param name="entity"></param>
	public void RemoveEntity(ATurnBasedEntity entity)
	{
		if (entity == CurrentlyPlayingEntity)
			EndTurn(true);
		_entities.Remove(entity);
		OnEntityRemoved?.Invoke(entity);
	}

	private async Task WaitForEndOfAction(ATurnBasedEntity playing, bool forced)
	{
		while (playing.IsBusy && !forced)
			await Task.Yield();
	}

	/// <summary>
	/// Ends the current entity's turn
	/// </summary>
	public async void EndTurn(bool forced = false)
	{
		if (!Started) return;

		ATurnBasedEntity current = CurrentlyPlayingEntity;

		// Ensures an end turn coroutine is not already executing
		if (_isBufferFull) return;

		_isBufferFull = true;
		OnWillEndTurn?.Invoke();

		// Waiting for the entity to end its turn
		await WaitForEndOfAction(current, forced);

		// End entity's turn
		current.OnTurnEnd();

		// Refreshing the queue
		_playingIndex++;

		// Handle the end of a full turn
		if (_playingIndex >= _entities.Count)
		{
			_playingIndex = 0;
			TurnNumber++;
			OnFullTurnBegin?.Invoke(TurnNumber);
			GameManager.Instance.OnFullTurnBegin(TurnNumber);
		}

		// Start new turn
		_turnTimer.Restart();
		CurrentlyPlayingEntity = _entities[_playingIndex];
		CurrentlyPlayingEntity.OnTurnBegin();
		_isBufferFull = false;
	}

	public void StopSystem()
	{
		Started = false;
		_turnTimer.Stop();
	}

	public async void StartSystem()
	{
		while (_entities.Any(x => !x.ReadyToStart))
			await Task.Yield();

		// Starting the system
		Started = true;
		CurrentlyPlayingEntity = _entities[0];
		_turnTimer.Start();
		OnFullTurnBegin?.Invoke(TurnNumber);
		GameManager.Instance.OnFullTurnBegin(TurnNumber);
		CurrentlyPlayingEntity.OnTurnBegin();
	}
}
