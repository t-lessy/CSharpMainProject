using Model;
using Model.Runtime.Projectiles;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnitBrains.Player;
using UnityEngine;
using Utilities;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdUnitBrain : DefaultPlayerUnitBrain
{
    public override string TargetUnitName => "Ironclad Behemoth";
    public bool isAttackMode = false;
    public bool isMoveMode = true;
    public const float changeMode = 1f;
    public float switchTimer = 0;


    protected List<Vector2Int> SelectTargets()
    {
        if (isAttackMode || switchTimer == 0)
        {
            return base.SelectTargets();
        }
        return new List<Vector2Int>();
    }

    public override Vector2Int GetNextStep()
    {
        if (switchTimer == 0 || isAttackMode)
        {
            return base.GetNextStep();
        }

        return unit.Pos;
    }

    public override void Update(float deltaTime, float time)
    {
        base.Update(deltaTime, time);

        if (switchTimer > 0)
        {
            switchTimer -= changeMode;

            if (switchTimer < 0)
            {
                isMoveMode = !isMoveMode;
                isAttackMode = !isAttackMode;
            }
            return;
        }

        List<Vector2Int> targets = base.SelectTargets();
        

        if (isMoveMode && targets.Count > 0)
        {
            isAttackMode = true;
            isMoveMode = false;
            
            switchTimer = changeMode;
        }
        else if (isMoveMode && targets.Count == 0)
        {
            isAttackMode = false;
            isMoveMode = true; 
            switchTimer = changeMode;
        }
    }
}
    

    
