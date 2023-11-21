using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool isDestructable;
    public float startHealth;
    
    [SerializeField]
    float currentHealth;
    SpriteRenderer renderer;
    
    public List<Sprite> allSprites;
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        currentHealth = startHealth;
    }

    void Update()
    {
        
    }
    
    public void Damage(float damage){
        currentHealth -= damage;
        if(currentHealth <= 0){
            Destroy(this.gameObject);
        }
        else{
            Debug.Log(Mathf.FloorToInt(allSprites.Count * (currentHealth / startHealth)));
            renderer.sprite = allSprites[Mathf.FloorToInt(allSprites.Count * (currentHealth / startHealth))];
        }
    }
}
