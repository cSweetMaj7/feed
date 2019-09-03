using UnityEngine;
using System.Collections;
using static Barrier;

public static class CollisionHandler
{
    public static void HandleCollision(GameObject hitObject, Ball ball)
    {
        Barrier barrierHit = hitObject.GetComponentInParent<Barrier>();
        BarrierType typeHit = barrierHit.getBarrierType();

        // handling for Targets
        if(typeHit == BarrierType.Target)
        {
            
            Target hitTarget = barrierHit as Target;
            hitTarget.deductResources(ball.getBallResources());
            if(hitTarget.breakResources.allResourceSum() < 1)
            {
                GameObject.Destroy(hitObject);
            }
        }

        // handling for simple Barriers
        if(typeHit == BarrierType.Barrier)
        {

        }
    }
}
