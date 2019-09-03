using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetField : MonoBehaviour
{
    public List<Target> targetList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int targetsRemaining()
    {
        int count = 0;
        for(int i = 0; i < targetList.Count; i++)
        {
            if(targetList[i])
            {
                count++;
            }
        }
        return count;
    }
}
