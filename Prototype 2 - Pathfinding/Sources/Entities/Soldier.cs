using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Soldier : PlayableEntity, IAnimationListener
{
	#region Fields

	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _buildAnimation;
	[SerializeField, ValueDropdown(nameof(GetAnimatorAnimations))] private AnimationClip _shootAnimation;
	[SerializeField] private float _360AngleDuration;

	private Transform _gfx;
	private Animator _animator;
	private LineRenderer _shotFxLine;
	private Vector3 _gfxEulerOffset;
	private Node _attackTarget;

	private GameObject _buildPreviewPrefab;
	private GameObject _buildPreviewInstance;
	private EntityData _selectedBuild;

	public static event Action<EntityData> OnEntityBuildStart;

	#endregion

	#region Unity builtins

	protected override void OnEnable()
	{
		base.OnEnable();
		BuildItemUI.OnBuildSelected += OnBuildBtnClicked;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BuildItemUI.OnBuildSelected -= OnBuildBtnClicked;
	}

	protected override void Awake()
	{
		base.Awake();
		_buildPreviewPrefab = ResourceLoader.GetBuildPreview();
		_animator = GetComponentInChildren<Animator>();
		_gfx = _animator.transform;
		_shotFxLine = GetComponentInChildren<LineRenderer>(includeInactive: true);

		_gfxEulerOffset = transform.rotation.eulerAngles - _gfx.rotation.eulerAngles;
	}

	#endregion

	#region Build

	protected override void OnPreviewCanceled()
	{
		if (_state == Action.BUILD)
		{
			Destroy(_buildPreviewInstance);
			_selectedBuild = null;
		}
		base.OnPreviewCanceled();
	}

	private void OnBuildBtnClicked(EntityData buildData)
	{
		if (!IsPlaying || HasExecutedAnAction) return;

		_state = Action.BUILD;
		_selectedBuild = buildData;

		_buildPreviewInstance = Instantiate(_buildPreviewPrefab);
		Instantiate(buildData.Prefab.transform.GetChild(0), _buildPreviewInstance.transform);
		_buildPreviewInstance.GetComponent<BuildPreview>().ConvertMaterials();
		_buildPreviewInstance.SetActive(false);
		ShowPreview();
	}

	protected override void OnBuildNodeSelected(Node node)
	{
		_buildPreviewInstance.transform.position = node.WorldPos;
		_buildPreviewInstance.SetActive(true);
	}

	protected override void ShowBuildPreview()
	{
		var preview = EntityMap.Instance.GetDamageableSurroundings(CurrentNode, 1);

		// cannot build on populated node
		preview.RemoveAll(x => !x.IsEmpty);
		CellManager.Instance.PreviewCells(preview, _previewData.RangePreviewColor);
	}

	protected override void BuildOnNode(Node node)
	{
		if (!node.IsEmpty) return;

		OnEntityBuildStart?.Invoke(_selectedBuild);
		// Look at the node we're building on and build on it
		StartCoroutine(LookAtNode(node, _gfx, _gfxEulerOffset, _360AngleDuration,
			// Spawning the build in and setting the entity busy
			onStart: (duration, angle) => {
				// Spawn the build
				GameObject instance = EntityMap.Instance.SpawnEntity(_selectedBuild.Prefab, node);
				DissolveEffect spawnEffect = instance.GetComponent<DissolveEffect>();

				// Set apparition time
				if (spawnEffect != null)
					spawnEffect.SpawnDuration = _buildAnimation.length + duration;
				// Walk animation while facing the build
				_animator.SetFloat("Speed", 0.5f);
				SetBusy(_buildAnimation.length + duration);
			},
			// Start build animation and tidy up the preview
			onDone: () => {
				// We are done facing the node
				_animator.SetFloat("Speed", 0f);

				// The playing the build animation
				_animator.Play(_buildAnimation.name);

				// Tidy up the preview system
				Destroy(_buildPreviewInstance);
				_selectedBuild = null;

				// Inform the turn based system that we've executed an action
				HasExecutedAnAction = true;
			}
		));
	}

	#endregion

	#region Attack

	protected override void Attack(Node node)
	{
		_attackTarget = node;

		// Aim at target, and shoot it by playing the shoot animation
		StartCoroutine(LookAtNode(node, _gfx, _gfxEulerOffset, _360AngleDuration,
			onStart: (duration, angle) =>
			{
				SetBusy(duration + _shootAnimation.length);
				_animator.SetFloat("Speed", 0.5f);
			},
			onDone: () => {
				_animator.SetFloat("Speed", 0f);
				_animator.Play(_shootAnimation.name);
			}
		));
	}

	/// <summary>
	/// Applies damages to the target and play the shoot animation
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	private IEnumerator PlayShootAnimation(Node target)
	{
		// Applying damage
		if (!target.IsEmpty)
		{
			// calling ToList() to create a copy since an entity my get removed after dying
			foreach (EntityType entity in target.Entities.ToList())
				entity.GetComponent<IDamageProcessor>()?.ApplyDamage(gameObject, Damage.Value);
		}

		// Displaying the shot effect
		_shotFxLine.SetPositions(new Vector3[]{
			_shotFxLine.transform.position,
			target.WorldPos.WithY(_shotFxLine.transform.position.y
		)});
		_shotFxLine.enabled = true;
		yield return new WaitForSeconds(0.1f);
		_shotFxLine.enabled = false;
	}

	#endregion

	public void OnAnimationEvent(string animationEvent)
	{
		if (animationEvent == "Attack")
			StartCoroutine(PlayShootAnimation(_attackTarget));
	}

	public override void OnTurnEnd()
	{
		base.OnTurnEnd();
		if (_state == Action.BUILD)
		{
			Destroy(_buildPreviewInstance);
			_selectedBuild = null;
		}
	}

	#region Editor

	private IEnumerable GetAnimatorAnimations()
	{
		Animator animator = gameObject.GetComponentInChildren<Animator>();
		return EditorUtilities.GetAnimatorAnimations(animator);
	}

	#endregion
}