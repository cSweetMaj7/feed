using UnityEngine;
using System.Collections;
using static Barrier;

public static class CollisionHandler
{
    public static void HandleCollision(GameObject hitObject)
    {
        Barrier barrierHit = hitObject.GetComponentInParent<Barrier>();
        BarrierType typeHit = barrierHit.getBarrierType();

        // handling for Targets
        if(typeHit == BarrierType.Target)
        {
            // for now just destroy what was hit
            GameObject.Destroy(hitObject);
        }

        // handling for simple Barriers
        if(typeHit == BarrierType.Barrier)
        {

        }
    }
}
