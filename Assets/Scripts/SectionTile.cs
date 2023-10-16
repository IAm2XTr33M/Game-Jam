using UnityEngine;

[System.Serializable]
public class SectionTile 
{
    public Vector2Int pos;

    public GameObject tile;
    public GameObject Layer1;
    public GameObject Layer2;
    public GameObject Layer3;

    public SectionTile(int x, int z)
    {
        this.pos = new Vector2Int(x,z);
    }

}
