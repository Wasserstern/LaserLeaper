using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ammo : MonoBehaviour{

    public int ammo;
    Animator animator;
    Collider2D col;
    private void Start(){
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update(){
        /*
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("PickedUp")){
            Destroy(this.gameObject);
        }
        */

    }

    public void Pickup(){

        animator.SetTrigger("Pickup");
        col.enabled = false;
    }
}