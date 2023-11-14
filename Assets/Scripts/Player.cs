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
    public float shotsPerSecond;
    public float shotForce;
    public float bulletSize;
    public float bulletDamage;
    public float bulletSpeed;




    [SerializeField]
    bool holdingShoot;
    
    [SerializeField]
    float mouseX;
    [SerializeField]
    float mouseY;
    
    [SerializeField]
    float timeSinceLastShot;
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
        currentDirectorPosition = ((Vector2)mainCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        if(mouseX != 0 && mouseY != 0){
            shootDirector.localPosition = currentDirectorPosition;
        }

        if(holdingShoot){
            Vector2 shootDirection = ((Vector2)shootDirector.position - (Vector2)transform.position).normalized;
            if(timeSinceLastShot >= 1f / shotsPerSecond){
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
            }
        }
        timeSinceLastShot += Time.deltaTime;
    }

    void GetInput(){
        holdingShoot = Input.GetKey(KeyCode.Mouse0);
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

   
}
