using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Target : Barrier
{
    
    public int cheese;
    public int pepperoni;
    public int canadianBacon;
    public int pineapple;

    public TextMeshPro cheeseText;
    public TextMeshPro pepperoniText;
    public TextMeshPro canadianBaconText;
    public TextMeshPro pineappleText;

    // these are the resource amounts needed to destroy target
    public GameResource breakResources;

    // Start is called before the first frame update
    void Start()
    {
        // move the inspector set amounts into the GameResource
        breakResources = new GameResource();
        breakResources.ModifyResource(GameResource.ResourceType.Cheese, cheese, true);
        breakResources.ModifyResource(GameResource.ResourceType.Pepperoni, pepperoni, true);
        breakResources.ModifyResource(GameResource.ResourceType.Canadian_Bacon, canadianBacon, true);
        breakResources.ModifyResource(GameResource.ResourceType.Pineapple, pineapple, true);
        updateTextValues();
    }

    public void updateTextValues()
    {
        cheeseText.text = breakResources.GetResourceQuantity(GameResource.ResourceType.Cheese).ToString();
        pepperoniText.text = breakResources.GetResourceQuantity(GameResource.ResourceType.Pepperoni).ToString();
        canadianBaconText.text = breakResources.GetResourceQuantity(GameResource.ResourceType.Canadian_Bacon).ToString();
        pineappleText.text = breakResources.GetResourceQuantity(GameResource.ResourceType.Pineapple).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deductResources(GameResource ballResources)
    {
        //breakResources.SetResourceQuantity(GameResource.ResourceType.Cheese, ballResources.ModifyResource(GameResource.ResourceType.Cheese, breakResources)
        //ballResources.SetResourceQuantity(
        //GameResource.ResourceType.Cheese,
        breakResources.ModifyResource(
               GameResource.ResourceType.Cheese, ballResources.GetResourceQuantity(GameResource.ResourceType.Cheese
        ), false);//);

        //ballResources.SetResourceQuantity(
        // GameResource.ResourceType.Pepperoni,
        breakResources.ModifyResource(
               GameResource.ResourceType.Pepperoni, ballResources.GetResourceQuantity(GameResource.ResourceType.Pepperoni
        ), false);//);

        //ballResources.SetResourceQuantity(
        //GameResource.ResourceType.Canadian_Bacon,
        breakResources.ModifyResource(
               GameResource.ResourceType.Canadian_Bacon, ballResources.GetResourceQuantity(GameResource.ResourceType.Canadian_Bacon
        ), false);//);

        //ballResources.SetResourceQuantity(
        //GameResource.ResourceType.Pineapple,
        breakResources.ModifyResource(
               GameResource.ResourceType.Pineapple, ballResources.GetResourceQuantity(GameResource.ResourceType.Pineapple
        ), false);//);

        updateTextValues();
    }

    override public BarrierType getBarrierType()
    {
        return Barrier.BarrierType.Target;
    }
}
