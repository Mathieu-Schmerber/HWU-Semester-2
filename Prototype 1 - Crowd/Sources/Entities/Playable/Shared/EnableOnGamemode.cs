using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnGamemode : MonoBehaviour
{
    [SerializeField] private Gamemode.Mode modeEnable;

	private void Start()
	{
		gameObject.SetActive(Gamemode.Instance.CurrentMode == modeEnable);
	}
}
