using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class CellManager : Singleton<CellManager>
{
	[SerializeField] private LayerMask _cellLayer;
	[SerializeField] private Transform _cellStorage;

	private GameObject _cellPrefab;
	private List<Cell> _cells = new List<Cell>();
	private List<Node> _previewNodes = new List<Node>();
	private Cell _selected;
	private bool _currentViewIsPersistent = false;

	public List<Node> PreviewNodes => _previewNodes;

	public event Action<Node> OnNodeClicked;
	public event Action<Node> OnNodeSelected;
	public event Action OnPreviewCanceled;

	private void Awake()
	{
		_cellPrefab = ResourceLoader.GetCellPrefab();
	}

	#region Previews

	private Cell GetAvailable()
	{
		Cell cell = _cells.FirstOrDefault(x => !x.gameObject.activeSelf);

		if (cell == null)
		{
			cell = Instantiate(_cellPrefab, _cellStorage).GetComponent<Cell>();
			cell.gameObject.SetActive(false);
			_cells.Add(cell);
		}
		return cell;
	}

	public void StopPreview()
	{
		_previewNodes.Clear();
		_cells.ForEach(x => x.gameObject.SetActive(false));
	}

	public void StopHightlight() => _cells.Where(x => x.IsHighlighted).ForEach(x => x.SetHighlight(false));

	public void Highlight(Node node)
	{
		if (node == null) return;

		Cell cell = _cells.FirstOrDefault(x => x.transform.position == node.WorldPos);

		if (cell != null)		
			cell.SetHighlight(true);
	}

	public void Highlight(List<Node> nodes)
	{
		if (nodes == null) return;
		nodes.ForEach(x => Highlight(x));
	}

	public void PreviewCell(Node node, Color color)
	{
		if (node == null) return;

		Cell cell = GetAvailable();

		cell.GetComponentInChildren<Renderer>().material.color = color;
		cell.gameObject.SetActive(true);
		cell.transform.position = EntityMap.Instance.ToWorldPos(node.Position);
		cell.Node = node;
		_previewNodes.Add(node);
	}

	public void PreviewCells(IEnumerable<Node> nodes, Color color, bool isPersistent = false)
	{
		_currentViewIsPersistent = isPersistent;
		nodes.ForEach(x => PreviewCell(x, color));
	}

	#endregion

	#region Click management

	private void HandleSelection()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, 100f, _cellLayer))
		{
			Cell cell = hit.transform.GetComponent<Cell>();

			if (cell != _selected && cell.Node != null)
			{
				_selected?.OnDeselect();
				_selected = cell;
				cell.OnSelect();
				OnNodeSelected?.Invoke(_selected.Node);
			}
		}
		else
		{
			_selected?.OnDeselect();
			_selected = null;
		}
	}

	private void HandleClick()
	{
		if (_selected != null && _selected.Node != null)
		{
			// Deselecting any entity when clicking a node
			_selected.Node.Entities.ForEach(x => x.GetComponents<ISelectableListener>().ForEach(y => y.OnDeselect()));
			OnNodeClicked?.Invoke(_selected.Node);
		}
		else if (!_currentViewIsPersistent)
		{
			StopPreview();
			OnPreviewCanceled?.Invoke();
		}
	}

	private void Update()
	{
		if (!TurnBasedManager.Instance.Started) return;
		HandleSelection();
		if (Input.GetMouseButtonDown(0))
			HandleClick();
	}

	#endregion
}