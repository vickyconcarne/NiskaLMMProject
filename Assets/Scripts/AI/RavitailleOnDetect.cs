using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavitailleOnDetect : AOnSideDetection
{

    public int moneyToAddPerTick;
    public int maxMoney;
    private int currentMoney;
    private float currentTime;
    const float timeOfTick = 1f;
    public Vector3 moveVector;
    public CharacterController charController;
    public Animator transitionAnimator;
    // Start is called before the first frame update
    void Start()
    {
        transitionAnimator.SetTrigger("Enter");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit leftHit;
        RaycastHit rightHit;
        charController.Move(moveVector * Time.fixedDeltaTime);

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
        if(currentTime >= timeOfTick)
        {
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position);
            AddMoney();
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.fixedDeltaTime;
        }
        
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        if (currentTime >= timeOfTick)
        {
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position);
            AddMoney();
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.fixedDeltaTime;
        }
    }

    void AddMoney()
    {
        
        if(currentMoney >= maxMoney)
        {
            ExitLevel();
        }
        else
        {
            currentMoney += moneyToAddPerTick;
        }
    }

    public void ExitLevel()
    {
        transitionAnimator.SetTrigger("Exit");
        Destroy(gameObject, 2f);
    }
}
