using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCounter : MonoBehaviour
{

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
            RandomTileManager.instance.currentCountedCars += 1;
            RandomTileManager.instance.CheckState();
        }
    }
}
