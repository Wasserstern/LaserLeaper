using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringGenerator : MonoBehaviour
{
    public GameObject branchPrefab;
    public string axiom;
    public int iterations;
    public float branchLength;
    public float rotation;
    string currentString;

    float currentRotation;
    Vector2 currentPosition;
    Vector2 savedPosition;
    float savedRotation;

    

    void Start()
    {

        // Generate string
        currentString = axiom;
        for(int i = 0; i < iterations; i++){
            string nextString = "";
            foreach(char c in currentString){
                switch(c){
                    case '1':{
                        nextString += "11";
                        break;
                    }
                    case '0':{
                        nextString += "1[0]0";
                        break;
                    }
                    case '[':{
                        // do nothing
                        break;
                    }
                    case ']':{
                        // do nothing
                        break;
                    }
                }
            }
            currentString = nextString;
        }
        Debug.Log(currentString);

        // Build scene with string
        currentPosition = transform.position;
        foreach(char c in currentString){
            switch(c){
                case '1':{
                        // Generate branch, set position to branch top
                        GameObject newBranch = Instantiate(branchPrefab);
                        newBranch.transform.position = currentPosition;
                        LineRenderer branchLine = newBranch.GetComponent<LineRenderer>();
                        Vector2 upDirection = (new Vector2(currentPosition.x, currentPosition.y +1) - currentPosition).normalized;
                        Vector2 branchDirection = rotate(upDirection, currentRotation);
                        Vector2 branchEnd = currentPosition + branchLength * branchDirection;
                        Vector3[] linePositions = {(Vector3)currentPosition, (Vector3)branchEnd};
                        branchLine.SetPositions(linePositions);
                        savedPosition = currentPosition;
                        currentPosition = branchEnd;
                        
                        break;
                    }
                    case '0':{
                        GameObject newBranch = Instantiate(branchPrefab);
                        newBranch.transform.position = currentPosition;
                        LineRenderer branchLine = newBranch.GetComponent<LineRenderer>();
                        Vector2 upDirection = (new Vector2(currentPosition.x, currentPosition.y +1) - currentPosition).normalized;
                        Vector2 branchDirection = rotate(upDirection, currentRotation);
                        Vector2 branchEnd = currentPosition + branchLength * branchDirection;
                        Vector3[] linePositions = {(Vector3)currentPosition, (Vector3)branchEnd};
                        branchLine.SetPositions(linePositions);
                        break;
                    }
                    case '[':{
                        savedRotation = currentRotation;
                        if(Random.Range(0, 2) == 0){
                            currentRotation += rotation;
                        }
                        else{
                            currentRotation -= rotation;
                        }
                        // do nothing
                        break;
                    }
                    case ']':{
                        currentRotation = 0f;
                        currentPosition = savedPosition;
                        // do nothing
                        break;
                    }
            }
        }
    }

    void Update()
    {
        
    }
    public Vector2 rotate(Vector2 v, float delta) {
    return new Vector2(
        v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
        v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
    );
    }
}
