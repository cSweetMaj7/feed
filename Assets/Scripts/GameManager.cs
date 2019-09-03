using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera mainCamera;
    public Ball[] ballList;
    public GameObject targetField;
    public GameObject aimReticle;
    public GameObject shotLine;

    private List<Target> targetList;
    bool launched = false;

    // Start is called before the first frame update
    void Start()
    {
        aimReticle.SetActive(false);
    }

    public void setLaunched(bool itIs)
    {
        launched = itIs;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Rigidbody2D ballCollider = ballList[0].GetComponent<Rigidbody2D>();
            bool aimed = true;

            if (ballCollider.OverlapPoint(mousePosition) && !launched)
            {
                //do great stuff
                Debug.Log("Launched");

                ShotLine shotLineComponent = shotLine.GetComponent<ShotLine>();
                shotLineComponent.clearLaunchPoint();

                aimReticle.SetActive(false);
                ballList[0].launch();
                aimed = false;
                launched = true;
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
}
