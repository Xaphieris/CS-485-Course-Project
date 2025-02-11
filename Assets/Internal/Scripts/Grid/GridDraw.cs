using UnityEngine;
public class GridDraw : MonoBehaviour
{
    //Set Game Object to spawn
    public GameObject block;

    //Set number of tiles wide
    public float width;

    //Set number of tiles high
    public float height;

    //Offsets for hexagons
    private float xOffset = .5f;

    //Height offset ((Tilesize)/2sin(60))*(3/2);
    private float zOffset = .866f;
  
    void Start()
    {
        for (float z = 0; z < height; ++z)
        {
            for (float x = 0; x < width; ++x)
            {
                GameObject clone;
                if(z % 2 == 0)//Even
                {
                    clone = Instantiate(block, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset, Quaternion.identity);
                }
                else
                {
                    clone = Instantiate(block, new Vector3(x,0,0) + new Vector3 (0, 0, z) * zOffset + new Vector3(1, 0, 0) * xOffset, Quaternion.identity);
                }
                clone.name = "Tile " + x + ", " + z;
            }
        }       
    }
}
