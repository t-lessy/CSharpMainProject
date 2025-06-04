using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UnitBrains.Player
{
    public interface IUnitCoordinator
    {
        float StandardAttackRange { get; }
        Vector2Int RecommendedTarget { get; }
        Vector2Int RecommendedPoint { get; }
    }
}
