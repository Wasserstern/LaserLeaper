using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    Collider2D col;
    public Vector2 direction;
    public float bulletSpeed;
    public float bulletDamage;

    public float bulletLifetime;

    [SerializeField]
    float currentLifetime;
    void Start()
    {
        col = GetComponent<Collider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(direction != default){
            transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(direction.x, direction.y, 0), bulletSpeed);
            currentLifetime += Time.deltaTime;
        }

        if(currentLifetime >= bulletLifetime){
            Burst();
        }
    }
     private void OnCollisionEnter2D(Collision2D other){
        if(other.collider.gameObject.layer == LayerMask.NameToLayer("Ground")){
            Burst();
        }
    }

    private void Burst(){
            Destroy(this.gameObject);
    }
}
