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

    public TextMeshPro slot0Text;
    public TextMeshPro slot1Text;
    public TextMeshPro slot2Text;

	public GameResource.ResourceType slot0ResourceType;
	public GameResource.ResourceType slot1ResourceType;
	public GameResource.ResourceType slot2ResourceType;

    public GameObject slot0;
    public GameObject slot1;
    public GameObject slot2;

    // these are the resource amounts needed to destroy target
    public GameResource breakResources;

    // Start is called before the first frame update
    override public void Start()
    {
        breakResources = new GameResource();
        updateSlots();
    }

    void updateSlots()
    {
        if(slot0ResourceType != GameResource.ResourceType.None)
        {
            TypeIcons slot0TypeIcons = slot0.GetComponentInChildren<TypeIcons>();
            slot0TypeIcons.setIconViewByResourceType(slot0ResourceType);
            setTextByResourceType(ref slot0Text, slot0ResourceType);
        } else
        {
            slot0.SetActive(false);
        }
        
        if(slot1ResourceType != GameResource.ResourceType.None)
        {
            TypeIcons slot1TypeIcons = slot1.GetComponentInChildren<TypeIcons>();
            slot1TypeIcons.setIconViewByResourceType(slot1ResourceType);
            setTextByResourceType(ref slot1Text, slot1ResourceType);
        } else
        {
            slot1.SetActive(false);
        }

        if(slot2ResourceType != GameResource.ResourceType.None)
        {
            TypeIcons slot2TypeIcons = slot2.GetComponentInChildren<TypeIcons>();
            slot2TypeIcons.setIconViewByResourceType(slot2ResourceType);
            setTextByResourceType(ref slot2Text, slot2ResourceType);
        } else
        {
            slot2.SetActive(false);
        }
        
    }

    void setTextByResourceType(ref TextMeshPro textMeshPro, GameResource.ResourceType resourceType)
    {
        switch (resourceType)
        {
            case GameResource.ResourceType.Cheese:
                textMeshPro.color = new Color32(255, 217, 0, 255);
                textMeshPro.text = cheese.ToString();
                breakResources.SetResourceQuantity(GameResource.ResourceType.Cheese, cheese);
                break;

            case GameResource.ResourceType.Pepperoni:
                textMeshPro.color = new Color32(255, 83, 43, 255);
                textMeshPro.text = pepperoni.ToString();
                breakResources.SetResourceQuantity(GameResource.ResourceType.Pepperoni, pepperoni);
                break;

            case GameResource.ResourceType.Canadian_Bacon:
                textMeshPro.color = new Color32(248, 160, 158, 255);
                textMeshPro.text = canadianBacon.ToString();
                breakResources.SetResourceQuantity(GameResource.ResourceType.Canadian_Bacon, canadianBacon);
                break;

            case GameResource.ResourceType.Pineapple:
                textMeshPro.color = new Color32(114, 207, 127, 255);
                textMeshPro.text = pineapple.ToString();
                breakResources.SetResourceQuantity(GameResource.ResourceType.Pineapple, pineapple);
                break;
        }
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
        cheese = breakResources.GetResourceQuantity(GameResource.ResourceType.Cheese);

        //ballResources.SetResourceQuantity(
        // GameResource.ResourceType.Pepperoni,
        breakResources.ModifyResource(
               GameResource.ResourceType.Pepperoni, ballResources.GetResourceQuantity(GameResource.ResourceType.Pepperoni
        ), false);//);
        pepperoni = breakResources.GetResourceQuantity(GameResource.ResourceType.Pepperoni);

        //ballResources.SetResourceQuantity(
        //GameResource.ResourceType.Canadian_Bacon,
        breakResources.ModifyResource(
               GameResource.ResourceType.Canadian_Bacon, ballResources.GetResourceQuantity(GameResource.ResourceType.Canadian_Bacon
        ), false);//);
        canadianBacon = breakResources.GetResourceQuantity(GameResource.ResourceType.Canadian_Bacon);

        //ballResources.SetResourceQuantity(
        //GameResource.ResourceType.Pineapple,
        breakResources.ModifyResource(
               GameResource.ResourceType.Pineapple, ballResources.GetResourceQuantity(GameResource.ResourceType.Pineapple
        ), false);//);
        pineapple = breakResources.GetResourceQuantity(GameResource.ResourceType.Pineapple);

        updateSlots();
    }

    override public BarrierType getBarrierType()
    {
        return Barrier.BarrierType.Target;
    }
}
