using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Cards;
using Game.Player;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Gameplay
{
    public class CardDispenser : MonoBehaviour
    {
        [SerializeField] private BaseCard _cardPrefab;
        [SerializeField] private int _minCardCount = 4;
        [SerializeField] private int _maxCardCount = 6;
        [SerializeField] private Transform _instantiatePoint;
        [SerializeField] private Transform _dispensePoint;
        
        private PlayerCardCollection _playerCardCollection;
        private PlayerHand _playerHand;
        private DiContainer _diContainer;

        public event Action CardDispenseEnded;
        
        
        [Inject]
        private void Construct(DiContainer diContainer, PlayerCardCollection playerCardCollection, PlayerHand playerHand)
        {
            _playerCardCollection = playerCardCollection;
            _diContainer = diContainer;
            _playerHand = playerHand;
        }

        
        private async void Start()
        {
            int startCardCount =  Random.Range(_minCardCount, _maxCardCount + 1);
            var startDispenseSequence = DOTween.Sequence();
            var startCards = new Queue<BaseCard>();
            for (int i = 0; i < startCardCount; i++)
            {
                var card = _diContainer
                    .InstantiatePrefabForComponent<Card>(_cardPrefab, _instantiatePoint.position, Quaternion.identity, transform);
                card.SetupData(_playerCardCollection.Cards[Random.Range(0, _playerCardCollection.Cards.Length)]);

                startDispenseSequence
                    .Append(card.CachedTransform.DOMove(_dispensePoint.position, 0.5f)
                        .SetEase(Ease.InExpo))
                    .Append(card.CachedTransform.DOMove(_playerHand.transform.position, 0.5f)
                        .SetEase(Ease.OutExpo));
                
                startCards.Enqueue(card);
            }
            
            await startDispenseSequence.SetAutoKill(true).Play().AsyncWaitForCompletion();
            await _playerHand.AddRangeCardsAsync(startCards);
            CardDispenseEnded?.Invoke();
        }
    }
}