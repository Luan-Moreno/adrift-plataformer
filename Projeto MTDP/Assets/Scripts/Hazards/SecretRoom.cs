using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class SecretRoom : MonoBehaviour
{
    private Tilemap tilemap;
    private BoundsInt area;
    private BoxCollider2D boxCollider2D;
    [SerializeField] private LayerMask layersToHide;
    private List<GameObject> hiddenObjects = new List<GameObject>();

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        
        Vector3Int position = Vector3Int.FloorToInt(boxCollider2D.bounds.min);
        Vector3Int size = Vector3Int.FloorToInt(boxCollider2D.bounds.size + new Vector3Int(0, 0, 1));
        area = new BoundsInt(position, size);

        foreach (Vector3Int point in area.allPositionsWithin)
        {
            tilemap.SetTileFlags(point, TileFlags.None);
            tilemap.SetColor(point, new Color(1f, 1f, 1f, 0f));
        }

        Collider2D[] colliders = Physics2D.OverlapBoxAll
        (
            boxCollider2D.bounds.center,
            boxCollider2D.bounds.size,
            0f
        );

        foreach (Collider2D c in colliders)
        {
            if (IsInLayerMask(c.gameObject, layersToHide))
            {
                c.gameObject.SetActive(false);
                hiddenObjects.Add(c.gameObject);
            }
        }
    }

    public void RevealRoom()
    {
        foreach (Vector3Int point in area.allPositionsWithin)
        {
            tilemap.SetColor(point, new Color(1f, 1f, 1f, 1f));
        }

        foreach (GameObject obj in hiddenObjects)
        {
            obj.SetActive(true);
        }
    }
    
    bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
}
