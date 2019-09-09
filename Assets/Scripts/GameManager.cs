using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject aimReticle;
    public GameObject shotLine;
    public TargetField targetField;
    public TextMeshProUGUI powerText;

    public TextMeshProUGUI dialogHeaderText;
    public TextMeshProUGUI dialogBodyText;
    public TextMeshProUGUI dialogLeftButtonText;
    public TextMeshProUGUI dialogRightButtonText;

    private int powerLevel = 0;
    public int powerMax = 2;

    private bool awaitingReflect;
    private float sinceReflect;
    private float sinceTap;

    public float timingWindow = 0.25f;
    public float advanceVelocity = 0.02f;

    private List<Target> targetList;
    GameState gameState = GameState.Aim; // starts off in aim state

    private bool gameEnded;
    private bool failed;

    public Canvas UICanvas;

    public enum GameState
    {
        Aim,
        Action,
        End,
        Pause
    }

    public void hideUICanvas()
    {
        UICanvas.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void SetGameState(GameState gameStateIn)
    {
        gameState = gameStateIn;
    }

    // Start is called before the first frame update
    void Start()
    {
        updatePowerLevel(1, true);
        aimReticle.SetActive(false);
    }

    Ball getTargetBall()
    {
        return shotLine.GetComponent<ShotLine>().launchBall.GetComponent<Ball>();
    }

    void updatePowerLevel(int amount, bool add)
    {
        if(add && powerLevel < powerMax)
        {
            powerLevel += amount;
        } else if (!add && powerLevel > 0)
        {
            powerLevel -= amount;
        }

        if(powerLevel > powerMax)
        {
            powerLevel = powerMax;
        } else if (powerLevel < 1)
        {
            powerLevel = 1;
        }

        powerText.text = powerLevel.ToString();

        // also increase velocity
        getTargetBall().v += advanceVelocity;
    }

    public int getPowerLevel()
    {
        return powerLevel;
    }

    // Update is called once per frame
    void Update()
    {
        sinceReflect += Time.deltaTime;
        sinceTap += Time.deltaTime;

        if(awaitingReflect && (sinceTap > timingWindow))
        {
            awaitingReflect = false; // "miss early"
        }

        // check for game enders
        if(targetField.targetsRemaining() < 1)
        {
            gameState = GameState.End;
        }

        if(gameState == GameState.End && !gameEnded)
        {
            gameEnded = true;
            endGame();
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Rigidbody2D ballCollider = getTargetBall().GetComponent<Rigidbody2D>();

        if (Input.GetMouseButtonDown(0))
        {
            if(gameState == GameState.Aim)
            {
                bool aimed = true;

                if (ballCollider.OverlapPoint(mousePosition))
                {
                    //do great stuff
                    Debug.Log("Launched");

                    ShotLine shotLineComponent = shotLine.GetComponent<ShotLine>();
                    shotLineComponent.clearLaunchPoint();

                    aimReticle.SetActive(false);
                    getTargetBall().launch();
                    SetGameState(GameState.Action);
                    aimed = false;
                }

                // set ball's target slope based on input point
                if (aimed)
                {
                    Debug.Log("Aimed");
                    // turn on circle collider to detect launch
                    getTargetBall().setCircleCollider(true);
                    aimReticle.transform.position = mousePosition;
                    aimReticle.SetActive(true);
                    getTargetBall().setSlopeByInputPoint(mousePosition);
                }
            }
            else if (gameState == GameState.Action)
            {
                // clearCircleColliders();
                // check if ball was tapped during action
                if (ballCollider.OverlapPoint(mousePosition))
                {
                    Debug.Log("Tapped Ball In Motion");
                    checkActionBallTap();
                }
            }
        }

        if(gameState == GameState.Action && getBallsInPlay() == 0)
        {
            targetField.advanceField();
            SetGameState(GameState.Aim);
        }
        
    }

    public void fail()
    {
        failed = true;
        gameState = GameState.End;
    }

    public void checkActionBallTap()
    {
        // if the ball reflects off a target within the next delta seconds or has within the last delta seconds
        if(sinceReflect < timingWindow)
        {
            // hit
            updatePowerLevel(1, true);
            awaitingReflect = false;
        } else if (!awaitingReflect)
        {
            awaitingReflect = true;
            sinceTap = 0f;
        }
    }

    public void reflected()
    {
        shotLine.GetComponent<ShotLine>().launchBall.GetComponent<Ball>().setResources(powerLevel);
        sinceReflect = 0f;
        if (awaitingReflect && sinceTap < timingWindow)
        {
            // hit
            updatePowerLevel(1, true);
            awaitingReflect = false;
        }
    }

    public void clearCircleColliders()
    {
        Ball[] balls = GetComponentsInChildren<Ball>();
        for (int i = 0; i < balls.Length; i++) {
            balls[i].setCircleCollider(false);
        }
    }

    public void advanceTargetField()
    {
        targetField.advanceField();
    }

    void endGame()
    {
        // dialog, scores, ratings get called here
        if(gameEnded)
        {
            string header = failed ? "Failure..." : "Victory!";
            string body = failed ? "You can't win em' all and you didn't win this one. Better luck next time." : "The victor writes the history book and this time it's YOU! Get writing, Victor!";
            string left = "Retry";
            string right = "Next";

            showEndGameDialog(header, body, left, right);
        }
    }

    void showEndGameDialog(string header, string body, string leftButton, string rightButton)
    {
        dialogHeaderText.text = header;
        dialogBodyText.text = body;
        dialogLeftButtonText.text = leftButton;
        dialogRightButtonText.text = rightButton;

        UICanvas.gameObject.SetActive(true);
    }

    public int getBallsInPlay()
    {
        int result = 0;
        Ball[] balls =  GetComponentsInChildren<Ball>();
        for(int i = 0; i < balls.Length; i++)
        {
            result++;
        }

        if(shotLine.GetComponent<ShotLine>().isLaunchPointSet())
        {
            result--;
        }

        return result;
    }
}
