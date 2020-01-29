using UnityEngine;
using System.Collections;

public class ShotLine : Barrier
{
    Vector2 launchPoint;
    public GameObject launchBall;
    bool launchPointSet = false;

    private float launchPointY;

    // Use this for initialization
    void Start()
    {
        // default launch point for now
        launchPointY = launchBall.transform.position.y;
        launchPoint = new Vector2(0, launchPointY);
        launchPointSet = true;
    }

    public void clearLaunchPoint()
    {
        launchPointSet = false;
    }

    public void updateLaunchPoint(Vector2 input, GameObject ball)
    {
        // normalize y to the launch point (to protect from faster moving balls overshoot)
        input.y = launchPointY;

        launchPoint = input;
        launchBall = ball;
        //GameManager gameManager = GetComponentInParent<GameManager>();
        // set the ball back to default size in case it was shrunk
        ball.GetComponent<Ball>().setBallSize();
        ball.GetComponent<Ball>().isLaunchBall = true;

        // move the ball's position in case we normalized it
        ball.transform.position = launchPoint;

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
