using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;

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
            NPCCarController carNPC = col.GetComponent<NPCCarController>();
            if (carNPC)
            {
                if (carNPC.canMove)
                {
                    carNPC.Kill(lane.nolane, true);
                }
            }
        }
    }
}
