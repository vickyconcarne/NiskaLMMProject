using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCounter : MonoBehaviour
{

    public RandomTileManager tileManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.tag == "OtherCar")
        {
            tileManager.currentCountedCars += 1;
            tileManager.CheckState();
        }
    }
}
