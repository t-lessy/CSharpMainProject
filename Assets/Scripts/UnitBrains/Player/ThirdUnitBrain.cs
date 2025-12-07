using Model;
using Model.Runtime.Projectiles;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Player;
using UnityEngine;
using Utilities;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";

    private bool isAttackMode = false;
    private bool isMoveMode = true;
    private const float switchTime = 1.0f;
    private float switchTimer = 0f;

    protected override List<Vector2Int> SelectTargets()
    {
        if (isAttackMode && switchTimer <= 0)
        {
            return base.SelectTargets();
        }
        return new List<Vector2Int>();
    }

    public override Vector2Int GetNextStep()
    {
        if (isMoveMode && switchTimer <= 0)
        {
            return base.GetNextStep();
        }
        return unit.Pos;
    }

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);

        var targets = base.SelectTargets();
        bool hasTargets = targets != null && targets.Count > 0;
        if (switchTimer > 0)
        {
            switchTimer -= deltaTime;

            if (switchTimer <= 0)
            {
                isMoveMode = !hasTargets;
                isAttackMode = hasTargets;
            }
            return;
        }     
        if (isMoveMode && hasTargets)
        {
            switchTimer = switchTime;
            isMoveMode = false;
            isAttackMode = false;
            return;
        }
        if (isAttackMode && !hasTargets)
        {
            switchTimer = switchTime;
            isMoveMode = false;
            isAttackMode = false;
            return;
        }
    }
}

