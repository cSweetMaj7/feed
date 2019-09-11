using UnityEngine;
using System.Collections;

public class ShotLine : Barrier
{
    Vector2 launchPoint;
    public GameObject launchBall;
    bool launchPointSet = false;
    // Use this for initialization
    void Start()
    {
        // default launch point for now
        launchPoint = new Vector2(0, -4.61f);
        launchPointSet = true;
    }

    public void clearLaunchPoint()
    {
        launchPointSet = false;
    }

    public void updateLaunchPoint(Vector2 input, GameObject ball)
    {
        launchPoint = input;
        launchBall = ball;
        //GameManager gameManager = GetComponentInParent<GameManager>();
        // set the ball back to default size in case it was shrunk
        ball.GetComponent<Ball>().setBallSize();
        //gameManager.SetGameState(GameManager.GameState.Aim);
        launchPointSet = true;
    }

    public bool isLaunchPointSet()
    {
        return launchPointSet;
    }

    // Update is called once per frame
    void Update()
    {

    }

    override public BarrierType getBarrierType()
    {
        return Barrier.BarrierType.ShotLine;
    }
}
