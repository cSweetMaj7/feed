using UnityEngine;
using System.Collections;
using static Barrier;

public static class CollisionHandler
{
    public static void HandleCollision(GameObject hitObject, Ball ball)
    {
        GameManager gameManager = hitObject.GetComponentInParent<GameManager>();
        Barrier barrierHit = hitObject.GetComponent<Barrier>();
        if(!barrierHit)
        {
            barrierHit = hitObject.GetComponentInParent<Barrier>();
        }
        BarrierType typeHit = barrierHit.getBarrierType();

        // handling for Targets
        if(typeHit == BarrierType.Target)
        {
            //gameManager.soundManager.playBallBounceBarrier();
            Target hitTarget = barrierHit as Target;
            hitTarget.deductResources(ball.getBallResources());
            if(hitTarget.breakResources.allResourceSum() < 1)
            {
                GameObject.Destroy(hitObject.GetComponentInParent<Target>().gameObject);
                gameManager.soundManager.playTargetDestroyed();
                
                if (gameManager.getPowerLevel() >= gameManager.powerMax) // split the ball
                {
                    GameObject splitBall = GameObject.Instantiate(ball.gameObject);
                    splitBall.gameObject.transform.parent = gameManager.gameObject.transform;
                    Ball newBall = splitBall.GetComponent<Ball>();

                    int powerLevel = gameManager.getPowerLevel();
                    newBall.setResources(powerLevel, powerLevel, powerLevel, powerLevel);
                    newBall.setComponents();
                    // make the split ball smaller than the original
                    newBall.setHalfBallSize();
                    if (newBall.reflectedY)
                    {
                        newBall.yDir = !newBall.yDir;
                    }
                    else
                    {
                        newBall.xDir = !newBall.xDir;
                    }
                    newBall.setComponents();
                    newBall.setCircleCollider(false);
                    newBall.launch();
                    // clear all circle colliders once we start splitting
                    gameManager.clearCircleColliders();
                }
            }
        }

        ShotLine shotLine = barrierHit as ShotLine;
        if (typeHit == BarrierType.ShotLine)
        {
            // see if this is the first ball back
            if(!shotLine.isLaunchPointSet())
            {
                ball.setLock(true);
                ball.transform.position = new Vector2(ball.transform.position.x, -4.61f);
                shotLine.updateLaunchPoint(ball.transform.position, ball.gameObject);
                
            } else {
                // destroy this extra ball
                GameObject.Destroy(ball.gameObject);
            }
        }

        // handling for simple Barriers
        if (typeHit == BarrierType.Barrier)
        {
            // gameManager.soundManager.playBallBounceBarrier();
        }
    }
}
