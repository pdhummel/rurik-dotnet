using System;
using System.Collections.Generic;
using System.Linq;

namespace rurik
{
    public class RurikGame
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public int CurrentRound { get; set; }
        public int TargetNumberOfPlayers { get; set; }
        public string Password { get; set; }
        public GameMap GameMap { get; set; }
        public AuctionBoard AuctionBoard { get; set; }
        public Cards Cards { get; set; }
        public ClaimBoard ClaimBoard { get; set; }
        public GamePlayers Players { get; set; }
        public AvailableLeaders AvailableLeaders { get; set; }
        public GameStates GameStates { get; set; }
        public Ai Ai { get; set; }
        public GameLog Log { get; set; }
        public string DeedCardToVerify { get; set; }

        public RurikGame(string gameName, string owner, int targetNumberOfPlayers = 4, string password = "")
        {
            Log = new GameLog(this);
            Owner = owner;
            CurrentRound = 1;
            AuctionBoard = null;
            GameMap = new GameMap();
            Ai = new Ai();
            Cards = new Cards();
            ClaimBoard = new ClaimBoard();
            TargetNumberOfPlayers = targetNumberOfPlayers;
            Players = new GamePlayers(targetNumberOfPlayers);
            AvailableLeaders = new AvailableLeaders();
            GameStates = new GameStates();
            // TODO: generate id with uuid
            Id = DateTime.Now.Ticks.ToString();
            // TODO: formatted creation date
            Name = gameName;
            Password = password;
            DeedCardToVerify = null;
        }

        public void AiEvaluateGame()
        {
            // TODO: Implement AI evaluation
        }

        public void ChangePlayerAndOrState(string player, string gameStateName)
        {
            // TODO: Implement change player and/or state
        }

        public void JoinGame(string name, string color, int position, bool isPlayerAi = false, string password = null)
        {
            // TODO: Implement join game
        }

        public void StartGame()
        {
            // TODO: Implement start game
        }

        public void SelectFirstPlayer(string color)
        {
            // TODO: Implement select first player
        }

        public void SelectRandomFirstPlayer()
        {
            // TODO: Implement select random first player
        }

        public void ChooseLeader(string color, string leaderName)
        {
            // TODO: Implement choose leader
        }

        public void SelectSecretAgenda(string color, string cardName)
        {
            // TODO: Implement select secret agenda
        }

        public void PlaceInitialTroop(string color, string locationName)
        {
            // TODO: Implement place initial troop
        }

        public void PlaceLeader(string color, string locationName)
        {
            // TODO: Implement place leader
        }

        public void PlayAdvisor(string color, string columnName, string advisor, int bidCoins = 0)
        {
            // TODO: Implement play advisor
        }

        public void BeginPlayerAction()
        {
            // TODO: Implement begin player action
        }

        public void UndoPlayerAction()
        {
            // TODO: Implement undo player action
        }

        public void TakeMainAction(string color, string advisor, string actionColumnName, int row, bool forfeitAction = false)
        {
            // TODO: Implement take main action
        }

        public void SchemeFirstPlayer(string currentPlayerColor, string firstPlayerColor)
        {
            // TODO: Implement scheme first player
        }

        public void DrawSchemeCards(string color, string schemeDeck)
        {
            // TODO: Implement draw scheme cards
        }

        public void SelectSchemeCardToKeep(string color, string schemeCard)
        {
            // TODO: Implement select scheme card to keep
        }

        public void SelectSchemeCardToReturn(string color, int schemeDeckNumber, string schemeCardId)
        {
            // TODO: Implement select scheme card to return
        }

        public void Muster(string color, string locationName, int numberOfTroops)
        {
            // TODO: Implement muster
        }

        public void Move(string color, string fromLocationName, string toLocationName, int numberOfTroops = 1, bool moveLeader = false)
        {
            // TODO: Implement move
        }

        public void Tax(string color, string locationName, bool toBoat = true, bool marketCoinNotResource = false)
        {
            // TODO: Implement tax
        }

        public void Build(string color, string locationName, string buildingName, string targetToConvert = null)
        {
            // TODO: Implement build
        }

        public void Attack(string color, string locationName, string target, int schemeDeckNumber = 1)
        {
            // TODO: Implement attack
        }

        public void GoUpWarTrack(string currentPlayer)
        {
            // TODO: Implement go up war track
        }

        public void HandleWarTrackReward(string currentPlayer, string reward)
        {
            // TODO: Implement handle war track reward
        }

        public void TransferGood(string color, string direction, string resource)
        {
            // TODO: Implement transfer good
        }

        public void DrawOneTimeSchemeCard(string color, string schemeDeck)
        {
            // TODO: Implement draw one time scheme card
        }

        public void PlayOneTimeSchemeCard(string color)
        {
            // TODO: Implement play one time scheme card
        }

        public void PlaySchemeCard(string color, string schemeCardId, string schemeCardActionChoice)
        {
            // TODO: Implement play scheme card
        }

        public void CollectSchemeCardReward(string currentPlayer, string schemeCard, string schemeCardActionChoice = null)
        {
            // TODO: Implement collect scheme card reward
        }

        public void TakeDeedCard(string color, string deedCardName)
        {
            // TODO: Implement take deed card
        }

        public void AccomplishedDeed(string color, string deedCardName, List<Dictionary<string, string>> claimStatements)
        {
            // TODO: Implement accomplished deed
        }

        public void VerifyDeed(string color, bool verified)
        {
            // TODO: Implement verify deed
        }

        public void RedeemDeed(Player player, string deedCard)
        {
            // TODO: Implement redeem deed
        }

        public void AccomplishAndRedeemDeed(Player player, string deedCard)
        {
            // TODO: Implement accomplish and redeem deed
        }

        public void EndRound()
        {
            // TODO: Implement end round
        }

        public void EndGame()
        {
            // TODO: Implement end game
        }

        public Dictionary<string, object> CalculateEndGameStats()
        {
            // TODO: Implement calculate end game stats
            return new Dictionary<string, object>();
        }

        public List<string> GetFirstPlacePlayers(Dictionary<string, object> endGameStats)
        {
            // TODO: Implement get first place players
            return new List<string>();
        }

        public void PlayMusterConversionTile(string currentPlayer, string resource1, string resource2)
        {
            // TODO: Implement play muster conversion tile
        }

        public void PlayBuildConversionTile(string currentPlayer, string resource1, string resource2)
        {
            // TODO: Implement play build conversion tile
        }

        public void PlayAttackConversionTile(string currentPlayer)
        {
            // TODO: Implement play attack conversion tile
        }

        public void PlayConversionTile(string color, string conversionTileName, string resource1, string resource2)
        {
            // TODO: Implement play conversion tile
        }

        public void EndCurrentAction(string color)
        {
            // TODO: Implement end current action
        }

        public void EndTurn(string color)
        {
            // TODO: Implement end turn
        }

        public void UpdateClaimsForClaimsPhase()
        {
            // TODO: Implement update claims for claims phase
        }

        public void EvaluateSecretAgendas()
        {
            // TODO: Implement evaluate secret agendas
        }

        public bool EvaluateSecretAgendaProsperous(string playerColor)
        {
            // TODO: Implement evaluate secret agenda prosperous
            return false;
        }

        public bool EvaluateSecretAgendaEsteemed(string playerColor)
        {
            // TODO: Implement evaluate secret agenda esteemed
            return false;
        }

        public bool EvaluateSecretAgendaCapable(string playerColor)
        {
            // TODO: Implement evaluate secret agenda capable
            return false;
        }

        public bool EvaluateSecretAgendaConquering(string playerColor)
        {
            // TODO: Implement evaluate secret agenda conquering
            return false;
        }

        public bool EvaluateSecretAgendaProtective(string playerColor)
        {
            // TODO: Implement evaluate secret agenda protective
            return false;
        }

        public bool EvaluateSecretAgendaSuccessful(string playerColor)
        {
            // TODO: Implement evaluate secret agenda successful
            return false;
        }

        public bool EvaluateSecretAgendaRegal(string playerColor)
        {
            // TODO: Implement evaluate secret agenda regal
            return false;
        }

        public bool EvaluateSecretAgendaCommitted(string playerColor)
        {
            // TODO: Implement evaluate secret agenda committed
            return false;
        }

        public bool EvaluateSecretAgendaDignified(string playerColor)
        {
            // TODO: Implement evaluate secret agenda dignified
            return false;
        }

        public bool EvaluateSecretAgendaCourageous(string playerColor)
        {
            // TODO: Implement evaluate secret agenda courageous
            return false;
        }

        public bool EvaluateSecretAgendaWealthy(string playerColor)
        {
            // TODO: Implement evaluate secret agenda wealthy
            return false;
        }

        public void BeginActionPhaseAction(string color, string action)
        {
            // TODO: Implement begin action phase action
        }

        public List<int> GetAdvisorsForRound(int numberOfPlayers, int round)
        {
            // TODO: Implement get advisors for round
            return new List<int>();
        }

        public Player GetPlayer(string color)
        {
            // TODO: Implement get player
            return null;
        }

        public void ValidateGameStatus(string desiredState, string method, string message = null)
        {
            // TODO: Implement validate game status
        }

        public void ValidateGameStatus2(List<string> possibleStates, string method, string message = null)
        {
            // TODO: Implement validate game status2
        }

        public Player ValidateCurrentPlayer(string color, string method)
        {
            // TODO: Implement validate current player
            return null;
        }

        public void ThrowError(string message, string method)
        {
            // TODO: Implement throw error
        }
    }
}
