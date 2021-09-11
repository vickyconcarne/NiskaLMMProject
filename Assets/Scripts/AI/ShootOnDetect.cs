using UnityEngine;
using LMM_Movement;
using System.Collections;
using System.Collections.Generic;
public class ShootOnDetect : AOnSideDetection
{

    public GameObject muzzleFX;
    public float speedToMatchPlayer;
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
        SetSpeed(speedToMatchPlayer);
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        SetSpeed(speedToMatchPlayer);
    }

    void InterpretAction(MovementAction givenAction)
    {
        switch (npcController.chosenLane)
        {

        }
    }

    


}

[System.Serializable]
public class MovementAction{
    public lane direction;
    public float speed;
    public float durationOfAction;
}
