using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Barrier
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    override public BarrierType getBarrierType()
    {
        return Barrier.BarrierType.Target;
    }
}
