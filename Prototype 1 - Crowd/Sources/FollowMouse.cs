using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float yOffset;

	// Update is called once per frame
	void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.MousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, groundLayer))
            transform.position = hit.point + Vector3.up * yOffset;
    }
}
