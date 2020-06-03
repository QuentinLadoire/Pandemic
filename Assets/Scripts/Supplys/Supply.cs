using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : MonoBehaviour
{
    public SupplyType type = SupplyType.None;
    SupplyRoom m_startRoom = null;
    SupplyRoom m_currentRoom = null;
    public SupplyRoom currentRoom { get => m_currentRoom; }

    public void ReturnToStartRoom()
    {
        PlaceInRoom(m_startRoom);
    }
    public void PlaceTo(Transform target)
    {
        transform.rotation = target.rotation;
        transform.position = target.position;
        transform.position += new Vector3(0.0f, 0.25f, 0.0f);
    }
    public void PlaceInRoom(SupplyRoom room)
    {
        if (m_startRoom == null) m_startRoom = room;
        if (currentRoom != null)
        {
            currentRoom.assignedSupplys.Remove(this);
        }

        PlaceTo(room.GetEmptySlotPosition());

        m_currentRoom = room;

        m_currentRoom.assignedSupplys.Add(this);
    }
}
