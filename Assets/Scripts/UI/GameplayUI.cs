using System;
using Cysharp.Threading.Tasks;
using Game.Cards;
using Game.Gameplay;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Button _decreaseRandomCardValueInHandButton;
        
        private GameStateDispatcher _gameStateDispatcher;
        private PlayerHand _playerHand;
        
        
        [Inject]
        private void Construct(GameStateDispatcher gameStateDispatcher, PlayerHand playerHand)
        {
            _gameStateDispatcher = gameStateDispatcher;
            _playerHand = playerHand;
            _decreaseRandomCardValueInHandButton.interactable = false;
            gameStateDispatcher.StateChanged += OnGameStateChanged;
            _playerHand.CardRemoved += OnAnyCardRemovedInPlayerHand;
        }

        
        private void OnAnyCardRemovedInPlayerHand(PlayerHand sender, BaseCard removedCard)
        {
            if(_playerHand.CardsInHand.Count > 0)
                return;

            _decreaseRandomCardValueInHandButton.interactable = false;
            _decreaseRandomCardValueInHandButton.onClick.RemoveListener(OnClickDecreaseRandomCardValueInHandButton);
        }
        
        private void OnGameStateChanged(GameState gameState)
        {
            if(gameState != GameState.PlayerTurn)
                return;

            _gameStateDispatcher.StateChanged -= OnGameStateChanged;
            _decreaseRandomCardValueInHandButton.onClick.AddListener(OnClickDecreaseRandomCardValueInHandButton);
            _decreaseRandomCardValueInHandButton.interactable = true;
        }

        private async void OnClickDecreaseRandomCardValueInHandButton()
        {
            _decreaseRandomCardValueInHandButton.interactable = false;
            
            while (_playerHand.CardsInHand.Count > 0)
            {
                for (int i = 0; i < _playerHand.CardsInHand.Count; i++)
                {
                    var card = _playerHand.CardsInHand[i];
                    int randomNumberOfTypeValue = Random.Range(0, 3);
                    int randomChangedValue = Random.Range(-2, 10);

                    CardValueCounter changedCardValueCounter;
                    switch (randomNumberOfTypeValue)
                    {
                        case 0:
                            changedCardValueCounter = card.CardView.AttackPoints;
                            break;
                        case 1:
                            changedCardValueCounter = card.CardView.HealthPoints;
                            break;
                        case 2:
                            changedCardValueCounter = card.CardView.ManaPoints;
                            break;
                        default:
                            Debug.LogWarning("Changed value counter is not found");
                            return;
                    }
                    changedCardValueCounter.ChangeValue(randomChangedValue);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
                }
            }
        }
    }
}

