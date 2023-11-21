using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    Collider2D col;
    Rigidbody2D rgbd;

    Camera mainCamera;
    public GameObject bulletPrefab;
    public Transform shootDirector;

    public Transform ammoDisplay;

    // Weapon settings

    public int maxAmmunition;
    public float reloadPerSecond;
    public float shotsPerSecond;
    public float shotForce;
    public float bulletSize;
    public float bulletDamage;
    public float bulletSpeed;

    [SerializeField]
    int currentAmmunition;

    // Velocity settings
    public float maxVelocity;

    // Timers
    public float shockTime;

    // Other

    public float groundCheckDistance;
    

    [SerializeField]
    float timeSinceLastShot;
    float timeSinceLastReload;


    // States
    public bool infiniteAmmo;
    [SerializeField]
    bool holdingShoot;
    [SerializeField]
    bool isShocking;
    [SerializeField]
    bool isGrounded;
    
    Vector2 currentDirectorPosition;
    void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        GetInput();
        // Check ground
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, (new Vector2(transform.position.x, transform.position.y -1) - (Vector2)transform.position).normalized, groundCheckDistance, LayerMask.GetMask("Ground"));
        if(groundHit.collider != null) {
            isGrounded = true;
        }
        else{
            isGrounded = false;
        }

        

        // Set director
        currentDirectorPosition = ((Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        shootDirector.transform.position = (Vector2)transform.position + currentDirectorPosition;

        if(!isShocking){ // Prevent actions if player is shocked

            if(holdingShoot){ 

                Vector2 shootDirection = ((Vector2)shootDirector.position - (Vector2)transform.position).normalized;
                if(timeSinceLastShot >= 1f / shotsPerSecond && currentAmmunition > 0){
                    timeSinceLastShot = 0f;

                    // Instantiate new bullet
                    GameObject newBullet = GameObject.Instantiate(bulletPrefab);
                    newBullet.transform.position = transform.position;
                    Bullet bulletInstance = newBullet.GetComponent<Bullet>();
                    bulletInstance.bulletDamage = bulletDamage;
                    bulletInstance.bulletSpeed = bulletSpeed;
                    newBullet.transform.localScale = new Vector3(bulletSize, bulletSize, bulletSize);
                    bulletInstance.direction = shootDirection;

                    // Apply force to player

                    rgbd.AddForce(-shootDirection * shotForce, ForceMode2D.Impulse);
                    
                    if(!infiniteAmmo){
                        currentAmmunition--;
                    }
                }
            }
            if(isGrounded){
            if(timeSinceLastReload >= 1f / reloadPerSecond && currentAmmunition < maxAmmunition){
                timeSinceLastReload = 0f;
                currentAmmunition++;
            }
        }
        }
        

        if(rgbd.velocity.magnitude >= maxVelocity){
            // Limit velocity
            rgbd.velocity = rgbd.velocity.normalized * maxVelocity;
        }
        timeSinceLastShot += Time.deltaTime;
        timeSinceLastReload += Time.deltaTime;
        ammoDisplay.transform.position = new Vector2(transform.position.x, transform.position.y + 1f);
        ammoDisplay.transform.localScale = new Vector3(1.5f * currentAmmunition / maxAmmunition, 0.1f, 0.1f);
    }

    void GetInput(){
        holdingShoot = Input.GetKey(KeyCode.Mouse0);
    }

    public IEnumerator Shock(){
        yield return new WaitForSeconds(shockTime);
        isShocking = false;
    }

    private void OnCollisionEnter2D(Collision2D other){
        if(other.collider.gameObject.layer == LayerMask.NameToLayer("Shock")){
            isShocking = true;
            StartCoroutine(Shock());
        }

        if(other.collider.gameObject.layer == LayerMask.NameToLayer("Explosive")){

        }
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Ammo")){
            other.gameObject.GetComponent<Ammo>().Pickup();
            if(currentAmmunition + other.gameObject.GetComponent<Ammo>().ammo > maxAmmunition){
                currentAmmunition = maxAmmunition;
            }
            else{
                currentAmmunition += other.gameObject.GetComponent<Ammo>().ammo;
            }
        }
    }

   
}
