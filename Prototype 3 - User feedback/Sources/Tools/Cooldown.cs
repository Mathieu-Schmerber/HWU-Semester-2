using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cooldown
{
    // Cooldown management
    private float _nextTime;
    private float _lastTime;

    public float LastOverTime { get => _lastTime; }

    public delegate void Callback(Cooldown actor);
    public event Callback IsOverEvent;

    public float cooldownTime;
    public bool readyOnStart = true;
    public bool automaticReset = true;

    public void Init()
    {
        if (!readyOnStart)
            this.Reset();
        this.Run();
    }

    // Automatic behaviour, callback based
    public void Run()
    {
        if (this.IsOver())
        {
            if (IsOverEvent != null)
                IsOverEvent(this);
            if (this.automaticReset)
                this.Reset();
        }
    }

    // Cooldown setter
    public void SetCooldown(float _time)
    {
        _lastTime = 0;
        cooldownTime = _time;
    }

    // Get elapsed time since last reset
    public float GetElapsed()
    {
        return Time.time - _lastTime;
    }

    // Get time until over
    public float TimeUntilOver()
    {
        return _nextTime - Time.time;
    }

    // Is the cooldown over
    public bool IsOver()
    {
        return Time.time >= _nextTime;
    }

    // Reset cooldown
    public void Reset()
    {
        _nextTime = Time.time + cooldownTime;
        _lastTime = Time.time;
    }
}