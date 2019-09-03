using UnityEngine;
using System.Collections;

public class ShotLine : Barrier
{
    Vector2 launchPoint;
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

    public void updateLaunchPoint(Vector2 input)
    {
        launchPoint = input;
        GameManager gameManager = GetComponentInParent<GameManager>();
        gameManager.SetGameState(GameManager.GameState.Aim);
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
