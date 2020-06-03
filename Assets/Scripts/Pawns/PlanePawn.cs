using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePawn : MonoBehaviour
{
    TownSlot m_currentTownSlot = null;
    public TownSlot currentTownSlot { get => m_currentTownSlot; }

    public void PlaceInTownSlot(TownSlot slot)
    {
        transform.position = slot.planePoint.transform.position;
        transform.position += new Vector3(0.0f, 1.0f, 0.0f);

        m_currentTownSlot = slot;
    }
}
