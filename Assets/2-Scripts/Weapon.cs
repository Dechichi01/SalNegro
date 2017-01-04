using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class Weapon : MonoBehaviour {

    public float damage;
    public LayerMask collisionMask;

    BoxCollider2D boxColl;
    // Use this for initialization
    protected void Awake()
    {
        boxColl = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update () {
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxColl.bounds.center, boxColl.bounds.size, 0f, collisionMask);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].name);
                LivingEntity livingEntity = hits[i].transform.root.GetComponent<LivingEntity>();
                if (livingEntity)
                    livingEntity.TakeDamage(damage);
            }
            this.enabled = false;
        }
	}
}
