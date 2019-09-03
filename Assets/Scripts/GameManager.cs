using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public Ball[] ballList;
    public GameObject aimReticle;
    public GameObject shotLine;
    public TargetField targetField;

    private List<Target> targetList;
    GameState gameState = GameState.Aim; // starts off in aim state

    public enum GameState
    {
        Aim,
        Action,
        End,
        Pause
    }

    public void SetGameState(GameState gameStateIn)
    {
        gameState = gameStateIn;
    }

    // Start is called before the first frame update
    void Start()
    {
        aimReticle.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // check for game enders
        if(targetField.targetsRemaining() < 1)
        {
            gameState = GameState.End;
        }

        if(gameState == GameState.End)
        {
            endGame();
        }
        else if (Input.GetMouseButtonDown(0) && gameState == GameState.Aim)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rigidbody2D ballCollider = ballList[0].GetComponent<Rigidbody2D>();
            bool aimed = true;

            if (ballCollider.OverlapPoint(mousePosition))
            {
                //do great stuff
                Debug.Log("Launched");

                ShotLine shotLineComponent = shotLine.GetComponent<ShotLine>();
                shotLineComponent.clearLaunchPoint();

                aimReticle.SetActive(false);
                ballList[0].launch();
                SetGameState(GameState.Action);
                aimed = false;
            }

            // set ball's target slope based on input point
            if(aimed)
            {
                Debug.Log("Aimed");
                aimReticle.transform.position = mousePosition;
                aimReticle.SetActive(true);
                ballList[0].setSlopeByInputPoint(mousePosition);
                
            }
        }
    }

    void endGame()
    {
        // dialog, scores, ratings get called here
    }
}
