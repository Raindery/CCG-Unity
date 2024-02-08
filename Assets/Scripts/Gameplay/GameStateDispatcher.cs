using System;

namespace Game.Gameplay
{
    public enum GameState
    {
        StartGame,
        PlayerTurn
    }
    
    public class GameStateDispatcher
    {
        private GameState _state;
        private readonly CardDispenser _cardDispenser;
        public event Action<GameState> StateChanged;


        public GameState State
        {
            get => _state;
            private set
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }


        public GameStateDispatcher(CardDispenser cardDispenser, GameState gameState = GameState.StartGame)
        {
            State = gameState;
            _cardDispenser = cardDispenser;
            _cardDispenser.CardDispenseEnded += OnCardDispenseEnded;
        }

        
        private void OnCardDispenseEnded()
        {
            _cardDispenser.CardDispenseEnded -= OnCardDispenseEnded;
            State = GameState.PlayerTurn;
        }
    }
}