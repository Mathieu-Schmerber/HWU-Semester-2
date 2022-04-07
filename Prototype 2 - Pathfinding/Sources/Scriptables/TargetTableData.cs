using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Target Worth Table")]
public class TargetTableData : ScriptableObject
{
    [System.Serializable]
    public class TargetWorthValue : UnitySerializedDictionary<EntityData, int> { }

    public TargetWorthValue WorthTable;

    [Button("Load all entities")]
    private void LoadFullTable()
	{
        EntityData[] entityDatas = ResourceLoader.GetEntities(x => x);

        entityDatas.ForEach(x => WorthTable.Add(x, 0));
	}
}
