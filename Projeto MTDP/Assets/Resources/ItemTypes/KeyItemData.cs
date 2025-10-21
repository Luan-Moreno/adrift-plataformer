using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Items/Key Item")]
public class KeyItemData : ItemData
{
    public string doorGUID;
    private Door rightDoor;

    public override bool CanUse(GameObject player)
    {
        return Door.GetUsableDoor(doorGUID) != null;
    }

    public override void OnUse(GameObject player)
    {
        rightDoor = Door.GetUsableDoor(doorGUID);
        rightDoor.OpenDoor();
    }
}
