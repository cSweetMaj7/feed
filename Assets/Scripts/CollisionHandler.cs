using UnityEngine;
using System.Collections;
using static Barrier;

public static class CollisionHandler
{
    public static void HandleCollision(GameObject hitObject, Ball ball)
    {
        Barrier barrierHit = hitObject.GetComponent<Barrier>();
        if(!barrierHit)
        {
            barrierHit = hitObject.GetComponentInParent<Barrier>();
        }
        BarrierType typeHit = barrierHit.getBarrierType();

        // handling for Targets
        if(typeHit == BarrierType.Target)
        {
            Target hitTarget = barrierHit as Target;
            hitTarget.deductResources(ball.getBallResources());
            if(hitTarget.breakResources.allResourceSum() < 1)
            {
                GameObject.Destroy(hitObject.GetComponentInParent<Target>().gameObject);
            }
        }

        if(typeHit == BarrierType.ShotLine)
        {
            // see if this is the first ball back
            ShotLine shotLine = barrierHit as ShotLine;
            if(!shotLine.isLaunchPointSet())
            {
                ball.setLock(true);
                ball.transform.position = new Vector2(ball.transform.position.x, -4.61f);
                shotLine.updateLaunchPoint(ball.transform.position);
            } else {
                // destroy this extra ball
                GameObject.Destroy(ball.gameObject);
            }
        }

        // handling for simple Barriers
        if(typeHit == BarrierType.Barrier)
        {

        }
    }
}
