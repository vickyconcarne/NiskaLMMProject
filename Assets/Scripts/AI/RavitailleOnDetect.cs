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
    private bool canGiveMoney = true;
    private bool hasPlacedFill = false;
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

        if (canGiveMoney)
        {
            if (Physics.Raycast(detectionTransform.position, -transform.right, out leftHit, distanceToDetect, layerToDetect, QueryTriggerInteraction.Collide))
            {
                LeftAction(leftHit);
                isLookingFor = false;
                if (!hasPlacedFill)
                {
                    RandomTileManager.instance.PlaceMoneyFillOnPosition(transform.position+ Vector3.up * 2f);
                    hasPlacedFill = true;
                }
                return;
            }
            else if (Physics.Raycast(detectionTransform.position, transform.right, out rightHit, distanceToDetect, layerToDetect, QueryTriggerInteraction.Collide))
            {
                RightAction(rightHit);
                isLookingFor = false;
                if (!hasPlacedFill)
                {
                    RandomTileManager.instance.PlaceMoneyFillOnPosition(transform.position + Vector3.up * 2f);
                    hasPlacedFill = true;
                }
                return;
            }
            else
            {
                isLookingFor = true;
            }
        }
        
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        if(currentTime >= timeOfTick)
        {
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position, 1f-currentMoney/maxMoney);
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
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position, 1f - currentMoney / maxMoney);
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
            canGiveMoney = false;
            ExitLevel();
        }
        else
        {
            currentMoney += moneyToAddPerTick;
        }
    }

    public void ExitLevel()
    {
        RandomTileManager.instance.HideMoneyFill();
        transitionAnimator.SetTrigger("Exit");
        Destroy(gameObject, 4f);
    }
}
