using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameResource
{
    public enum ResourceType
    {
        Cheese,
        Pepperoni,
        Canadian_Bacon,
        Pineapple,
        None
    }

    private Dictionary<ResourceType,int> resources = new Dictionary<ResourceType,int>();

    public GameResource()
    {
        // init all resources to zero for safety
        SetResourceQuantity(ResourceType.Cheese, 0);
        SetResourceQuantity(ResourceType.Pepperoni, 0);
        SetResourceQuantity(ResourceType.Canadian_Bacon, 0);
        SetResourceQuantity(ResourceType.Pineapple, 0);
    }

    public int GetResourceQuantity(ResourceType resourceType)
    {
        int result = 0;

        if(resources.ContainsKey(resourceType))
        {
            result = resources[resourceType];
        } else {
            // init queried resources that don't exist to zero for double safety
            SetResourceQuantity(resourceType, 0);
        }

        return result;
    }

    public void SetResourceQuantity(ResourceType resourceType, int quantity)
    {
        if(resources.ContainsKey(resourceType))
        {
            resources[resourceType] = quantity;
        } else {
            resources.Add(resourceType, quantity);
        }
    }

    public int ModifyResource(ResourceType resourceType, int quantity, bool sum)
    {
        int resourceRemainder = 0;
        int currentQuantity = GetResourceQuantity(resourceType); // gives back zero and inits if it doesn't exist
        if(sum)
        {
            // adding, only need to check for upper limits if we want them later
            SetResourceQuantity(resourceType, currentQuantity + quantity);
        } else {
            // subtracting, if we reach zero then give back any remaining resources
            resourceRemainder = quantity;
            int curResourceVal = GetResourceQuantity(resourceType);
            while(curResourceVal > 0 && resourceRemainder > 0)
            {
                resourceRemainder--;
                SetResourceQuantity(resourceType, --curResourceVal);
            }
        }

        return resourceRemainder;
    }

    public int allResourceSum()
    {
        return GetResourceQuantity(ResourceType.Cheese) + GetResourceQuantity(ResourceType.Pepperoni) + GetResourceQuantity(ResourceType.Canadian_Bacon) + GetResourceQuantity(ResourceType.Pineapple);
    }

}
