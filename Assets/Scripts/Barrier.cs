using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public enum BarrierType {
        Barrier,
        Target,
        ShotLine
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual public BarrierType getBarrierType()
    {
        return BarrierType.Barrier;
    }
}
