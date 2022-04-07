using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
	[SerializeField] private GameObject _afterImagePrefab;
    private SkinnedMeshRenderer _skin;

	private void Awake()
	{
		_skin = GetComponentInChildren<SkinnedMeshRenderer>();
	}

	private IEnumerator AfterImage(float duration, int number)
	{
		float interval = duration / number;

		for (int i = 0; i < number; i++)
		{
			GameObject image = Instantiate(_afterImagePrefab, transform.position, transform.rotation);
			var filter = image.GetComponentInChildren<MeshFilter>();

			_skin.BakeMesh(filter.mesh);
			yield return new WaitForSeconds(interval);
		}
	}

	public void Play(float duration, int number)
	{
		StartCoroutine(AfterImage(duration, number));
	}
}
