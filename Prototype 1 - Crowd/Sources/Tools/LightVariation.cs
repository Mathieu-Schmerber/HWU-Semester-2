using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightVariation : MonoBehaviour
{
    private Light _light;

    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    [SerializeField] private float repeatTime = 0.2f;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    private void Start() => InvokeRepeating("ChangeLighting", 0, repeatTime);

    void ChangeLighting() => _light.intensity = Random.Range(minIntensity, maxIntensity);

    private void OnDisable() => CancelInvoke();
}
