using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{
    [SerializeField] Color m_color = new Color();
    public Color color { get => m_color; }

    Room m_currentRoom = null;
    public Room currentRoom { get => m_currentRoom; }

    public void PlaceInRoom(Room room)
    {
        if (m_currentRoom != null) m_currentRoom.pawnPointIndex--;

        transform.position = room.pawnPoints[room.pawnPointIndex].transform.position;
        transform.position += new Vector3(0.0f, 1.0f, 0.0f);

        m_currentRoom = room;
        m_currentRoom.pawnPointIndex++;
    }
}
