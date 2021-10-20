using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private CharacterController bulletController;

    [HideInInspector] public Vector3 direction;
    public float lateralBulletSpeed;

    // Start is called before the first frame update

    private void Start()
    {
        bulletController = GetComponent<CharacterController>();
    }
    void FixedUpdate()
    {
        if (bulletController)
        {
            bulletController.Move(direction * Time.fixedDeltaTime);
        }
    }


}
