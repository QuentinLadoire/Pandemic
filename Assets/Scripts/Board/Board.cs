using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Child Reference :")]
    [Header("Rooms Reference")]
    [SerializeField] CommandCenterRoom m_commandCenterRoom = new CommandCenterRoom();
    [SerializeField] SupplyRoom m_waterRoom = null;
    [SerializeField] SupplyRoom m_firstAidRoom = null;
    [SerializeField] SupplyRoom m_vaccineRoom = null;
    [SerializeField] SupplyRoom m_energyRoom = null;
    [SerializeField] SupplyRoom m_foodRoom = null;
    [SerializeField] RecyclingRoom m_recyclingRoom = null;
    [SerializeField] SupplyRoom m_cargoBayRoom = null;

    [Header("Towns Reference")]
    [SerializeField] TownSlot[] m_blackTowns = new TownSlot[6];
    [SerializeField] TownSlot[] m_blueTowns = new TownSlot[6];
    [SerializeField] TownSlot[] m_redTowns = new TownSlot[6];
    [SerializeField] TownSlot[] m_yellowTowns = new TownSlot[6];

    Room[] m_rooms = new Room[7];
    TownSlot[] m_townSlots = new TownSlot[24];

    void SetupSupplyRoom(SupplyRoom supplyRoom, GameObject[] supplys)
    {
        for (int i = 0; i < supplyRoom.supplyBar.Length; i++)
        {
            var supply = supplys[i].GetComponent<Supply>();

            //set the supply to the assigned room and set the assigned room to the supply and move the supply to the room
            supply.PlaceInRoom(supplyRoom);
        }
    }

    void SetupAllSupplyRoom(SupplysReference supplysReference)
    {
        SetupSupplyRoom(m_waterRoom, supplysReference.waterSupplys);
        SetupSupplyRoom(m_firstAidRoom, supplysReference.firstAidSupplys);
        SetupSupplyRoom(m_vaccineRoom, supplysReference.vaccineSupplys);
        SetupSupplyRoom(m_energyRoom, supplysReference.energySupplys);
        SetupSupplyRoom(m_foodRoom, supplysReference.foodSupplys);
    }
    void SetupRecyclingRoom(TokensReference tokensReference)
    {
        tokensReference.wasteToken.transform.position = m_recyclingRoom.recyclingPoints[0].transform.position;
        tokensReference.wasteToken.transform.position += new Vector3(0.0f, 0.05f, 0.0f);
    }

    void SetupPlayers(Player[] players)
    {
        foreach (var player in players) player.pawn.PlaceInRoom(GetRoom(player.characterCard.startRoom));
    }
    void SetupCards(List<TownCard> drawingCard, List<TownCard> townCardDeck)
    {
        foreach (var card in drawingCard) card.PlaceInSlot(GetTownSlot(card.townName));
        foreach (var card in townCardDeck) card.PlaceInDeck(m_commandCenterRoom);
    }

    public void SetupBoard(SupplysReference supplysReference, TokensReference tokensReference, Player[] players, List<TownCard> drawingCard, List<TownCard> townCardDeck)
    {
        SetupAllSupplyRoom(supplysReference);
        SetupRecyclingRoom(tokensReference);

        SetupPlayers(players);
        SetupCards(drawingCard, townCardDeck);
    }

    public Room GetRoom(RoomType roomType)
    {
        switch (roomType)
        {
            case RoomType.CargoBayRoom: return m_cargoBayRoom;
            case RoomType.CommandCenter: return null;
            case RoomType.EnergyRoom: return m_energyRoom;
            case RoomType.FirstAidRoom: return m_firstAidRoom;
            case RoomType.FoodRoom: return m_foodRoom;
            case RoomType.RecyclingRoom: return m_recyclingRoom;
            case RoomType.VaccineRoom: return m_vaccineRoom;
            case RoomType.WaterRoom: return m_waterRoom;
        }

        return null;
    }
    public TownSlot GetTownSlot(string townName)
    {
        foreach (var town in m_townSlots) if (town.townName == townName) return town;

        return null;
    }

    public CommandCenterRoom GetCommandCenter()
    {
        return m_commandCenterRoom;
    }
    public Room GetHitRoom(Ray ray)
    {
        foreach (var room in m_rooms)
        {
            RaycastHit hit;
            if (room.hitBox.Raycast(ray, out hit, 100.0f)) return room;
        }

        return null;
    }
    public TownSlot GetHitTownSlot(Ray ray)
    {
        foreach (var slot in m_townSlots)
        {
            RaycastHit hit;
            if (slot.hitBox.Raycast(ray, out hit, 100.0f)) return slot;
        }

        return null;
    }

    private void Awake()
    {
        m_rooms[0] = m_waterRoom;
        m_rooms[1] = m_firstAidRoom;
        m_rooms[2] = m_vaccineRoom;
        m_rooms[3] = m_energyRoom;
        m_rooms[4] = m_foodRoom;
        m_rooms[5] = m_recyclingRoom;
        m_rooms[6] = m_cargoBayRoom;

        for (int i = 0; i < m_townSlots.Length; i++)
        {
            if (i < 6) m_townSlots[i] = m_blackTowns[i];
            else if (i < 12) m_townSlots[i] = m_blueTowns[i % 6];
            else if (i < 18) m_townSlots[i] = m_yellowTowns[i % 6];
            else m_townSlots[i] = m_redTowns[i % 6];

            m_townSlots[i].index = i;
        }
    }
}
