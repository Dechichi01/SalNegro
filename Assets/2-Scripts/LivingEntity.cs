using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public LivingEntityStates states;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        states = new LivingEntityStates();
        health = startingHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection, float amountToFend = 0)
    {
        //TODO: Some stuffs with hit

        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
            OnDeath = null;//Make all methods unsubscribe from OnDeath
        }
        Destroy(gameObject);

    }
}

[System.Serializable]
public class LivingEntityStates
{
    public bool canMove = true;
    public bool canAttack = true;
    public bool canPerformAction = true;
    public bool facingRight = true;
    public bool grounded;

    public bool useGravity = true;
    public bool checkCollisions = true;

    public bool isRolling = false;
    public bool isAttacking = false;
    public bool isClimbing = false;

    public void Copy(LivingEntityStates states)
    {
        canMove = states.canMove;
        canAttack = states.canAttack;
        canPerformAction = states.canPerformAction;
        useGravity = states.useGravity;
        checkCollisions = states.checkCollisions;

        isRolling = states.isRolling;
        isAttacking = states.isAttacking;
        isClimbing = states.isClimbing;
    }
}
