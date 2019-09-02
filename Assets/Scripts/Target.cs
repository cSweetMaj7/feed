using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Barrier
{
    
    public int cheese;
    public int pepperoni;
    public int canadianBacon;
    public int pineapple;
    // these are the resource amounts needed to destroy target
    public GameResource breakResources;

    // Start is called before the first frame update
    void Start()
    {
        // move the inspector set amounts into the GameResource
        breakResources = new GameResource();
        breakResources.ModifyResource(GameResource.ResourceType.Cheese, cheese, true);
        breakResources.ModifyResource(GameResource.ResourceType.Pepperoni, pepperoni, true);
        breakResources.ModifyResource(GameResource.ResourceType.Cheese, canadianBacon, true);
        breakResources.ModifyResource(GameResource.ResourceType.Cheese, pineapple, true);
    }

    // Update is called once per frame
    void Update()
    {
        if(breakResources.allResourceSum() < 1)
        {
            // target destroyed
            Destroy(gameObject);
        }
    }

    public void deductResources(GameResource ballResources)
    {
        //breakResources.SetResourceQuantity(GameResource.ResourceType.Cheese, ballResources.ModifyResource(GameResource.ResourceType.Cheese, breakResources)
        ballResources.SetResourceQuantity(
            GameResource.ResourceType.Cheese,
            breakResources.ModifyResource(
                   GameResource.ResourceType.Cheese, ballResources.GetResourceQuantity(GameResource.ResourceType.Cheese
            ), false));

        ballResources.SetResourceQuantity(
            GameResource.ResourceType.Pepperoni,
            breakResources.ModifyResource(
                   GameResource.ResourceType.Pepperoni, ballResources.GetResourceQuantity(GameResource.ResourceType.Pepperoni
            ), false));

        ballResources.SetResourceQuantity(
            GameResource.ResourceType.Canadian_Bacon,
            breakResources.ModifyResource(
                   GameResource.ResourceType.Canadian_Bacon, ballResources.GetResourceQuantity(GameResource.ResourceType.Canadian_Bacon
            ), false));

        ballResources.SetResourceQuantity(
            GameResource.ResourceType.Pineapple,
            breakResources.ModifyResource(
                   GameResource.ResourceType.Pineapple, ballResources.GetResourceQuantity(GameResource.ResourceType.Pineapple
            ), false));
    }

    override public BarrierType getBarrierType()
    {
        return Barrier.BarrierType.Target;
    }
}
