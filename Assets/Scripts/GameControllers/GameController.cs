using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate void OnRollDiceResult(Dice[] dices);
public delegate void OnTurn(Player currentPlayer);
public delegate void OnState();
public delegate void OnUpdateTimer(float percent);

public class GameController : MonoBehaviour
{
    #region PrefabReferences
    [Header("References")]
    [SerializeField] DicesPrefabReference m_dicesPrefabReference = new DicesPrefabReference();
    [SerializeField] PawnsPrefabReference m_pawnPrefabReference = new PawnsPrefabReference();
    [SerializeField] SupplysPrefabReference m_supplysPrefabReference = new SupplysPrefabReference();
    [SerializeField] TokensPrefabReference m_tokensPrefabReference = new TokensPrefabReference();
    [SerializeField] CardsPrefabReference m_cardsPrefabReference = new CardsPrefabReference();
	#endregion

	#region GameInstanciatedReference
	//GameReferences
	DicesReference m_dicesReference = new DicesReference();
    PawnsReference m_pawnsReference = new PawnsReference();
    SupplysReference m_supplysReference = new SupplysReference();
    TokensReference m_tokensReference = new TokensReference();
    CardsReference m_cardsReference = new CardsReference();
	#endregion

	#region GameParameters
	//GameParameters
	public int nbPlayer = 2;
    public Difficulty difficulty = Difficulty.Easy;
    float m_timerDuration = 120.0f;

    public bool IsMyPlayerTurn(Player player) { return player == m_players[m_playerIndex]; }
    public bool canRollDice
    {
        get => !m_endGame && !m_movePawnState && !m_movePlaneState;
    }
    public bool canMovePawn
    {
        get => !m_endGame && !m_movePlaneState && !m_waitFirstRoll;
    }
    public bool canMovePlane
    {
        get => !m_endGame && !m_movePawnState && !m_waitFirstRoll;
    }
    public bool canAssignDice
    {
        get => !m_endGame && !m_movePawnState && !m_movePlaneState && !m_waitFirstRoll;
    }
    public bool canActivateRoom
    {
        get => !m_endGame && !m_movePawnState && !m_movePlaneState && !m_waitFirstRoll;
    }
	#endregion

	#region GameReference
	Board m_board = null;
    CameraController m_cameraController = null;
    CanvasController m_canvasController = null;

    //CurrentGameReference
    List<TownCard> m_deck = new List<TownCard>();
    List<TownCard> m_townCardDeck = new List<TownCard>();
    List<TownCard> m_placedTownCards = new List<TownCard>();

    List<Tokens> m_tokenDeck = new List<Tokens>();
    List<Tokens> m_currentTokenDeck = new List<Tokens>();

    Player[] m_players = null;
    public Player currentPlayer { get => m_players[m_playerIndex]; }

    PlanePawn m_planePawn = null;

    Coroutine m_movePawnStateCoro = null;
    Coroutine m_movePlaneStateCoro = null;

    Coroutine m_updateTimerCoro = null;
	#endregion

	#region GameTimer
	bool m_endGame = true;
    bool m_isPlaying = false;
    bool m_timeOut = false;
    float m_currentTime = 0.0f;

    public OnUpdateTimer onUpdateTimer = (float percent) => { };
    public OnState onEndGame = () => { };
	#endregion

	#region GameTurnInfo
	//GameTurn Info
	int m_playerIndex = 1;
    int m_rerollCout = 3;

    bool m_movePawnState = false;
    bool m_movePlaneState = false;
    bool m_waitFirstRoll = false;
    public bool waitFirstRoll { get => m_waitFirstRoll; }
    #endregion

    #region DelegateGameTurn
    public OnRollDiceResult onRollDiceResult = (Dice[] dices) => { };
    public OnTurn onStartNewTurn = (Player currentPlayer) => { };
    public OnTurn onEndTurn = (Player currentPlayer) => { };

    public OnState onMovePawnStateEnter = () => { };
    public OnState onMovePawnStateExit = () => { };

    public OnState onMovePlaneStateEnter = () => { };
    public OnState onMovePlaneStateExit = () => { };
	#endregion

	#region InstantiationGameObjectFunction
	GameObject[] InstantiateDices(GameObject[] dices, GameObject dicePrefab, Transform parent)
    {
        if (dicePrefab == null) { return null; }
        var folder = new GameObject();
        folder.name = dicePrefab.name;
        folder.transform.parent = parent;

        for (int i = 0; i < dices.Length; i++) dices[i] = Instantiate(dicePrefab, folder.transform);

        return dices;
    }
    void InstantiatePawns(Transform parent)
    {
        if (m_pawnPrefabReference.bluePawn != null) m_pawnsReference.pawns[0] = Instantiate(m_pawnPrefabReference.bluePawn, parent);
        if (m_pawnPrefabReference.brownPawn != null) m_pawnsReference.pawns[1] = Instantiate(m_pawnPrefabReference.brownPawn, parent);
        if (m_pawnPrefabReference.greenPawn != null) m_pawnsReference.pawns[2] = Instantiate(m_pawnPrefabReference.greenPawn, parent);
        if (m_pawnPrefabReference.greyPawn != null) m_pawnsReference.pawns[3] = Instantiate(m_pawnPrefabReference.greyPawn, parent);
        if (m_pawnPrefabReference.planePawn != null) m_pawnsReference.planePawn = Instantiate(m_pawnPrefabReference.planePawn, parent);
    }
    void InstantiateSupplys(GameObject[] supplys, GameObject supplyPrefab, Transform parent)
    {
        if (supplyPrefab == null) { return; }
        var folder = new GameObject();
        folder.name = supplyPrefab.name;
        folder.transform.parent = parent;

        for (int i = 0; i < supplys.Length; i++) supplys[i] = Instantiate(supplyPrefab, folder.transform);
    }
    void InstantiateTokens(Transform parent)
    {
        var folder = new GameObject();
        folder.name = "TimeTokens";
        folder.transform.parent = parent;

        if (m_tokensPrefabReference.timeToken != null) for (int i = 0; i < m_tokensReference.timeToken.Length; i++) m_tokensReference.timeToken[i] = Instantiate(m_tokensPrefabReference.timeToken, folder.transform);
        //else Debug.LogWarning("TimeTokenPrefab is null");

        if (m_tokensPrefabReference.wasteToken != null) m_tokensReference.wasteToken = Instantiate(m_tokensPrefabReference.wasteToken, parent);
    }
    void InstantiateCharacterCards(GameObject[] characterCards, GameObject[] characterCardsPrefab, Transform parent)
    {
        var folder = new GameObject();
        folder.name = "CharacterCards";
        folder.transform.parent = parent;

        for (int i = 0; i < characterCards.Length; i++) characterCards[i] = Instantiate(characterCardsPrefab[i], folder.transform);
    }
    void InstantiateTownCards(GameObject[] townCards, TownRegion townRegion, TownRegion townRegionPrefab, Transform parent)
    {
        var folder = new GameObject();
        folder.name = "TownCards";
        folder.transform.parent = parent;

        var folder2 = new GameObject();
        folder2.name = "BlackTown";
        folder2.transform.parent = folder.transform;

        for (int i = 0; i < townRegion.blackTownCards.Length; i++)
        {
            townRegion.blackTownCards[i] = Instantiate(townRegionPrefab.blackTownCards[i], folder2.transform);
            townCards[i] = townRegion.blackTownCards[i];
        }

        folder2 = new GameObject();
        folder2.name = "BlueTown";
        folder2.transform.parent = folder.transform;

        for (int i = 0; i < townRegion.blueTownCards.Length; i++)
        {
            townRegion.blueTownCards[i] = Instantiate(townRegionPrefab.blueTownCards[i], folder2.transform);
            townCards[6 + i] = townRegion.blueTownCards[i];
        }

        folder2 = new GameObject();
        folder2.name = "RedTown";
        folder2.transform.parent = folder.transform;

        for (int i = 0; i < townRegion.redTownCards.Length; i++)
        {
            townRegion.redTownCards[i] = Instantiate(townRegionPrefab.redTownCards[i], folder2.transform);
            townCards[12 + i] = townRegion.redTownCards[i];
        }

        folder2 = new GameObject();
        folder2.name = "YellowTown";
        folder2.transform.parent = folder.transform;

        for (int i = 0; i < townRegion.yellowTownCards.Length; i++)
        {
            townRegion.yellowTownCards[i] = Instantiate(townRegionPrefab.yellowTownCards[i], folder2.transform);
            townCards[18 + i] = townRegion.yellowTownCards[i];
        }
    }

    void InstantiateGameObject()
    {
        //Instantiate All Dices
        var folder = new GameObject();
        folder.name = "Dices";
        folder.transform.parent = transform.parent;

        m_dicesReference.dices[0] = InstantiateDices(m_dicesReference.blueDice, m_dicesPrefabReference.blueDice, folder.transform);
        m_dicesReference.dices[1] = InstantiateDices(m_dicesReference.brownDice, m_dicesPrefabReference.brownDice, folder.transform);
        m_dicesReference.dices[2] = InstantiateDices(m_dicesReference.greenDice, m_dicesPrefabReference.greenDice, folder.transform);
        m_dicesReference.dices[3] = InstantiateDices(m_dicesReference.greyDice, m_dicesPrefabReference.greyDice, folder.transform);

        //Instantiate All Pawns
        folder = new GameObject();
        folder.name = "Pawns";
        folder.transform.parent = transform.parent;

        InstantiatePawns(folder.transform);

        //Instantiate All Supplys
        folder = new GameObject();
        folder.name = "Supplys";
        folder.transform.parent = transform.parent;

        InstantiateSupplys(m_supplysReference.energySupplys, m_supplysPrefabReference.energySupply, folder.transform);
        InstantiateSupplys(m_supplysReference.firstAidSupplys, m_supplysPrefabReference.firstAidSupply, folder.transform);
        InstantiateSupplys(m_supplysReference.foodSupplys, m_supplysPrefabReference.foodSupply, folder.transform);
        InstantiateSupplys(m_supplysReference.vaccineSupplys, m_supplysPrefabReference.vaccineSupply, folder.transform);
        InstantiateSupplys(m_supplysReference.waterSupplys, m_supplysPrefabReference.waterSupply, folder.transform);

        //Instantiate All Tokens
        folder = new GameObject();
        folder.name = "Tokens";
        folder.transform.parent = transform.parent;

        InstantiateTokens(folder.transform);

        //Instantiate All CharacterCards
        folder = new GameObject();
        folder.name = "Cards";
        folder.transform.parent = transform.parent;

        InstantiateCharacterCards(m_cardsReference.characterCards, m_cardsPrefabReference.characterCards, folder.transform);

        //Instantiate All TownCards
        InstantiateTownCards(m_cardsReference.townCards, m_cardsReference.townRegion, m_cardsPrefabReference.townRegion, folder.transform);
    }
	#endregion

	#region CreateAGameFunction
    TownCard DrawARandomCard()
    {
        int rand = Random.Range(0, m_deck.Count);

        var randomTown = m_deck[rand];
        m_deck.RemoveAt(rand);

        return randomTown;
    }
    void SetupGame()
    {
        int randomSeed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(randomSeed);

        m_endGame = false;
        m_currentTime = m_timerDuration;

        foreach (var townCard in m_cardsReference.townCards) m_deck.Add(townCard.GetComponent<TownCard>());
    }
    void AssignTimeTokens(GameObject[] timeTokens)
    {
        for (int i = 0; i < timeTokens.Length; i++)
        {
            if (i < 3)
            {
                m_currentTokenDeck.Add(timeTokens[i].GetComponent<Tokens>());
                m_currentTokenDeck[i].PlaceAt(m_board.GetCommandCenter().timeTokenPoints[i].transform);
            }
            else m_tokenDeck.Add(timeTokens[i].GetComponent<Tokens>());
        }
    }
	void CreatePlayer()
    {
        var folder = new GameObject();
        folder.name = "Players";
        folder.transform.parent = transform.parent;

        m_players = new Player[nbPlayer];
        for (int i = 0; i < nbPlayer; i++)
        {
            var playerGo = new GameObject();
            playerGo.name = "Player_" + i;
            playerGo.transform.parent = folder.transform;

            m_players[i] = playerGo.AddComponent<Player>();
            m_players[i].playerName = "Player_" + i;
        }
    }
    void GiveCharacterCard(GameObject[] characterCards)
    {
        List<GameObject> tmpCharacterCard = new List<GameObject>();
        foreach (var player in m_players)
        {
            //Give a random not already used character card
            int rand = 0;
            do rand = Random.Range(0, characterCards.Length);
            while (tmpCharacterCard.Contains(characterCards[rand]));

            tmpCharacterCard.Add(characterCards[rand]);

            player.characterCard = characterCards[rand].GetComponent<CharacterCard>();
            player.characterCard.ownerPlayer = player;
        }
    }
    void GivePawnAndDices(GameObject[] pawns, GameObject[][] dices)
    {
        //Give pawn and dices with the identical color
        for (int i = 0; i < nbPlayer; i++)
        {
            m_players[i].pawn = pawns[i].GetComponent<Pawn>();

            for (int j = 0; j < dices[i].Length; j++)
            {
                m_players[i].dices[j] = dices[i][j].GetComponent<Dice>();
                m_players[i].dices[j].ownerPlayer = m_players[i];
                m_players[i].dices[j].Init(j);
            }
        }
    }
    void SetAndPlacePlanePawn()
    {
        m_planePawn = m_pawnsReference.planePawn.GetComponent<PlanePawn>();

        var randomCard = DrawARandomCard();
        m_planePawn.PlaceInTownSlot(m_board.GetTownSlot(randomCard.townName));
    }
    void DrawTownCard(GameObject[] townCards)
    {
        //Select the placed Cities Cards in city deck in fuction of difficulty level
        int nbDrawCard = 0;
        int nbStackCard = 0;

        switch (difficulty)
        {
            case Difficulty.Easy:
                nbDrawCard = 2;
                nbStackCard = 3;
                break;

            case Difficulty.Medium:
                nbDrawCard = 2;
                nbStackCard = 5;
                break;

            case Difficulty.Hard:
                nbDrawCard = 3;
                nbStackCard = 7;
                break;

            case Difficulty.Heroic:
                nbDrawCard = 4;
                nbStackCard = 9;
                break;
        }

        for (int i = 0; i < nbDrawCard; i++)
        {
            //Give a random not already used town Card
            var randomCard = DrawARandomCard();

            m_placedTownCards.Add(randomCard);
        }
        for (int i = 0; i < nbStackCard; i++)
        {
            //Give a random not already used town Card
            var randomCard = DrawARandomCard();
            
            m_townCardDeck.Add(randomCard);
        }
    }

    public void CreateGame()
    {
        SetupGame();

        CreatePlayer();
        GiveCharacterCard(m_cardsReference.characterCards);
        GivePawnAndDices(m_pawnsReference.pawns, m_dicesReference.dices);

        SetAndPlacePlanePawn();
        DrawTownCard(m_cardsReference.townCards);

        AssignTimeTokens(m_tokensReference.timeToken);

        m_board.SetupBoard(m_supplysReference, m_tokensReference, m_players, m_placedTownCards, m_townCardDeck);
    }
	#endregion

	#region GameplayFunction
    public void StartGame()
    {
        m_isPlaying = true;
        m_endGame = false;
        m_timeOut = false;

        m_updateTimerCoro = StartCoroutine(UpdateTimer());

        StartNewTurn();
    }
    void StartNewTurn()
    {
        //next player
        m_playerIndex++;
        m_playerIndex %= nbPlayer;

        m_rerollCout = 3;

        m_waitFirstRoll = true;

        onStartNewTurn(currentPlayer);
    }
    void ReStartTimer()
    {
        m_currentTime = m_timerDuration;
        m_updateTimerCoro = StartCoroutine(UpdateTimer());
    }
    void RollDice(Player player)
    {
        var dices = player.dices;
        for (int i = 0; i < dices.Length; i++) dices[i].Rool();

        onRollDiceResult(dices);
    }
    void MovePawn(Player player, Room room)
    {
        player.MovePawn(room);
    }
    void MovePlane(Player player, TownSlot slot)
    {
        if (m_planePawn.currentTownSlot.IsNeighbour(slot) && player.nbDiceToMovePlane != 0)
        {
            m_planePawn.PlaceInTownSlot(slot);
            player.UseDiceForMovePlane();
        }
    }
    void AssignDice(Player player)
    {
        player.AssignDices();
    }
    void AddTokenTime()
    {
        var token = m_tokenDeck[m_tokenDeck.Count - 1];
        m_tokenDeck.RemoveAt(m_tokenDeck.Count - 1);

        m_currentTokenDeck.Add(token);
        token.PlaceAt(m_board.GetCommandCenter().timeTokenPoints[m_currentTokenDeck.Count - 1].transform);
    }
    void RemoveTokenTime()
    {
        var token = m_currentTokenDeck[m_currentTokenDeck.Count - 1];
        m_currentTokenDeck[m_currentTokenDeck.Count - 1].ResetPosition();
        m_currentTokenDeck.RemoveAt(m_currentTokenDeck.Count - 1);

        m_tokenDeck.Add(token);
    }
    void PickTownCard()
    {
        if (m_townCardDeck.Count == 0) return;

        var townCard = m_townCardDeck[m_townCardDeck.Count - 1];
        m_townCardDeck.RemoveAt(m_townCardDeck.Count - 1);

        m_placedTownCards.Add(townCard);
        townCard.PlaceInSlot(m_board.GetTownSlot(townCard.townName));
    }
    void ValidateCard(TownCard card)
    {
        //remove the card
        m_placedTownCards.Remove(card);
        m_planePawn.currentTownSlot.townCard = null;
        card.RestartPosition();

        //add a token time
        AddTokenTime();
    }
    void ActivateRoom(Player player)
    {
        var room = player.GetRoom();
        if (room is SupplyRoom)
        {
            var supplyRoom = room as SupplyRoom;
            var cargoBayRoom = m_board.GetRoom(RoomType.CargoBayRoom) as SupplyRoom;

            //if ressource room
            if (room.roomType != RoomType.CargoBayRoom)
            {
                var productionCount = player.ActivateRoom();

                supplyRoom.MoveSupplysTo(productionCount, cargoBayRoom);
            }
            else //if Cargobay
            {
                var townCard = m_planePawn.currentTownSlot.townCard;
                if (townCard == null)
                {
                    //Debug.Log("No town to deliver");
                    return;
                }
                var supplyRequested = new SupplyRequest(townCard.supplyRequested);

                int energySupplyCount = cargoBayRoom.GetCountOf(SupplyType.Energy);
                int firstAidSupplyCount = cargoBayRoom.GetCountOf(SupplyType.FirstAid);
                int foodSupplyCount = cargoBayRoom.GetCountOf(SupplyType.Food);
                int vaccineSupplyCount = cargoBayRoom.GetCountOf(SupplyType.Vaccine);
                int waterSupplyCount = cargoBayRoom.GetCountOf(SupplyType.Water);

                if (energySupplyCount >= supplyRequested.ernergyRequest &&
                    firstAidSupplyCount >= supplyRequested.firstAidRequest &&
                    foodSupplyCount >= supplyRequested.foodRequest &&
                    vaccineSupplyCount >= supplyRequested.vaccineRequest &&
                    waterSupplyCount >= supplyRequested.waterRequest)
                {
                    player.ActivateRoom();

                    //Return the count of requested supply to their start room and validate the townCard
                    var tmp = new List<Supply>(cargoBayRoom.assignedSupplys);
                    foreach (var supply in tmp)
                    {
                        if (supply.type == SupplyType.Energy && supplyRequested.ernergyRequest > 0)
                        {
                            supply.ReturnToStartRoom();
                            supplyRequested.ernergyRequest--;
                        }
                        else if (supply.type == SupplyType.FirstAid && supplyRequested.firstAidRequest > 0)
                        {
                            supply.ReturnToStartRoom();
                            supplyRequested.firstAidRequest--;
                        }
                        else if (supply.type == SupplyType.Food && supplyRequested.foodRequest > 0)
                        {
                            supply.ReturnToStartRoom();
                            supplyRequested.foodRequest--;
                        }
                        else if (supply.type == SupplyType.Vaccine && supplyRequested.vaccineRequest > 0)
                        {
                            supply.ReturnToStartRoom();
                            supplyRequested.vaccineRequest--;
                        }
                        else if (supply.type == SupplyType.Water && supplyRequested.waterRequest > 0)
                        {
                            supply.ReturnToStartRoom();
                            supplyRequested.waterRequest--;
                        }
                    }

                    cargoBayRoom.RePositionSupplys();

                    ValidateCard(townCard);
                }
                else
                {
                    //Debug.Log("NotEnough Ressource in the CargoBay for deliver the town.");
                }
            }
        }
    }
    void EndGame()
    {
        m_endGame = true;

        StopCoroutine(m_updateTimerCoro);

        onEndGame();
    }
    #endregion

    #region CoroutineState
    IEnumerator OnMovePawnStateStay()
    {
        m_movePawnState = true;
        onMovePawnStateEnter();

        while (currentPlayer.nbDiceToMovePawn != 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var room = m_board.GetHitRoom(m_cameraController.MousePointToRay());
                MovePawn(currentPlayer, room);
            }

            yield return null;
        }

        m_movePawnState = false;

        onMovePawnStateExit();
    }
    IEnumerator OnMovePlaneStateStay()
    {
        m_movePlaneState = true;
        onMovePlaneStateEnter();

        while (currentPlayer.nbDiceToMovePlane != 0)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var townSlot = m_board.GetHitTownSlot(m_cameraController.MousePointToRay());
                if (townSlot != null) MovePlane(currentPlayer, townSlot);
            }

            yield return null;
        }

        m_movePlaneState = false;

        onMovePlaneStateExit();
    }
    IEnumerator UpdateTimer()
    {
        m_timeOut = false;

        while (m_currentTime > 0.0f)
        {
            m_currentTime -= Time.deltaTime;

            onUpdateTimer(m_currentTime / m_timerDuration);

            yield return null;
        }

        m_timeOut = true;
    }
    #endregion

    #region DelegateUIFunction
    void OnShowHideCharacterCardButtonClick()
    {
        currentPlayer.characterCard.isVisible = !currentPlayer.characterCard.isVisible;
    }
    void OnRollDiceButtonClick()
    {
        if (!canRollDice) return;
        if (m_rerollCout == 0) return;
        if (currentPlayer.nbDiceToRoll == 0) return;

        RollDice(currentPlayer);

        if (m_waitFirstRoll) m_waitFirstRoll = false;
        m_rerollCout--;
    }
    void OnMovePawnButtonClick()
    {
        if (!canMovePawn) return;
        if (currentPlayer.nbDiceToMovePawn == 0) return;

        if (!m_movePawnState)
        {
            m_movePawnStateCoro = StartCoroutine(OnMovePawnStateStay());
        }
        else
        {
            StopCoroutine(m_movePawnStateCoro);
            m_movePawnState = false;

            onMovePawnStateExit();
        }
    }
    void OnMovePlaneButtonClick()
    {
        if (!canMovePlane) return;
        if (currentPlayer.nbDiceToMovePlane == 0) return;

        if (!m_movePlaneState)
        {
            m_movePlaneStateCoro = StartCoroutine(OnMovePlaneStateStay());
        }
        else
        {
            StopCoroutine(m_movePlaneStateCoro);
            m_movePlaneState = false;

            onMovePlaneStateExit();
        }
    }
    void OnAssignDiceButtonClick()
    {
        if (!canAssignDice) return;
        if (currentPlayer.nbDiceAssignable == 0) return;

        AssignDice(currentPlayer);
    }
    void OnActivateRoomButtonClick()
    {
        if (!canActivateRoom) return;
        if (!currentPlayer.canActivateRoom) return;

        ActivateRoom(currentPlayer);
    }
    void OnSkipTurnButtonClick()
    {
        if (m_endGame) return;
        if (m_waitFirstRoll) return;

        //break the coroutine if a skyp the turn
        if (m_movePawnState)
        {
            StopCoroutine(m_movePawnStateCoro);
            m_movePawnState = false;
            onMovePawnStateExit();
        }
        if (m_movePlaneState)
        {
            StopCoroutine(m_movePlaneStateCoro);
            m_movePlaneState = false;
            onMovePlaneStateExit();
        }

        onEndTurn(currentPlayer);

        StartNewTurn();
    }
	#endregion

	#region UnityCallBack
	private void Awake()
    {
        GameStaticRef.gameController = this;

        InstantiateGameObject();

        m_board = GameObject.FindObjectOfType<Board>();
        m_cameraController = GameObject.FindObjectOfType<CameraController>();
        m_canvasController = GameObject.FindObjectOfType<CanvasController>();
    }

    private void Start()
    {
        m_canvasController.gamePanel.onShowHideCharacterCardButtonClick += OnShowHideCharacterCardButtonClick;
        m_canvasController.gamePanel.onRollDiceButtonClick += OnRollDiceButtonClick;
        m_canvasController.gamePanel.onMovePawnButtonClick += OnMovePawnButtonClick;
        m_canvasController.gamePanel.onMovePlaneButtonClick += OnMovePlaneButtonClick;
        m_canvasController.gamePanel.onAssignButtonClick += OnAssignDiceButtonClick;
        m_canvasController.gamePanel.onActivateButtonClick += OnActivateRoomButtonClick;
        m_canvasController.gamePanel.onSkipTurnButtonClick += OnSkipTurnButtonClick;
    }

    private void Update()
    {
        if (m_timeOut && m_currentTokenDeck.Count > 0)
        {
            RemoveTokenTime();
            PickTownCard();
            ReStartTimer();
        }
        else if ((m_timeOut && m_currentTokenDeck.Count == 0) || (m_isPlaying && m_townCardDeck.Count == 0 && m_placedTownCards.Count == 0))
        {
            EndGame();
        }
    }

    private void OnDestroy()
    {
        m_canvasController.gamePanel.onShowHideCharacterCardButtonClick -= OnShowHideCharacterCardButtonClick;
        m_canvasController.gamePanel.onRollDiceButtonClick -= OnRollDiceButtonClick;
        m_canvasController.gamePanel.onMovePawnButtonClick -= OnMovePawnButtonClick;
        m_canvasController.gamePanel.onMovePlaneButtonClick -= OnMovePlaneButtonClick;
        m_canvasController.gamePanel.onAssignButtonClick -= OnAssignDiceButtonClick;
        m_canvasController.gamePanel.onActivateButtonClick -= OnActivateRoomButtonClick;
        m_canvasController.gamePanel.onSkipTurnButtonClick -= OnSkipTurnButtonClick;
    }
	#endregion
}
