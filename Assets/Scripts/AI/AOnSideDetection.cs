using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LMM_Movement;
public abstract class AOnSideDetection : MonoBehaviour
{

    [SerializeField] protected NPCCarController npcController;

    [Header("Detection")]
    public bool detecting = true;
    public float distanceToDetect;
    public LayerMask layerToDetect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SideDetection()
    {
        RaycastHit leftHit;
        RaycastHit rightHit;

        if (Physics.Raycast(transform.position, -transform.right, out leftHit, distanceToDetect, layerToDetect))
        {
            LeftAction(leftHit);
            detecting = false;
            return;
        }
        else if (Physics.Raycast(transform.position, transform.right, out rightHit, distanceToDetect, layerToDetect))
        {
            RightAction(rightHit);
            detecting = false;
            return;
        }
    }

    protected void SetSpeed(float speed)
    {
        npcController.k_movementDirection.z = speed;
    }
    public abstract void LeftAction(RaycastHit hitInfo);

    public abstract void RightAction(RaycastHit hitInfo);
}
