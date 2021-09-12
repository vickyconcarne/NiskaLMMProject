using UnityEngine;
using LMM_Movement;
using System.Collections;
using System.Collections.Generic;
public class ShootOnDetect : AOnSideDetection
{

    public GameObject muzzleFX;
    public float speedToMatchPlayer;

    public float shootCooldown;
    private float currentCooldown;

    private float shootDirection;
    private GameObject bulletPrefab;
    const float modelDifferential = 0.5f; //To know how far we instantiate bullet and vfx from center of model

    // Start is called before the first frame update
    void Start()
    {
        bulletPrefab = Resources.Load("Bullet") as GameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLookingFor)
        {
            if(currentCooldown >= shootCooldown && npcController.canMove)
            {
                Shoot();
            }
            else
            {
                currentCooldown += Time.fixedDeltaTime;
            }
        }

        if (isLookingFor)
        {
            Debug.DrawRay(transform.position, -transform.right * distanceToDetect, Color.yellow);
            Debug.DrawRay(transform.position, transform.right * distanceToDetect, Color.yellow);
            SideDetection();
        }
        else
        {
            Debug.DrawRay(transform.position, -transform.right * distanceToDetect, Color.green);
            Debug.DrawRay(transform.position, transform.right * distanceToDetect, Color.green);
        }

    }

    public override void LeftAction(RaycastHit hitInfo)
    {
        SetSpeed(speedToMatchPlayer);
        shootDirection = -1f;
    }

    public override void RightAction(RaycastHit hitInfo)
    {
        SetSpeed(speedToMatchPlayer);
        shootDirection = 1f;
    }

    void InterpretAction(MovementAction givenAction)
    {
        switch (npcController.chosenLane)
        {

        }
    }

    void Shoot()
    {
        if (muzzleFX) Instantiate(muzzleFX, transform.position + modelDifferential * shootDirection * transform.right, transform.rotation);
        GameObject bulletObj = GameObject.Instantiate(bulletPrefab, transform.position + modelDifferential * shootDirection * transform.right, transform.rotation);
        Vector3 givenVector = new Vector3(shootDirection * bulletObj.GetComponent<Bullet>().lateralBulletSpeed, 0f, speedToMatchPlayer);
        bulletObj.GetComponent<Bullet>().direction = givenVector;
        currentCooldown = 0f;
    }

    


}

[System.Serializable]
public class MovementAction{
    public lane direction;
    public float speed;
    public float durationOfAction;
}
