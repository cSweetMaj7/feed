using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallX : MonoBehaviour
{
    Ball ball;
    // Start is called before the first frame update
    void Start()
    {
        ball = GetComponentInParent<Ball>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ball.reverseX();
        Debug.Log("REVERSE X");
    }
}
