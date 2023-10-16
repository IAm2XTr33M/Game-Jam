using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSection : MonoBehaviour
{
    float tileHeight = 0.9f;

    [Min(4)]
    [SerializeField] Vector2Int size;

    [System.Serializable]
    public class AllGameObjects
    {
        public GameObject dirtObj;
        public GameObject floorObj;
        public GameObject detailObj;
    }

    public AllGameObjects allGameObjects = new AllGameObjects();

    public Material groundMat;

    GameObject startSection;

    List<SectionTile> tiles = new();

    private void Start()
    {
        CreateTiles();
        CreateParent();
        CreatePrimitives();

        foreach (SectionTile tile in tiles)
        {
            SpawnTile(tile, 0);
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
        startSection = new GameObject();
        startSection.transform.position = Vector3.zero;
        startSection.name = "Start";
    }
    void CreatePrimitives()
    {
        foreach (SectionTile tile in tiles)
        {
            GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tileObject.name = "tile";
            tileObject.transform.parent = startSection.transform;
            tileObject.transform.position = gameObject.transform.position + new Vector3(tile.pos.x, -1, tile.pos.y);
            tileObject.transform.localScale = new Vector3(1, 0.98f, 1f);
            tileObject.GetComponent<MeshRenderer>().material = groundMat;
            tile.tile = tileObject;
        }

        foreach (SectionTile tile in tiles)
        {
            GameObject tileObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tileObject.name = "pilar";
            tileObject.transform.parent = startSection.transform;
            tileObject.transform.position = gameObject.transform.position + new Vector3(tile.pos.x, -2, tile.pos.y);
            tileObject.transform.localScale = new Vector3(1, 1.02f, 1f);
            tileObject.GetComponent<MeshRenderer>().material = groundMat;
        }
    }

    void SpawnTile(SectionTile sectionTile, int height)
    {
        int rnd = Random.Range(0, 8);
        GameObject selectedObj = null;
        Vector3 pos = new Vector3(0, tileHeight * height + 0.505f, 0);

        if (rnd < 3)
        {
            selectedObj = allGameObjects.floorObj;
        }
        else if (rnd == 4 || rnd == 5)
        {
            selectedObj = allGameObjects.detailObj;
            pos = rnd == 5 ? new Vector3(0, tileHeight * height + 0.48f, 0) : pos;
        }
        if (selectedObj != null)
        {
            selectedObj = Instantiate(selectedObj);
            selectedObj.transform.parent = sectionTile.tile.transform;
            selectedObj.transform.position = sectionTile.tile.transform.position + pos;
        }
    }
}
