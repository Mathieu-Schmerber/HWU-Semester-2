using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeSwitch : MonoBehaviour
{
    [SerializeField] private Gamemode.Mode _playerMode;
    [SerializeField] private List<Behaviour> _playerScripts = new List<Behaviour>();
    [SerializeField] private List<Behaviour> _aiScripts = new List<Behaviour>();

    public void ApplyGamemode(Gamemode.Mode mode)
	{
        _aiScripts.ForEach(x => x.enabled = mode != _playerMode);
        _playerScripts.ForEach(x => x.enabled = mode == _playerMode);
    }
}
