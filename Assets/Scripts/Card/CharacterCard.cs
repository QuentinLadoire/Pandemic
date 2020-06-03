using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] string m_name = "CharacterName";
    [SerializeField] RoomType m_startRoom = RoomType.None;

    public string characterName { get => m_name; }
    public RoomType startRoom { get => m_startRoom; }

    public Player ownerPlayer { get; set; }
    public bool hasPlayer { get => ownerPlayer != null; }
    public bool isVisible { get => gameObject.activeSelf; set => gameObject.SetActive(value); }

    private void Start()
    {
        transform.position = GameStaticRef.cameraController.characterCardTarget.position;

        isVisible = false;
    }
}
