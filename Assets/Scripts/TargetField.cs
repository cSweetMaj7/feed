using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetField : MonoBehaviour
{
    public Target[] targetList;
    public GameObject advanceLimiter;

    bool fieldAdvancing;

    public float advanceByFrame = -0.1f;
    public float advanceEachTurn = 1.0f; //abs

    Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        targetList = GetComponentsInChildren<Target>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fieldAdvancing)
        {
            float newY = transform.position.y + advanceByFrame;
            if(Mathf.Abs(startPos.y - newY) >= advanceEachTurn)
            {
                fieldAdvancing = false;
            } else
            {
                transform.position = new Vector2(transform.position.x, newY);
            }

            // check if game ends
            if(getLowestTargetY() <= 200f)
            {
                fieldAdvancing = false;
                GetComponentInParent<GameManager>().fail();
            }
        }

        
    }

    float getLowestTargetY()
    {
        float result = 999.0f;

        for(int i = 0; i < targetList.Length; i++)
        {
            if(targetList[i])
            {
                Vector3 point = GetComponentInParent<GameManager>().mainCamera.WorldToScreenPoint(targetList[i].transform.position);

                if (point.y < result)
                {
                    result = point.y;
                    break;
                }
            }
            
        }

        return result;
    }

    public int targetsRemaining()
    {
        int count = 0;
        for(int i = 0; i < targetList.Length; i++)
        {
            if(targetList[i])
            {
                count++;
            }
        }
        return count;
    }

    public void advanceField()
    {
        startPos = transform.position;
        fieldAdvancing = true;
    }
}
