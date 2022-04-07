using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Linq.Expressions;

public static class ResourceLoader
{
    public static TeamData[] GetTeamDatas() => Resources.LoadAll<TeamData>("Scriptables");
    public static GameObject GetResourcePrefab() => Resources.Load<GameObject>("Prefabs/ResourceEntity");
    public static GameObject GetCellPrefab() => Resources.Load<GameObject>("Prefabs/Cell");
    public static PreviewData GetPreviewData() => Resources.Load<PreviewData>("Scriptables/CellPreview");
    public static EntityData[] GetEntities(Expression<Func<EntityData, bool>> expression)
    {
        EntityData[] entities = Resources.LoadAll<EntityData>("Scriptables/EntityIds");

        return entities.Where(expression.Compile()).ToArray();
    }
    public static GameObject GetBuildPreview() => Resources.Load<GameObject>("Prefabs/BuildPreview");
}