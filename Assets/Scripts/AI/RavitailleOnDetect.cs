using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RavitailleOnDetect : AOnSideDetection
{

    public int moneyToAddPerTick;
    public int maxMoney;
    private int currentMoney = 0;
    private float currentTime;
    const float timeOfTick = 1f;
    public Vector3 moveVector;
    public CharacterController charController;
    public Animator transitionAnimator;
    private bool canGiveMoney = true;
    private bool canPlaceFill = false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        transitionAnimator.SetTrigger("Enter");
        yield return new WaitForSeconds(1f);
        canPlaceFill = true;
    }

    private void LateUpdate()
    {
        if (canPlaceFill && canGiveMoney) RandomTileManager.instance.PlaceMoneyFillOnPosition(transform.position + Vector3.up * 2f + transform.forward * 4f);
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
        
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        if(currentTime >= timeOfTick)
        {
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position + Vector3.up * 1f, ((float)currentMoney / (float)maxMoney));
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
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position + Vector3.up * 1f, ((float)currentMoney / (float)maxMoney));
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
        Destroy(transform.parent.gameObject, 13f);
    }
}
