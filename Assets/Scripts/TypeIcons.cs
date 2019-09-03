using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeIcons : MonoBehaviour
{
    public GameObject cheeseIcon;
    public GameObject pepperoniIcon;
    public GameObject canadianBaconIcon;
    public GameObject pineappleIcon;

    // Start is called before the first frame update
    void Start()
    {
        /*cheeseIcon.SetActive(false);
        pepperoniIcon.SetActive(false);
        canadianBaconIcon.SetActive(false);
        pineappleIcon.SetActive(false);*/
    }

    public void setIconViewByResourceType(GameResource.ResourceType resourceType)
    {
        switch(resourceType)
        {
            case GameResource.ResourceType.Cheese:
                cheeseIcon.SetActive(true);
                break;

            case GameResource.ResourceType.Pepperoni:
                pepperoniIcon.SetActive(true);
                break;

            case GameResource.ResourceType.Canadian_Bacon:
                canadianBaconIcon.SetActive(true);
                break;

            case GameResource.ResourceType.Pineapple:
                pineappleIcon.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
