using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Weapon : MonoBehaviour {

    public float Damage;
    public LayerMask collisionMask;

    RaycastController raycastCtrl;
    // Use this for initialization
    private void Awake()
    {
        raycastCtrl = GetComponent<RaycastController>();      
    }

    // Update is called once per frame
    void Update () {
        raycastCtrl.UpdateRaycastOrigins();
        for (int i = 0; i < raycastCtrl.horizontalRayCount; i++)
        {

        }
	}
}
