using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{   public enum LevelMode {cascade, tunnel, stairs, mineField}
    AllManager allmng;
    // Settings for all modes
    public int iterations;
    public Vector3Int currentPosition;
    public int blockHeightPerGeneration;
    public int blockWidth;
    public float shockBlockChance;
    public LevelMode currentLevelMode;
    public List<LevelMode> activeLevelModes;
    public Tilemap groundTiles;
    public Tilemap shockTiles;

    public TileBase groundTile;
    public TileBase shockTile;

    [SerializeField]
    Vector3Int startPosition;
    
    // Settings cascade

    public int cascadeHoleWidth;
    public int cascadeMaxOffset;
    public int cascadeYSpacing;

    // Settings tunnel

    public int tunnelBlockWidth;
    Vector2 lastEndLine;

    // Settings stairs
    public int stairFlightCount;
    public int stairBlockWidth;

    void Start()
    {
        allmng = GameObject.Find("AllManager").GetComponent<AllManager>();
        currentPosition = groundTiles.WorldToCell(transform.position);
        startPosition = currentPosition;
        lastEndLine = default;
        for(int i = 0; i < iterations; i++){
            Generate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.G)){
            Generate();
        }
    }

    void Generate(){
        switch(currentLevelMode){
            case LevelMode.cascade:{
                int heightCounter = 0;
                for(int i = 0; i < blockHeightPerGeneration; i++){
                    if(heightCounter >= cascadeYSpacing){
                        heightCounter = 0;
                        int holeLeft = Random.Range(0, blockWidth);
                        int holeRight;
                        if(holeLeft <= blockWidth / 2){
                            holeRight = holeLeft + cascadeHoleWidth -1;
                        }
                        else{
                            int temp = holeLeft - cascadeHoleWidth -1;
                            holeRight = holeLeft;
                            holeLeft = temp;
                        }
                        for(int j = 0; j < blockWidth; j++){
                            if(j < holeLeft || j > holeRight)
                            {
                                if(Random.Range(0f, 1f) <= shockBlockChance){
                                    /*
                                    GameObject newShockBlock = GameObject.Instantiate(allmng.shockBlockPrefab);
                                    newShockBlock.transform.position = currentPosition;
                                    */
                                    shockTiles.SetTile(currentPosition, groundTile);
                                }
                                else{
                                    /*
                                    GameObject newNormalBlock = GameObject.Instantiate(allmng.normalBlockPrefab);
                                    newNormalBlock.transform.position = currentPosition;
                                    */
                                    groundTiles.SetTile(currentPosition, groundTile);
                                }
                                
                            }
                            else{
                                groundTiles.SetTile(currentPosition, null);
                                shockTiles.SetTile(currentPosition, null);
                            }
                            currentPosition = new Vector3Int(currentPosition.x +1, currentPosition.y);
                        }
                    }
                    else{
                        heightCounter++;
                    }
                    currentPosition = new Vector3Int(startPosition.x, currentPosition.y +1);
                    }
                }
                break;
            case LevelMode.stairs:{
                
                for(int i = 0; i < blockHeightPerGeneration; i++){
                    Vector3Int currentBlockPosition = currentPosition;
                    for(int j = 0; j < blockWidth; j++){
                        Vector3Int nextBlockPosition = new Vector3Int(currentPosition.x + j, currentPosition.y + i);
                        if(Random.Range(0f, 1f) <= shockBlockChance){
                            shockTiles.SetTile(nextBlockPosition, shockTile);
                        }
                        else{
                            groundTiles.SetTile(nextBlockPosition, groundTile);
                        }
                    }
                }
                currentPosition = new Vector3Int(Random.Range(startPosition.x, startPosition.x + blockWidth), currentPosition.y);
                int stairY = currentPosition.y;
                for(int k = 0; k < blockHeightPerGeneration / stairFlightCount; k++){
                    Vector2 startLine;
                    if(lastEndLine != default){
                        startLine = new Vector2(lastEndLine.x, lastEndLine.y);
                    }
                    else{
                        startLine = new Vector2(Random.Range(startPosition.x, startPosition.x + blockWidth), stairY);
                    }

                    Vector2 endLine;
                    if(k % 2 == 0){
                        endLine = new Vector2(startPosition.x, stairY + blockHeightPerGeneration / stairFlightCount);
                    }
                    else{
                        endLine = new Vector2(startPosition.x + blockWidth, stairY + blockHeightPerGeneration / stairFlightCount);
                    }
                    Vector2 lineDirection = (endLine - startLine).normalized;
                    for(int i = 0; i <= Vector2.Distance(endLine, startLine); i++){
                        Vector2 linePoint = startLine + lineDirection * i;
                        for(int j = 0 ; j < stairBlockWidth; j++){
                            Vector3Int nextHolePosition = new Vector3Int((int)linePoint.x - stairBlockWidth / 2 + j, (int)linePoint.y);
                            shockTiles.SetTile(nextHolePosition, null);
                        }
                    }
                    lastEndLine = endLine;
                    stairY += blockHeightPerGeneration / stairFlightCount;

                }
                    currentPosition = new Vector3Int(startPosition.x, stairY);

                
                break;
            }
            case LevelMode.tunnel:{


                // Build set of blocks
                
                for(int i = 0; i < blockHeightPerGeneration; i++){
                    Vector3Int currentBlockPosition = currentPosition;
                    for(int j = 0; j < blockWidth; j++){
                        Vector3Int nextBlockPosition = new Vector3Int(currentPosition.x + j, currentPosition.y + i);
                        if(Random.Range(0f, 1f) <= shockBlockChance){
                            shockTiles.SetTile(nextBlockPosition, shockTile);
                        }
                        else{
                            groundTiles.SetTile(nextBlockPosition, groundTile);
                        }
                    }
                }

                // Draw line through blocks and remove blocks along the line

                // Bezier
                /*
                currentPosition = new Vector3Int(startPosition.x + blockWidth / 2, currentPosition.y);
                Vector3Int bezierEnd = new Vector3Int(currentPosition.x, currentPosition.y + blockHeightPerGeneration);
                Vector3Int cp1 = new Vector3Int(Random.Range(startPosition.x + blockWidth / 4, startPosition.x + blockWidth / 4 * 3), Random.Range(currentPosition.x, currentPosition.y + blockHeightPerGeneration));
                Vector3Int cp2 = new Vector3Int(Random.Range(startPosition.x + blockWidth / 4, startPosition.x + blockWidth) / 4, Random.Range(currentPosition.x, currentPosition.y + blockHeightPerGeneration));
                Func<float, Vector2> bezierFunction = BezierFunction(new Vector2(currentPosition.x, currentPosition.y), new Vector2(bezierEnd.x, bezierEnd.y), new Vector2(cp1.x, cp1.y), new Vector2(cp2.x, cp2.y));
                Debug.Log(currentPosition);
                Debug.Log(bezierEnd);
                for(int i = 0; i < blockHeightPerGeneration; i++){
                    float t = (float)i / (float)blockHeightPerGeneration;
                    Vector2 bezierPoint = bezierFunction(t);
                    Vector3Int nextHolePosition = new Vector3Int((int)bezierPoint.x, (int)bezierPoint.y);
                    Debug.Log(nextHolePosition);

                    groundTiles.SetTile(nextHolePosition, groundTile);
                }
                */

                // Simple line
                currentPosition = new Vector3Int(Random.Range(startPosition.x, startPosition.x + blockWidth), currentPosition.y);
                Vector2 startLine;
                if(lastEndLine != default){
                    startLine = new Vector2(lastEndLine.x, lastEndLine.y + 1);
                }
                else{
                    startLine = new Vector2(Random.Range(startPosition.x, startPosition.x + blockWidth), currentPosition.y);
                }
                Vector2 endLine = new Vector2(Random.Range(startPosition.x, startPosition.x + blockWidth), currentPosition.y + blockHeightPerGeneration);
                Vector2 lineDirection = (endLine - startLine).normalized;
                for(int i = 0; i <= Vector2.Distance(endLine, startLine); i++){
                    Vector2 linePoint = startLine + lineDirection * i;
                    for(int j = 0 ; j < tunnelBlockWidth; j++){
                        Vector3Int nextHolePosition = new Vector3Int((int)linePoint.x - tunnelBlockWidth / 2 + j, (int)linePoint.y);
                        shockTiles.SetTile(nextHolePosition, null);
                    }
                }
                lastEndLine = endLine;

                currentPosition = new Vector3Int(startPosition.x, currentPosition.y + blockHeightPerGeneration +1);
                break;
            }
            case LevelMode.mineField:{
                break;
            }
        }

        // Add wall blocks
        Vector3Int wallStart = new Vector3Int(startPosition.x, currentPosition.y - blockHeightPerGeneration -1);
        for(int i = 0; i < blockHeightPerGeneration; i++){
            groundTiles.SetTile(new Vector3Int(wallStart.x -1, wallStart.y), groundTile);
            groundTiles.SetTile(new Vector3Int(wallStart.x + blockWidth, wallStart.y), groundTile);
            wallStart = new Vector3Int(wallStart.x, wallStart.y + 1);
        }

        // TODO: Comment this out when starting game
        ChangeLevelMode();
    }
    public void ChangeLevelMode(){
        currentLevelMode = activeLevelModes[Random.Range(0, activeLevelModes.Count)];
    }

    private Func<float, Vector2> BezierFunction(Vector2 a, Vector2 b, Vector2 cp1, Vector2 cp2){
        Func<float, Vector2> bezier = (t) => {
            Vector2 bezierResult = Mathf.Pow(1-t, 3) * a + 3 * Mathf.Pow(1-t, 2) * t * cp1 + 3 * (1-t) * MathF.Pow(t, 2) * cp2 + MathF.Pow(t, 3) * b;
            return bezierResult;
        };

        return bezier;
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.layer == LayerMask.NameToLayer("Player")){
            Generate();
        }
    }
}
