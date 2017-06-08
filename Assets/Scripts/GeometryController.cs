using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometryController : MonoBehaviour {

    public Block emptyBlockPrefab;
    public float cubeScale;
    public Vector3 mapSize;
    private Utility.Coord[] allBlockCoords;
    private Block[][][] blocks;

    private int c = 0;

    [Range(0, 1)]
    public float padding;

    private void Start() {

        // Create the Transform for this map's geometry
        Transform mapHolder = new GameObject("Blocks").transform;
        mapHolder.parent = transform;

        allBlockCoords = new Utility.Coord[(int)mapSize.x * (int)mapSize.y * (int)mapSize.z];
        blocks = Utility.CreateJaggedArray<Block[][][]>((int)mapSize.x, (int)mapSize.y, (int)mapSize.z);

        // Populate the parellel arrays
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                for (int k = 0; k < mapSize.z; k++) {
                    // New block's position is scaled from its coordinates
                    Utility.Coord newBlockCoord = new Utility.Coord(i, j, k);
                    Vector3 newBlockPosition = CoordToPosition(newBlockCoord);

                    // Instantiate a new Block object and add it to the map's Transform
                    Block newBlock = Instantiate(emptyBlockPrefab, newBlockPosition, Quaternion.identity) as Block;
                    newBlock.transform.localScale = Vector3.one * (1 - padding) * cubeScale;
                    newBlock.transform.parent = mapHolder;

                    // Add the new Block and its Coord
                    blocks[i][j][k] = newBlock;
                    allBlockCoords[FlattenIndex(i, j, k)] = newBlockCoord;
                }
            }
        }

        // Testing out some logic here
        for (int i = 0; i < 2; i++) {
            InvertRandomRange();
        }

        InvokeRepeating("Methody", 2.0f, 1.0f);
    }

    private void Methody() {
        if (c < 100) { 
            ShiftGrid(Vector3.up);
            c++;
        }
    }



    // TEST METHOD
    private void InvertRandomRange() {
        Utility.Coord corner1 = GetRandomCoord();
        Utility.Coord corner2 = GetRandomCoord();
        foreach (Block b in GetBlocksInRange(corner1, corner2)) {
            b.Invert();
        }
    }

    // TEST METHOD
    private Utility.Coord GetRandomCoord() {
        int randomIndex = FlattenIndex(
            UnityEngine.Random.Range(0, (int)mapSize.x),
            UnityEngine.Random.Range(0, (int)mapSize.y),
            UnityEngine.Random.Range(0, (int)mapSize.z)
            );
        return allBlockCoords[randomIndex];
    }

    public void ShiftGrid(Vector3 dir) {
        Utility.Rotate(blocks, (int)dir.x);
        foreach (Block[][] layer1 in blocks) {
            Utility.Rotate(layer1, (int)dir.y);
            foreach (Block[] layer2 in layer1) {
                Utility.Rotate(layer2, (int)dir.z);
                foreach (Block b in layer2) {
                    b.UpdatePosition();
                }
            }
        }
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                for (int k = 0; k < mapSize.z; k++) {
                    blocks[i][j][k].CheckNeighbors();
                }
            }
        }
    }

    // Returns a list containing references to thisBlock's neighbors
    public Block[] GetNeighbors(Block thisBlock) {
        Utility.Coord thisBlockCoord = GetBlockCoord(thisBlock);
        List<Block> thisBlockNeighbors = new List<Block>();

        // Depending on where the block is located, it may have 3, 4, 5, or 6 neighbors...
        foreach (Vector3 dir in Utility.Coord.GetDirectionalOffsets()) {
            // Apply the offsets to thisBlock's Coord to get a neighbor's indices
            int neighborI = (int)dir.x + thisBlockCoord.i;
            int neighborJ = (int)dir.y + thisBlockCoord.j;
            int neighborK = (int)dir.z + thisBlockCoord.k;

            // Check if the neighbor's index is a valid
            if (IndexInBounds(neighborI, neighborJ, neighborK)) {
                thisBlockNeighbors.Add(blocks[neighborI][neighborJ][neighborK]);
            } else {
                thisBlockNeighbors.Add(null);
            }
        }

        // Return the List of thisBlock's neighbors
        return thisBlockNeighbors.ToArray();
    }

    // Scales and returns a Vector3 based on its 3D indices
    public Vector3 CoordToPosition(Utility.Coord thisCoord) {
        float offsetX = (-mapSize.x / 2) + 0.5f;
        float offsetY = (-mapSize.y / 2) + 0.5f;
        float offsetZ = (-mapSize.z / 2) + 0.5f;

        float scaledX = offsetX + thisCoord.i;
        float scaledY = offsetY + thisCoord.j;
        float scaledZ = offsetZ + thisCoord.k;

        return new Vector3(scaledX, scaledY, scaledZ) * cubeScale;
    }

    private int FindBlockIndex(Block thisBlock) {
        for (int i = 0; i < (int)mapSize.x; i++) {
            for (int j = 0; j < (int)mapSize.y; j++) {
                for (int k = 0; k < (int)mapSize.z; k++) {
                    if (thisBlock.Equals(blocks[i][j][k])) {
                        return FlattenIndex(i, j, k);
                    }
                }
            }
        }
        return -1;
    }

    private int FlattenIndex(int i, int j, int k) {
        if (IndexInBounds(i, j, k)) {
            return FlattenIndex(i, j, k, (int)mapSize.x, (int)mapSize.y, (int)mapSize.z);
        }
        else { return -1; }
    }

    private int FlattenIndex(int i, int j, int k, int xRange, int yRange, int zRange) {
        if (IndexInBounds(i, j, k, xRange, yRange, zRange)) {
            return i + xRange * (j + yRange * k);
        }
        else { return -1; }
    }

    // Returns true if the indices are within the boundaries of the map
    private bool IndexInBounds(int i, int j, int k) {
        return IndexInBounds(i, j, k, (int)mapSize.x, (int)mapSize.y, (int)mapSize.z);
    }

    // Returns true if the indices are within the boundaries of the provided ranges
    private bool IndexInBounds(int i, int j, int k, int xRange, int yRange, int zRange) {
        return (
            i < xRange &&
            j < yRange &&
            k < zRange &&
            i >= 0 &&
            j >= 0 &&
            k >= 0
        );
    }

    // Finds a Block in the grid and returns its Coord
    public Utility.Coord GetBlockCoord(Block thisBlock) {
        for (int i = 0; i < mapSize.x; i++) {
            for (int j = 0; j < mapSize.y; j++) {
                for (int k = 0; k < mapSize.z; k++) {
                    Block b = blocks[i][j][k];
                    if (b.Equals(thisBlock)) {
                        return allBlockCoords[FlattenIndex(i, j, k)];
                    }
                }
            }
        }
        return new Utility.Coord();
    }

    // Determines whether or not a given block is located on the outside "shell" of the structure
    public bool IsEdgeBlock(Block thisBlock) {
        Utility.Coord thisBlockCoord = GetBlockCoord(thisBlock);
        return (
            thisBlockCoord.i == mapSize.x - 1 ||
            thisBlockCoord.j == mapSize.y - 1 ||
            thisBlockCoord.k == mapSize.z - 1 ||
            thisBlockCoord.i == 0 ||
            thisBlockCoord.j == 0 ||
            thisBlockCoord.k == 0 );
    }

    // Returns an array of blocks that can be thought of as a rectangular prism of blocks between 2 vertices
    private Block[] GetBlocksInRange(int i1, int j1, int k1, int i2, int j2, int k2) {
        if (IndexInBounds(i1, j1, k1) && IndexInBounds(i2, j2, k2)) {
            return new Block[0];
        }
        else {
            Utility.Coord corner1 = allBlockCoords[FlattenIndex(i1, j1, k1)];
            Utility.Coord corner2 = allBlockCoords[FlattenIndex(i2, j2, k2)];
            return GetBlocksInRange(corner1, corner2);
        }
    }

    // Returns an array of blocks that can be thought of as a rectangular prism of blocks between 2 vertices
    private Block[] GetBlocksInRange(Utility.Coord corner1, Utility.Coord corner2) {

        int xDiff = Math.Abs(corner1.i - corner2.i) + 1;
        int yDiff = Math.Abs(corner1.j - corner2.j) + 1;
        int zDiff = Math.Abs(corner1.k - corner2.k) + 1;
        Block[] selectedRange = new Block[xDiff * yDiff * zDiff];
        int lesserI = Math.Min(corner1.i, corner2.i);
        int lesserJ = Math.Min(corner1.j, corner2.j);
        int lesserK = Math.Min(corner1.k, corner2.k);

        int index = 0;
        for (int i = 0; i < xDiff; i++) {
            for (int j = 0; j < yDiff; j++) {
                for (int k = 0; k < zDiff; k++) {
                    Block current = blocks[(lesserI + i)][(lesserJ + j)][(lesserK + k)];
                    selectedRange[index++] = current;
                }
            }
        }
        return selectedRange;
    }

    // Returns the range of blocks that are alive (the rectangular prism of blocks that contains the alive blocks)
    private Block[] GetLiveBlockRange() {

        // Initialize these variables with obviously invalid & inverted values
        int smallestI = (int)mapSize.x;
        int smallestJ = (int)mapSize.y;
        int smallestK = (int)mapSize.z;
        int biggestI = -1;
        int biggestJ = -1;
        int biggestK = -1;

        // Loop over all the blocks to find the smallest and largest indices of the alive blocks' coordinates
        for (int i = 0; i < (int)mapSize.x; i++) {
            for (int j = 0; j < (int)mapSize.y; j++) {
                for(int k = 0; k < mapSize.z; k++) {
                    Block thisBlock = blocks[i][j][k];
                    if ( thisBlock.IsAlive() ) {
                        Utility.Coord thisBlockCoord = allBlockCoords[FlattenIndex(i, j, k)];
                        smallestI = Math.Min(thisBlockCoord.i, smallestI);
                        smallestJ = Math.Min(thisBlockCoord.j, smallestJ);
                        smallestK = Math.Min(thisBlockCoord.k, smallestK);
                        biggestI = Math.Max(thisBlockCoord.i, biggestI);
                        biggestJ = Math.Max(thisBlockCoord.j, biggestJ);
                        biggestK = Math.Max(thisBlockCoord.k, biggestK);
                    }
                }
            }
        }

        // Once the boundaries of the alive blocks have been identified, return the range of blocks contained therein
        return GetBlocksInRange(smallestI, smallestJ, smallestK, biggestI, biggestJ, biggestK);
    }
}
