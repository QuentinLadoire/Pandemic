using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupplyRequest
{
    public int ernergyRequest = 0;
    public int firstAidRequest = 0;
    public int foodRequest = 0;
    public int vaccineRequest = 0;
    public int waterRequest = 0;

    public SupplyRequest()
    {

    }
    public SupplyRequest(SupplyRequest copy)
    {
        ernergyRequest = copy.ernergyRequest;
        firstAidRequest = copy.firstAidRequest;
        foodRequest = copy.foodRequest;
        vaccineRequest = copy.vaccineRequest;
        waterRequest = copy.waterRequest;
    }
}

public class TownCard : MonoBehaviour
{
    [SerializeField] Color[] m_supplyTypeColors = new Color[5];
    [SerializeField] GameObject m_supply = null;
    List<GameObject> m_supplys = new List<GameObject>();

    [SerializeField] string m_townName = "TownName";
    [SerializeField] WorldRegion m_worldRegion = WorldRegion.None;
    [SerializeField] SupplyRequest m_supplyRequested = new SupplyRequest();

    public string townName
    {
        get => m_townName;
    }
    public WorldRegion worldRegion
    {
        get => m_worldRegion;
    }
    public SupplyRequest supplyRequested { get => m_supplyRequested; }

    public void RestartPosition()
    {
        transform.position = Vector3.zero;
    }
    public void PlaceInSlot(TownSlot slot)
    {
        slot.townCard = this;

        transform.position = slot.transform.position;
        transform.rotation = slot.transform.rotation;
        transform.position += transform.forward * -3.0f;
    }
    public void PlaceInDeck(CommandCenterRoom commandRoom)
    {
        transform.position = commandRoom.townDeck.transform.position;
        transform.position += new Vector3(0.0f, 0.05f, 0.0f);
        transform.Rotate(Vector3.forward, 180);
    }

    int index = 0;
    GameObject InstantiateSupplyIcon(Vector3 startPosition, Vector3 offset, int i, int countRequest)
    {
        var tmp = Instantiate(m_supply);
        tmp.transform.parent = m_supply.transform.parent;
        tmp.transform.position = startPosition + new Vector3(offset.x * (index % (2 + (countRequest % 2))), 0.0f, offset.z * (index / (2 + (countRequest % 2))));

        index++;

        return tmp;
    }
    void InstantiateSupplyIconAndSetColor(Vector3 startPosition, Vector3 offset, int countRequest, SupplyType supplyType, int supplyCountRequest)
    {
        for (int i = 0; i < supplyCountRequest; i++)
        {
            var tmp = InstantiateSupplyIcon(startPosition, offset, i, countRequest);
            tmp.GetComponent<SpriteRenderer>().color = m_supplyTypeColors[(int)supplyType];
            m_supplys.Add(tmp);
        }
    }

    private void Awake()
    {
        int countRequest = supplyRequested.ernergyRequest + supplyRequested.firstAidRequest + supplyRequested.foodRequest + supplyRequested.vaccineRequest + supplyRequested.waterRequest;

        Vector3 startPosition = Vector3.zero;
        Vector3 offset = new Vector3(1.0f, 0.0f, -1.0f);
        bool pair = (countRequest % 2 == 0);

        if (pair) startPosition = new Vector3(-0.5f, 0.01f, 0.0f);
        else startPosition = new Vector3(-1.0f, 0.01f, 0.0f);

        InstantiateSupplyIconAndSetColor(startPosition, offset, countRequest, SupplyType.Energy, supplyRequested.ernergyRequest);
        InstantiateSupplyIconAndSetColor(startPosition, offset, countRequest, SupplyType.FirstAid, supplyRequested.firstAidRequest);
        InstantiateSupplyIconAndSetColor(startPosition, offset, countRequest, SupplyType.Food, supplyRequested.foodRequest);
        InstantiateSupplyIconAndSetColor(startPosition, offset, countRequest, SupplyType.Vaccine, supplyRequested.vaccineRequest);
        InstantiateSupplyIconAndSetColor(startPosition, offset, countRequest, SupplyType.Water, supplyRequested.waterRequest);

        Destroy(m_supply);
    }
}
