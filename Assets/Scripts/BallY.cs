using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallY : MonoBehaviour
{
    // Start is called before the first frame update
    Ball ball;
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
        //ball.reverseY();
        Debug.Log("REVERSE Y");
    }
}
