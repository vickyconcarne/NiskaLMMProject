using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavitailleOnDetect : AOnSideDetection
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit leftHit;
        RaycastHit rightHit;

        if (Physics.Raycast(detectionTransform.position, -transform.right, out leftHit, distanceToDetect, layerToDetect, QueryTriggerInteraction.Collide))
        {
            LeftAction(leftHit);
            isLookingFor = false;
            return;
        }
        else if (Physics.Raycast(detectionTransform.position, transform.right, out rightHit, distanceToDetect, layerToDetect, QueryTriggerInteraction.Collide))
        {
            RightAction(rightHit);
            isLookingFor = false;
            return;
        }
        else
        {
            isLookingFor = true;
        }
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        
    }

    public override void RightAction(RaycastHit hitInfo)
    {

    }
}
