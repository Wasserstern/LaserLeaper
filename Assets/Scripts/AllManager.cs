using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllManager : MonoBehaviour
{
    public GameObject normalBlockPrefab;
    public GameObject shockBlockPrefab;
    public bool camIsScrolling;
    public float camScrollSpeed;

    Camera mainCamera;

    void Awake(){
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(camIsScrolling){
            mainCamera.gameObject.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + camScrollSpeed * Time.deltaTime, mainCamera.transform.position.z);
        }
    }
}
