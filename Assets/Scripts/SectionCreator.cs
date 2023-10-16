using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[ExecuteInEditMode]
public class SectionCreator : MonoBehaviour
{
    float tileHeight = 0.9f;
    float wallHeight;

    [Min(4)]
    [SerializeField] Vector2Int size;

    public enum CreateMode{Empty,Ruin,Castle,Construction,Wall,Random};
    public CreateMode createMode;

    [SerializeField] int DirtAmmount;

    [SerializeField] bool Clear = false;
    [SerializeField] bool Create = false;


    

    [System.Serializable]
    public class AllGameObjects
    {
        public GameObject dirtObj;
        public GameObject floorObj;
        public GameObject detailObj;
        public GameObject wallObj;
        public GameObject wallHalfObj;
        public GameObject wallNarrowObj;
        public GameObject wallOpenObj;
        public GameObject stairsObj;
        public GameObject woodPlatformObj;
        public GameObject columnObj;
    }

    public AllGameObjects allGameObjects = new AllGameObjects();

    public Material groundMat;

    GameObject section;

    List<SectionTile> tiles = new();


    void Update()
    {
        if (Clear)
        {
            Clear = false;
            DestroyImmediate(section);
            tiles.Clear();
        }

        if (Create && section == null)
        {
            Create = false;

            CreateTiles();
            CreateParent();
            CreatePrimitives();

            CreateSection();
        }
        else if (Create)
        {
            Create = false;
        }
    }

    void CreateTiles()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                tiles.Add(new SectionTile(x, y));
            }
        }
    }
    void CreateParent()
    {
        section = new GameObject();
        section.transform.position = Vector3.zero;
        section.name = "Section";
    }
    void CreatePrimitives()
    {
        foreach(SectionTile tile in tiles)
        {
            GameObject tileObject =  GameObject.CreatePrimitive(PrimitiveType.Cube);
            tileObject.name = "tile";
            tileObject.transform.parent = section.transform;
            tileObject.transform.position = gameObject.transform.position + new Vector3(tile.pos.x,-1,tile.pos.y);
            tileObject.transform.localScale = new Vector3(1, 0.98f, 1f);
            tileObject.GetComponent<MeshRenderer>().material = groundMat;
            tile.tile = tileObject;
        }

        foreach(SectionTile tile in tiles)
        {
            GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tileObject.name = "pilar";
            tileObject.transform.parent = section.transform;
            tileObject.transform.position = gameObject.transform.position + new Vector3(tile.pos.x, -2, tile.pos.y);
            tileObject.transform.localScale = new Vector3(1, 1.02f, 1f);
            tileObject.GetComponent<MeshRenderer>().material = groundMat;
        }
    }

    void CreateSection()
    {
        if(createMode == CreateMode.Empty)
        {
            foreach (SectionTile tile in tiles)
            {
                SpawnTile(tile, 0);
            }
        }
        else if(createMode == CreateMode.Ruin)
        {
            CreateWeird();
        }
        else if(createMode == CreateMode.Castle)
        {
            CreateCastle();
        }
        else if(createMode == CreateMode.Construction)
        {
            CreateCastle();
            CreateConstruction();
        }
        else if(createMode == CreateMode.Wall)
        {
            CreateWall();
        }
    }


    void CreateWeird()
    {
        //Get random dirt positions
        List<int> randomIndexes = new();
        for (int i = 0; i < Mathf.Clamp(DirtAmmount, 0, size.x * size.y / 2); i++)
        {
            int randomNum = Random.Range(0, size.x * size.y + 1);

            if (!randomIndexes.Contains(randomNum))
            {
                randomIndexes.Add(randomNum);
            }
            else
            {
                i--;
            }
        }

        List<SectionTile> dirtTiles = new();

        //Get all dirt tiles
        foreach (SectionTile tile in tiles)
        {
            if (randomIndexes.Contains(tiles.IndexOf(tile)))
            {
                dirtTiles.Add(tile);
                dirtTiles.AddRange(GetNeighbours(tile, true));
            }
        }

        //Remove duplicates
        dirtTiles = dirtTiles.Distinct().ToList();

        //Spawn all dirt
        foreach (SectionTile tile in dirtTiles)
        {
            GameObject tempDirt = Instantiate(allGameObjects.dirtObj);
            tempDirt.transform.position = tile.tile.transform.position + new Vector3(0, 0.5f, 0);
            tempDirt.transform.parent = tile.tile.transform;
            tile.Layer1 = tempDirt;
            SpawnTile(tile, 1);
        }

        //Get all closest dirts

        List<SectionTile> closestTiles = new List<SectionTile>();

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                SectionTile currentTile = GetTileOfPos(x, y);
                if (currentTile.Layer1 != null)
                {
                    closestTiles.Add(currentTile);
                    y = size.y;
                }
            }
        }

        //replace closest dirt with walls
        foreach (SectionTile tile in closestTiles)
        {
            Transform parent = tile.Layer1.transform.parent;
            Vector3 pos = tile.Layer1.transform.position;

            DestroyImmediate(tile.Layer1);
            tile.Layer1 = Instantiate(allGameObjects.wallObj);
            tile.Layer1.transform.position = pos;
            tile.Layer1.transform.parent = parent;
        }

        //Check if all are in the first row

        bool allInFirstRow = true;
        for (int i = 0; i < closestTiles.Count; i++)
        {
            if (closestTiles[i].pos.y != 0)
            {
                allInFirstRow = false;
            }
        }
        if (allInFirstRow)
        {
            int rnd = Random.Range(0, size.x);
            
            Transform parent = closestTiles[rnd].Layer1.transform.parent;
            Vector3 pos = closestTiles[rnd].Layer1.transform.position;

            while (closestTiles[rnd].tile.transform.childCount > 0)
            {
                DestroyImmediate(closestTiles[rnd].tile.transform.GetChild(0).gameObject);
            }

            GameObject stair = Instantiate(allGameObjects.stairsObj);
            
            stair.transform.parent = parent;
            stair.transform.position = pos;
            stair.transform.eulerAngles = new Vector3(0, 180, 0);

            SectionTile behindStair = GetTileOfPos(closestTiles[rnd].pos.x, 1);
            if (behindStair.Layer1 == null)
            {
                GameObject tempDirt = Instantiate(allGameObjects.dirtObj);
                tempDirt.transform.position = behindStair.tile.transform.position + new Vector3(0, 0.5f, 0);
                tempDirt.transform.parent = behindStair.tile.transform;
                behindStair.Layer1 = tempDirt;
                SpawnTile(behindStair, 1);
            }

            closestTiles[rnd].Layer1 = stair;
        }

        foreach(SectionTile tile in tiles)
        {
            int rnd = Random.Range(0, 10);
            if(rnd == 1)
            {
                if (tile.Layer1)
                {
                    Transform parent = tile.Layer1.transform.parent;
                    Vector3 pos = tile.Layer1.transform.position;

                    while (tile.tile.transform.childCount > 0)
                    {
                        DestroyImmediate(tile.tile.transform.GetChild(0).gameObject);
                    }

                    GameObject stair = Instantiate(allGameObjects.stairsObj);

                    stair.transform.parent = parent;
                    stair.transform.position = pos;

                    stair.transform.eulerAngles = new Vector3(0, 180, 0);
                    if(tile.pos.y > 1)
                    {
                        if (HasObjectInfront(tile, 1))
                        {
                            while (GetTileOfPos(tile.pos.x, tile.pos.y - 1).tile.transform.childCount > 0)
                            {
                                DestroyImmediate(GetTileOfPos(tile.pos.x, tile.pos.y - 1).tile.transform.GetChild(0).gameObject);
                            }
                        }
                    }
                    else if(tile.pos.y < size.y)
                    {
                        if (!HasObjectBehind(tile, 1))
                        {
                            SectionTile behind = GetTileOfPos(tile.pos.x,tile.pos.y+1);

                            GameObject tempDirt = Instantiate(allGameObjects.dirtObj);
                            tempDirt.transform.position = behind.tile.transform.position + new Vector3(0, 0.5f, 0);
                            tempDirt.transform.parent = behind.tile.transform;
                            behind.Layer1 = tempDirt;
                            SpawnTile(behind, 1);
                        }
                    }
                    tile.Layer1 = stair;
                }
            }
        }
        foreach (SectionTile tile in tiles)
        {
            SpawnTile(tile, 0);
        }
    }

    void CreateCastle()
    {
        Vector2Int randomSize = new Vector2Int(Random.Range(1,size.x),Random.Range(3,size.y));
        Debug.Log(randomSize);

        int currentY = 1;
        //Draw Front
        for (int i = 0; i < randomSize.x; i++)
        {
            SectionTile tile = GetTileOfPos(i, currentY);
            GameObject temp;
            if(i != randomSize.x-1)
            {
                if (Random.Range(0, 100) < 50)
                {
                    temp = Instantiate(allGameObjects.stairsObj);
                }
                else
                {
                    temp = Instantiate(allGameObjects.wallHalfObj);
                    SpawnTile(tile, 1);
                }
                temp.transform.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                temp = Instantiate(allGameObjects.wallObj);
            }

            temp.transform.parent = tile.tile.transform;
            temp.transform.position = tile.tile.transform.position + new Vector3(0, 0.5f, 0);
            tile.Layer1 = temp;
        }

        //Draw Center
        currentY++;
        for (int i = 0; i < randomSize.y-2; i++)
        {
            for (int x = 0; x < randomSize.x; x++)
            {
                SectionTile tile = GetTileOfPos(x, currentY);
                GameObject temp;
                if (x != randomSize.x - 1)
                {
                    temp = Instantiate(allGameObjects.dirtObj);
                }
                else
                {
                    temp = Instantiate(allGameObjects.wallHalfObj);
                    temp.transform.eulerAngles = new Vector3(0, 90, 0);
                    SpawnTile(tile, 1);
                }
                temp.transform.parent = tile.tile.transform;
                temp.transform.position = tile.tile.transform.position + new Vector3(0, 0.5f, 0);
                tile.Layer1 = temp;
                
                if(randomSize.x == 2)
                {   
                    if(x == 0)
                    {
                        SpawnObject(tile, allGameObjects.wallNarrowObj, 2 , 90);
                    }
                }
                else if(randomSize.x >= 3)
                {
                    if (x == 0)
                    {
                        SpawnObject(tile, allGameObjects.wallNarrowObj, 2, 90);
                        SpawnObject(tile, allGameObjects.wallNarrowObj, 3, 90);
                    }
                    if (x == 1)
                    {
                        SpawnObject(tile, allGameObjects.wallObj, 2, 90);
                    }
                }
            }
            currentY++;
        }
        //Draw Back
        for (int i = 0; i < randomSize.x; i++)
        {
            SectionTile tile = GetTileOfPos(i, currentY);
            GameObject temp;
            if (i != randomSize.x - 1)
            {
                temp = Instantiate(allGameObjects.wallHalfObj);
                SpawnTile(tile, 1);
            }
            else
            {
                temp = Instantiate(allGameObjects.wallObj);
            }

            temp.transform.parent = tile.tile.transform;
            temp.transform.position = tile.tile.transform.position + new Vector3(0, 0.5f, 0);
            tile.Layer1 = temp;
        }

        int randomInt = Random.Range(1,randomSize.y);
        bool hasOpenWall = false;

        for (int i = 0; i < size.x; i++)
        {
            SectionTile tile = GetTileOfPos(i, randomInt);
            if(tile.Layer1 == null)
            {
                if (hasOpenWall)
                {
                    if(Random.Range(0,100) > 50)
                    {
                        SpawnObject(tile, allGameObjects.wallOpenObj, 1, 0);
                    }
                    else
                    {
                        SpawnObject(tile, allGameObjects.wallNarrowObj, 1, 0);
                    }
                }
                else
                {
                    SpawnObject(tile, allGameObjects.wallOpenObj, 1, 0);
                    hasOpenWall = true;
                }

                SpawnTile(tile, 1);
            }
        }

        foreach (SectionTile tile in tiles)
        {
            SpawnTile(tile, 0);
        }
    }

    void CreateConstruction()
    {
        foreach (SectionTile tile in tiles)
        {
            if(tile.Layer1 != null)
            {
                if(Random.Range(0,100) < 40)
                {
                    if(tile.Layer2 == null)
                    {
                        DestroyImmediate(tile.Layer1);
                        SpawnObject(tile, allGameObjects.woodPlatformObj, 1, 0);
                    }
                }
            }
            if (tile.Layer2 != null)
            {
                if (Random.Range(0, 100) < 70)
                {
                    DestroyImmediate(tile.Layer2);
                    SpawnObject(tile, allGameObjects.woodPlatformObj, 2, 0);
                }
            }
            if (tile.Layer3 != null)
            {
                if (Random.Range(0, 100) < 90)
                {
                    DestroyImmediate(tile.Layer3);
                    SpawnObject(tile, allGameObjects.woodPlatformObj, 3, 0);
                }
            }
        }
    }

    void CreateWall()
    {
        for (int i = 0; i < size.x; i++)
        {
            SectionTile tile = GetTileOfPos(i, size.y-1);
            for (int b = 0; b < 3; b++)
            {
                if(b == 2)
                {
                    if(Random.Range(0,100) < 50)
                    {
                        SpawnObject(tile, allGameObjects.wallOpenObj, b + 1, 0);
                    }
                    else
                    {
                        SpawnObject(tile, allGameObjects.wallObj, b + 1, 0);
                    }
                }
                else
                {
                    if(i != 0 && i != size.x-1)
                    {
                        SpawnObject(tile, allGameObjects.dirtObj, b + 1, 0);
                    }
                    else
                    {
                        SpawnObject(tile, allGameObjects.wallObj, b + 1, 0);
                    }
                }
            }
        }

        //Get random positions
        Vector2Int tempPos = GetRandomPos(size.x - 2, size.y - 3) + new Vector2Int(1, 1);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.stairsObj, 1, 180);

        SectionTile Tile = GetTileOfV2Pos(tempPos + new Vector2Int(0, 1));
        SpawnObject(Tile, allGameObjects.dirtObj, 1, 0);
        SpawnTile(Tile, 1);

        bool left = tempPos.x < size.x / 2;
        tempPos += left ? new Vector2Int(2, 1) : new Vector2Int(-2, 1);

        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.dirtObj, 1, 0);
        SpawnTile(GetTileOfV2Pos(tempPos), 1);

        tempPos -= new Vector2Int(0, 2);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.dirtObj, 1, 0);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.stairsObj, 2, 1);

        tempPos += left ? new Vector2Int(-1, 0) : new Vector2Int(1, 0);
        if (Random.Range(0, 100) < 30)
        {
            SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.dirtObj, 1, 0);
            SpawnTile(GetTileOfV2Pos(tempPos), 1);
        }
        else
        {
            SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.woodPlatformObj, 1, 0);
        }
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.woodPlatformObj, 2, 0);

        tempPos += left ? new Vector2Int(-2, 0) : new Vector2Int(2, 0);
        if(Random.Range(0,100) < 30)
        {
            SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.dirtObj, 1, 0);
            SpawnTile(GetTileOfV2Pos(tempPos), 1);
        }
        else
        {
            SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.woodPlatformObj, 1, 0);
        }
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.woodPlatformObj, 2, 0);

        tempPos += new Vector2Int(0, 2);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.dirtObj, 1, 0);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.woodPlatformObj, 2, 0);
        SpawnObject(GetTileOfV2Pos(tempPos), allGameObjects.stairsObj, 3, 180);
    }


    Vector2Int GetRandomPos(int x, int y)
    {
        return new Vector2Int(Random.Range(0, x), Random.Range(0, y));
    }

    /// <summary>
    /// Spawns an object on a tile height + adds rotation
    /// </summary>
    /// <param name="tile">The tile to spawn it on</param>
    /// <param name="obj">The object to spawn</param>
    /// <param name="height">The height to spawn on</param>
    /// <param name="rot">Y rotation for spawn</param>
    void SpawnObject(SectionTile tile , GameObject obj , int height , float rot)
    {
        GameObject temp = Instantiate(obj);
        temp.transform.position = tile.tile.transform.position + new Vector3(0, 0.5f + tileHeight * (height - 1), 0);
        temp.transform.parent = tile.tile.transform;
        temp.transform.eulerAngles = new Vector3(0, rot, 0);
        if(height == 0) { tile.tile = temp; }
        if(height == 1) { tile.Layer1 = temp;}
        if (height == 2) { temp.transform.localScale = new Vector3(0.998f, 0.998f, 0.998f); tile.Layer2 = temp; }
        if(height == 3) { temp.transform.localScale = new Vector3(0.997f, 0.997f, 0.997f); tile.Layer3 = temp; }
    }

    void SpawnTile(SectionTile sectionTile , int height)
    {
        int rnd = Random.Range(0, 8);
        GameObject selectedObj = null;
        Vector3 pos = new Vector3(0, tileHeight*height + 0.505f, 0);

        if (rnd < 3)
        {
            selectedObj = allGameObjects.floorObj;
        }
        else if (rnd == 4 || rnd == 5)
        {
            selectedObj = allGameObjects.detailObj;
            pos = rnd == 5 ? new Vector3(0, tileHeight * height +0.48f, 0) : pos;
        }
        if (selectedObj != null)
        {
            selectedObj = Instantiate(selectedObj);
            selectedObj.transform.parent = sectionTile.tile.transform;
            selectedObj.transform.position = sectionTile.tile.transform.position + pos;
        }
    }

    List<SectionTile> GetNeighbours(SectionTile centerTile,bool randomise)
    {
        List<SectionTile> tempList = new List<SectionTile>();

        foreach (SectionTile tile in tiles)
        {
            if(centerTile.pos.x + 1 == tile.pos.x && centerTile.pos.y == tile.pos.y
            || centerTile.pos.x - 1 == tile.pos.x && centerTile.pos.y == tile.pos.y
            || centerTile.pos.y - 1 == tile.pos.y && centerTile.pos.x == tile.pos.x
            || centerTile.pos.y + 1 == tile.pos.y && centerTile.pos.x == tile.pos.x)
            {
                if (randomise)
                {
                    int rnd = Random.Range(0, 4);
                    if(rnd != 0)
                    {
                        tempList.Add(tile);
                    }
                }
                else
                {
                    tempList.Add(tile);
                }
            }
        }

        return tempList;
    }

    SectionTile GetTileOfPos(float x, float y)
    {
        if (x < size.x && y < size.y)
        {
            SectionTile currentTile = null;
            foreach (SectionTile tile in tiles)
            {
                if (tile.pos == new Vector2(x, y))
                {
                    currentTile = tile;
                }
            }
            return currentTile;
        }
        else return null;
    }

    SectionTile GetTileOfV2Pos(Vector2Int pos)
    {
        if (pos.x < size.x && pos.y < size.y)
        {
            SectionTile currentTile = null;
            foreach (SectionTile tile in tiles)
            {
                if (tile.pos == new Vector2(pos.x, pos.y))
                {
                    currentTile = tile;
                }
            }
            return currentTile;
        }
        else return null;
    }

    bool HasObjectInfront(SectionTile tile, int layer)
    {
        SectionTile temp = GetTileOfPos(tile.pos.x, tile.pos.y - 1);
        switch (layer)
        {
            case 1: return temp.Layer1 != null ? true : false ;
            case 2: return temp.Layer2 != null ? true : false;
            case 3: return temp.Layer3 != null ? true : false;
        }
        return true;
    }
    bool HasObjectBehind(SectionTile tile , int layer)
    {
        SectionTile temp = GetTileOfPos(tile.pos.x, tile.pos.y+1);
        switch (layer)
        {
            case 1: return temp.Layer1 != null ? true : false;
            case 2: return temp.Layer2 != null ? true : false;
            case 3: return temp.Layer3 != null ? true : false;
        }
        return true;
    }
}
