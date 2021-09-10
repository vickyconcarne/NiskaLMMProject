using UnityEngine;
using LMM_Movement;
using System.Collections;
using System.Collections.Generic;
public class SequenceActionOnDetect : AOnSideDetection
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detecting)
        {
            SideDetection();
        }
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        
    }

    void InterpretAction(MovementAction givenAction)
    {

    }


}

[System.Serializable]
public class MovementAction{
    public lane direction;
    public float speed;
    public float durationOfAction;
}
