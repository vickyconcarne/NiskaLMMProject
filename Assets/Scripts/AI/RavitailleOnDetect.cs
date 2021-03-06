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
    public Animator zoneAnimator;
    public bool canGiveMoney = true;
    private bool canPlaceFill = false;
    private bool exiting = false;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        transitionAnimator.SetTrigger("Enter");
        RandomTileManager.instance.HideMoneyFill();
        yield return new WaitForSeconds(2f);
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
                if(leftHit.collider.GetComponent<CarCollisionManager>() != null)
                {
                    LeftAction(leftHit);
                    isLookingFor = false;
                }
                
                
                return;
            }
            else if (Physics.Raycast(detectionTransform.position, transform.right, out rightHit, distanceToDetect, layerToDetect, QueryTriggerInteraction.Collide))
            {
                if (rightHit.collider.GetComponent<CarCollisionManager>() != null)
                {
                    RightAction(rightHit);
                    isLookingFor = false;
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
            AddMoney();
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position + Vector3.up * 1f, ((float)currentMoney / (float)maxMoney));
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
            AddMoney();
            RandomTileManager.instance.AddMoneyToLevel(moneyToAddPerTick, transform.position + Vector3.up * 1f, ((float)currentMoney / (float)maxMoney));
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
            if(!exiting) StartCoroutine(ExitLevel());
        }
        else
        {
            currentMoney += moneyToAddPerTick;
            zoneAnimator.SetTrigger("Pop");
        }
    }

    public IEnumerator ExitLevel()
    {
        if (!exiting)
        {
            exiting = true;
            transitionAnimator.SetTrigger("Exit");
            Destroy(transform.parent.gameObject, 8f);
            currentTime = 1f;
            while(currentTime > 0)
            {
                RandomTileManager.instance.PlaceMoneyFillOnPosition(transform.position + Vector3.up * 2f + transform.forward * 4f);
                currentTime -= Time.fixedDeltaTime;
                yield return null;
            }
            RandomTileManager.instance.HideMoneyFill();
            
        }
        
    }

    public void ExitLevelImmediately()
    {
        if (exiting) return;
        exiting = true;
        RandomTileManager.instance.HideMoneyFill();
        transitionAnimator.SetTrigger("Exit");
        Destroy(transform.parent.gameObject, 8f);
    }
}
