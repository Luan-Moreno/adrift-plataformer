using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string uniqueId;
    [SerializeField] private bool isOpen = false;
    private GameObject playerNearby;
    private Animator anim;
    public static List<Door> nearbyDoors = new();

    public string UniqueId { get => uniqueId; set => uniqueId = value; }

    [ContextMenu("Generate New GUID")]
    private void GenerateNewGUID()
    {
        UniqueId = System.Guid.NewGuid().ToString();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool CanUseKey(string doorGUID)
    {
        return !isOpen && doorGUID == UniqueId && playerNearby != null;
    }

    public void OpenDoor()
    {
        isOpen = true;
        gameObject.SetActive(false);
        //Debug.Log($"Porta {UniqueId} aberta!");
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerNearby = collider.gameObject;
            nearbyDoors.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            playerNearby = null;
            nearbyDoors.Remove(this);
        }
    }
    
    public static Door GetUsableDoor(string doorGUID)
    {
        foreach (Door door in nearbyDoors)
        {
            if (!door.isOpen && door.UniqueId == doorGUID)
            {
                return door;
            }
        }    
        return null;
    }

    public void SaveData(ref GameData data)
    {
        if (data.doors.ContainsKey(UniqueId))
        {
            data.doors.Remove(UniqueId);
        }
        data.doors.Add(UniqueId, isOpen);
    }

    public void LoadData(GameData data)
    {
        data.doors.TryGetValue(UniqueId, out isOpen);
        if (isOpen)
        {
            //Debug.Log($"Estado da porta {UniqueId} salvo!");
            gameObject.SetActive(false);
        }
    }
}
