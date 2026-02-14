using System;
using System.Collections.Generic;

namespace rurik
{

    public class GameStates
    {
        private Dictionary<string, GameState> gameStates;
        private GameState currentState;

        public GameStates()
        {
            gameStates = new Dictionary<string, GameState>();
            gameStates["waitingForPlayers"] = new GameState("waitingForPlayers", new List<string> { "joinGame", "startGame" });
            gameStates["waitingForFirstPlayerSelection"] = new GameState("waitingForFirstPlayerSelection", new List<string> { "selectFirstPlayer" });
            gameStates["waitingForLeaderSelection"] = new GameState("waitingForLeaderSelection", new List<string> { "chooseLeader" });
            gameStates["waitingForSecretAgendaSelection"] = new GameState("waitingForSecretAgendaSelection", new List<string> { "chooseSecretAgenda" });
            gameStates["waitingForTroopPlacement"] = new GameState("waitingForTroopPlacement", new List<string> { "placeTroop" });
            gameStates["waitingForLeaderPlacement"] = new GameState("waitingForLeaderPlacement", new List<string> { "placeLeader" });
            gameStates["strategyPhase"] = new GameState("strategyPhase", new List<string> { "playAdvisor" });
            gameStates["retrieveAdvisor"] = new GameState("retrieveAdvisor", new List<string> { "retrieveAdvisor" });
            gameStates["actionPhase"] = new GameState("actionPhase", new List<string> { "takeMainAction", "playSchemeCard", "accomplishDeed", "convertGoods", "endTurn" });
            gameStates["actionPhaseMuster"] = new GameState("actionPhaseMuster", new List<string> { "muster", "cancel" });
            gameStates["actionPhaseMove"] = new GameState("actionPhaseMove", new List<string> { "move", "cancel" });
            gameStates["actionPhaseAttack"] = new GameState("actionPhaseAttack", new List<string> { "attack", "cancel" });
            gameStates["actionPhaseTax"] = new GameState("actionPhaseTax", new List<string> { "tax", "cancel" });
            gameStates["actionPhaseBuild"] = new GameState("actionPhaseBuild", new List<string> { "build", "cancel" });
            gameStates["actionPhaseTransfer"] = new GameState("actionPhaseTransfer", new List<string> { "transfer", "cancel" });
            gameStates["actionPhasePlaySchemeCard"] = new GameState("actionPhasePlaySchemeCard", new List<string> { "playSchemeCard", "cancel" });
            gameStates["actionPhasePlayConversionTile"] = new GameState("actionPhasePlayConversionTile", new List<string> { "playConversionTile", "cancel" });
            gameStates["actionPhaseAccomplishDeed"] = new GameState("actionPhaseAccomplishDeed", new List<string> { "accomplishDeed", "cancel" });
            gameStates["actionPhaseVerifyDeed"] = new GameState("actionPhaseVerifyDeed", new List<string> { "verifyDeed", "rejectDeed" });
            gameStates["selectSchemeCard"] = new GameState("selectSchemeCard", new List<string> { "returnSchemeCard" });
            gameStates["schemeFirstPlayer"] = new GameState("schemeFirstPlayer", new List<string> { "assignFirstPlayer" });
            gameStates["drawSchemeCards"] = new GameState("drawSchemeCards", new List<string> { "drawSchemeCards" });
            gameStates["takeDeedCardForActionPhase"] = new GameState("takeDeedCardForActionPhase", new List<string> { "takeSchemeCard" });
            gameStates["takeDeedCardForClaimPhase"] = new GameState("takeDeedCardForClaimPhase", new List<string> { "takeSchemeCard" });
            gameStates["claimPhase"] = new GameState("claimPhase", new List<string> { "chooseDeedCard" });
            gameStates["endGame"] = new GameState("endGame", new List<string>());

            setCurrentState("waitingForPlayers");
        }

        public void setCurrentState(string stateName)
        {
            currentState = gameStates[stateName];
        }

        public GameState getCurrentState()
        {
            return currentState;
        }
    }
}
