using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {

    public LayerMask collisionMask;

    public float rotateSpeed = 80f;
    public float pickupDistTreshold = .3f;

    protected RaycastHit2D hit;

    protected virtual void Update()
    {
        Rotate();

        hit = CheckSurroundings();

        if (hit)
            if (Mathf.Abs(transform.position.x - hit.transform.position.x) < pickupDistTreshold) GetPicked();
    }

    protected RaycastHit2D CheckSurroundings()
    {
        return Physics2D.BoxCast(transform.position, new Vector2(1, .3f) * 30f, 0f, Vector3.forward, 30f, collisionMask);
    }

    protected void Rotate()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y + rotateSpeed * Time.deltaTime, rot.z);
    }

    protected virtual void GetPicked()
    {
        gameObject.SetActive(false);
    }

}
