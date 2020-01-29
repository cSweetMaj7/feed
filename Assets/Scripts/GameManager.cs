using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Tooltip("The one and only game camera.")]
    public Camera mainCamera;

    [Tooltip("The game object which has the aim reticle sprite. Used for aiming.")]
    public GameObject aimReticle;

    [Tooltip("The game object that contains the ShotLine component. The ShotLine " +
        "is normally attached to the bottom playfield boundary. Its function is to serve " +
        "as a launching point for the ball as well as catch the ball, set it up for" +
        " the next shot and dispose of clone balls.")]
    public GameObject shotLine;

    [Tooltip("Reference to the TargetField, which contains and controls all the Targets on the playfield.")]
    public TargetField targetField;

    [Tooltip("Reference to the text that displays the current power level.")]
    public TextMeshProUGUI powerText;

    [Tooltip("Reference to the sound manager.")]
    public SoundManager soundManager;

    [Tooltip("The game object that contains the background sprite.")]
    public GameObject background;

    [Tooltip("Text that appears at the top of the dialog.")]
    public TextMeshProUGUI dialogHeaderText;

    [Tooltip("Text that appears as the body of the dialog.")]
    public TextMeshProUGUI dialogBodyText;

    [Tooltip("Left button text.")]
    public TextMeshProUGUI dialogLeftButtonText;

    [Tooltip("Right button text.")]
    public TextMeshProUGUI dialogRightButtonText;

    [Tooltip("Sets the maximum that power level can grow to. Also represents the level at which splitting is turned on.")]
    public int powerMax = 2;

    [Tooltip("Sets how many seconds to remain at max level once reached, then reduce back to 1.")]
    public float maxLevelTime = 3.0f;

    [Tooltip("Sets the number of seconds, before and after a reflection, that a ball tap will be accepted. Lower is harder, bigger is easier.")]
    public float timingWindow = 0.25f;

    [Tooltip("Sets how much to advance ball velocity by each time power levels up.")]
    public float advanceVelocity = 0.02f;

    [Tooltip("A reference to the UI Canvas.")]
    public Canvas UICanvas;

    [Tooltip("Sets how far, vertically, background is bounced on each beat. Bigger = motion sickness.")]
    public float backgroundBounceAmount = 0.25f;

    [Tooltip("The current game state.")]
    public GameState gameState = GameState.Aim; // starts off in aim state

    [Tooltip("Sets the number of frames to freeze on freeze actions.")]
    public int freezeActionFrames = 5;

    private int powerLevel; // keeps track of the current power level
	private bool reachedMax; // true when the current power level is the maximum level
	private float sinceLevelLowered; // seconds since we started lowering the level
    private bool awaitingReflect; // used for validating early inputs to ball tapping
    private float sinceReflect; // used for validating late inputs to ball tapping, seconds since the reflect
    private float sinceTap; // used for validating ball taps, seconds since input

    private bool gameEnded; // local switch for knowing if the game has ended or not
    private bool failed; // set on game end, whether or not the player has failed
    private int frozeActionFrames;
    
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    private bool aimed;
    private bool startedMusic;
    private bool bgBounced;
    private float sinceStart;

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
        if (add && powerLevel < powerMax)
        {
            soundManager.playBallTap();
            powerLevel += amount;
        } else if (!add && powerLevel > 0)
        {
            powerLevel -= amount;
        }

        if (powerLevel >= powerMax)
        {
            powerLevel = powerMax;
			sinceLevelLowered = 0;
			reachedMax = true;
        } else if (powerLevel < 1)
        {
            powerLevel = 1;
        }

        powerText.text = powerLevel.ToString();

        // also increase velocity
        getTargetBall().v += advanceVelocity;

        // set the music once we get to max power
        if(powerLevel == powerMax)
        {
            SoundManager.Instance.startMusicFast();
        }
    }

    public int getPowerLevel()
    {
        return powerLevel;
    }

    // Update is called once per frame
    void Update()
    {
        sinceStart += Time.deltaTime;
        // delay starting music to avoid weird load issues with beat tracking
        if(sinceStart > 1.0f && !startedMusic)
        {
            soundManager.startMusicSlow();
            startedMusic = true;
        }

		checkLowerPowerLevel();

        sinceReflect += Time.deltaTime;
        sinceTap += Time.deltaTime;
        if(frozeActionFrames > 0)
        {
            frozeActionFrames++;
            unfreezeAction();
        }

        if (awaitingReflect && (sinceTap > timingWindow))
        {
            awaitingReflect = false; // "miss early"
        }

        // check for game enders
        if (targetField.targetsRemaining() < 1)
        {
            gameState = GameState.End;
        }

        if (gameState == GameState.End && !gameEnded)
        {
            gameEnded = true;
            endGame();
        }

        Rigidbody2D ballCollider = null;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ball targetBall = getTargetBall();
        if(targetBall)
        {
            ballCollider = getTargetBall().GetComponent<Rigidbody2D>();
        }
        
		// instead of GetMouseButtonDown start looking at touches and figuring out what we need to do
		// use mouse clicks when in eidtor. Otherwise fall back to touches, which is what will be used on-device

		if (ballCollider && Input.GetMouseButtonDown(0))
		{
			if (gameState == GameState.Aim)
			{
				aimed = true;

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

        /*
		if (Application.isEditor)
        {
            
        } else
        {
            if (Input.touches.Length > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchStartPos = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // moving...
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {

                        touchEndPos = touch.position;
                        // touch/swipe is over, figure out what it was and do it
                        if (touchStartPos == touchEndPos)
                        {
                            // a tap, aim the ball
                            aimed = true;
                        }
                        else if (touchStartPos.y < touchEndPos.y)
                        {
                            // swipe up, launch
                            aimed = false;
                        }

                        // execution
                        if (gameState == GameState.Aim)
                        {
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

                        touchStartPos = Vector2.zero;
                        touchEndPos = Vector2.zero;
                    }
                }
            }
        }
        


        if (Input.GetMouseButtonDown(0))
        {
            
        }*/

        if (gameState == GameState.Action && getBallsInPlay() == 0)
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
        if (sinceReflect < timingWindow)
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
        if (gameEnded)
        {
            string header = failed ? "Failure..." : "Victory!";
            string body = failed ? "You can't win em' all and you didn't win this one. Better luck next time." : "The victor writes the history book and this time it's YOU! Get to writing, Victor!";
            string left = "Retry";
            string right = "Next";

            showEndGameDialog(header, body, left, right);
            SoundManager.Instance.startEndMusic();
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
        Ball[] balls = GetComponentsInChildren<Ball>();
        for (int i = 0; i < balls.Length; i++)
        {
            result++;
        }

        if (shotLine.GetComponent<ShotLine>().isLaunchPointSet())
        {
            result--;
        }

        return result;
    }

    // locks the position of all balls on the field
    // 
    public void freezeAction()
    {
        frozeActionFrames = 1;
    }

    void unfreezeAction()
    {
        if(frozeActionFrames >= freezeActionFrames)
        {
            // unfreeze
            frozeActionFrames = 0;
        }
    }

    public void bounceBackground()
    {
        Vector3 pos = background.transform.position;
        if(bgBounced)
        {
            // move it back down
            pos.y -= backgroundBounceAmount;
        } else
        {
            // bounce it up
            pos.y += backgroundBounceAmount;
        }
		//background.transform.position = pos;
		iTween.MoveTo(background, pos, 0.4f);
		bgBounced = !bgBounced;
    }

    private void checkLowerPowerLevel()
	{
		sinceLevelLowered += Time.deltaTime;

        if(reachedMax && sinceLevelLowered >= maxLevelTime)
		{
			sinceLevelLowered = 0;
			updatePowerLevel(powerLevel - 1, false);
		}

        if(powerLevel < powerMax && reachedMax)
		{
            // remove all but the launch ball
            // lowered back to 1, turn off "hyper" mode
            Ball[] balls = GetComponentsInChildren<Ball>();
            Debug.Log("launch ball already set, get rid of the rest");
            for (int i = 0; i < balls.Length; i++)
            {
                if (!balls[i].isLaunchBall)
                {
                    GameObject.Destroy(balls[i].gameObject);
                }
            }

            for (int i = 0; i < balls.Length; i++)
			{
				balls[i].v = 0.1f;
			}
			reachedMax = false;
			soundManager.startMusicSlow();
		}
	}
}
