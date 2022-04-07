using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Stats/Enemy")]
public class EnemyData : EntityStatData
{
    public EnemyPool.EnemyType Behaviour;
    public Sprite Graphics;
}
