using UnityEngine;
using System.Collections;

public class ClimbTrigger : MonoBehaviour {

    AnimController2D animCtrl;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        transform.parent.GetComponent<AnimController2D>().ClimbLadder();
    }
}
