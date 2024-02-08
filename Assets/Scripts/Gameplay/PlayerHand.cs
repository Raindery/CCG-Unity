using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cards;
using UnityEngine;

namespace Game.Gameplay
{
    public class PlayerHand : MonoBehaviour
    {
        [SerializeField] private float _cardXSpacing = 1.5f;
        [SerializeField] private float _cardRotationStep = 5f;
        [SerializeField] private Transform _removeCardPoint;
        
        private readonly List<BaseCard> _cards = new List<BaseCard>();
        private Sequence _repositionSequence;

        public event Action<PlayerHand, IEnumerable<BaseCard>> CardRangeAdded;
        public event Action<PlayerHand, BaseCard> CardAdded;
        public event Action<PlayerHand, BaseCard> CardRemoved;

        public IReadOnlyList<BaseCard> CardsInHand => _cards;

        
        private void Awake()
        {
            CardAdded += OnCardAdded;
            CardRangeAdded += OnCardRangeAdded;
            CardRemoved += OnCardRemoved;
        }

        private void OnDestroy()
        {
            CardAdded -= OnCardAdded;
            CardRangeAdded -= OnCardRangeAdded;
            CardRemoved -= OnCardRemoved;
        }
        

        public void AddRangeCards(IEnumerable<BaseCard> cardRange)
        {
            var cardList = cardRange.ToList();
            for (int i = 0; i < cardList.Count; i++)
            {
                var card = cardList[i];
                card.CachedTransform.SetParent(transform);
                if (!card.HasExtension<InPlayerHandCardExtension>())
                {
                    var extension = card.AddExtension<InPlayerHandCardExtension>();
                    extension.CardDragStart += OnAnyCardStartedDrag;
                    extension.CardDragEnd += OnAnyCardEndedDrag;
                }
                
                if (!card.HasExtension<InPlayerHandFrontViewCardExtension>())
                    card.AddExtension<InPlayerHandFrontViewCardExtension>();
            }
            _cards.AddRange(cardList);
            
            SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(false);
            UniTask.Void(async () =>
            {
                await RepositionCards();
                SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(true);
            });
            CardRangeAdded?.Invoke(this, cardList);
        }

        public async UniTask AddRangeCardsAsync(IEnumerable<BaseCard> cardRange)
        {
            var cardList = cardRange.ToList();
            for (int i = 0; i < cardList.Count; i++)
            {
                var card = cardList[i];
                card.CachedTransform.SetParent(transform);
                if (!card.HasExtension<InPlayerHandCardExtension>())
                {
                    var extension = card.AddExtension<InPlayerHandCardExtension>();
                    extension.CardDragStart += OnAnyCardStartedDrag;
                    extension.CardDragEnd += OnAnyCardEndedDrag;
                }
                
                if (!card.HasExtension<InPlayerHandFrontViewCardExtension>())
                    card.AddExtension<InPlayerHandFrontViewCardExtension>();
            }
            _cards.AddRange(cardList);
            SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(false);
            await RepositionCards();
            SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(true);
            CardRangeAdded?.Invoke(this, cardList);
        }

        public void AddCard(BaseCard card)
        {
            card.CachedTransform.SetParent(transform);
            if (!card.HasExtension<InPlayerHandCardExtension>())
            {
                var extension = card.AddExtension<InPlayerHandCardExtension>();
                extension.CardDragStart += OnAnyCardStartedDrag;
                extension.CardDragEnd += OnAnyCardEndedDrag;
            }
            if (!card.HasExtension<InPlayerHandFrontViewCardExtension>())
                card.AddExtension<InPlayerHandFrontViewCardExtension>();
            _cards.Add(card);
            
            SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(false);
            UniTask.Void(async () =>
            {
                await RepositionCards();
                SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(true);
            });
            
            CardAdded?.Invoke(this, card);
        }

        public async UniTask AddCardAsync(BaseCard card)
        {
            card.CachedTransform.SetParent(transform);
            if (!card.HasExtension<InPlayerHandCardExtension>())
            {
                var extension = card.AddExtension<InPlayerHandCardExtension>();
                extension.CardDragStart += OnAnyCardStartedDrag;
                extension.CardDragEnd += OnAnyCardEndedDrag;
            }
            
            if (!card.HasExtension<InPlayerHandFrontViewCardExtension>())
                card.AddExtension<InPlayerHandFrontViewCardExtension>();
            _cards.Add(card);

            SetActiveExtensionForCardsInHand<InPlayerHandCardExtension>(false);
            await RepositionCards();
            SetActiveExtensionForCardsInHand<InPlayerHandFrontViewCardExtension>(true);
            CardAdded?.Invoke(this, card);
        }

        public void RemoveCard(BaseCard card)
        {
            if (card.TryGetExtension(out InPlayerHandCardExtension inPlayerHandCardExtension))
            {
                inPlayerHandCardExtension.CardDragStart -= OnAnyCardStartedDrag;
                inPlayerHandCardExtension.CardDragEnd -= OnAnyCardEndedDrag;
                card.RemoveExtension<InPlayerHandCardExtension>();
            }
            if(card.HasExtension<InPlayerHandFrontViewCardExtension>())
                card.RemoveExtension<InPlayerHandFrontViewCardExtension>();
            _cards.Remove(card);
            
            UniTask.Void(async () =>
            {
                await RepositionCards();
            });
            CardRemoved?.Invoke(this, card);
        }

        public async UniTask RemoveCardAsync(BaseCard card)
        {
            if (card.TryGetExtension(out InPlayerHandCardExtension inPlayerHandCardExtension))
            {
                inPlayerHandCardExtension.CardDragStart -= OnAnyCardStartedDrag;
                inPlayerHandCardExtension.CardDragEnd -= OnAnyCardEndedDrag;
                card.RemoveExtension<InPlayerHandCardExtension>();
            }
                
            if(card.HasExtension<InPlayerHandFrontViewCardExtension>())
                card.RemoveExtension<InPlayerHandFrontViewCardExtension>();
            
            _cards.Remove(card);
            await RepositionCards();
            CardRemoved?.Invoke(this, card);
        }

        public void SetActiveExtensionForCardsInHand<T>(bool active)
            where T : CardExtensionComponent 
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                var cardInHand = _cards[i];
                if(cardInHand.TryGetExtension(out T inPlayerHandCardExtension))
                    inPlayerHandCardExtension.SetExtensionActive(active);
            }
        }

        private void OnAnyCardEndedDrag(BaseCard sender)
        {
            SetActiveExtensionForCardsInHand<InPlayerHandFrontViewCardExtension>(true);
        }

        private void OnAnyCardStartedDrag(BaseCard sender)
        {
            SetActiveExtensionForCardsInHand<InPlayerHandFrontViewCardExtension>(false);
        }

        private void OnCardRangeAdded(PlayerHand sender, IEnumerable<BaseCard> addedCardRange)
        {
            foreach (BaseCard addedCard in addedCardRange)
            {
                addedCard.CardView.HealthPoints.ValueChanged += OnAnyCardHealthValueChanged;
            }
        }

        private void OnCardAdded(PlayerHand sender, BaseCard addedCard)
        {
            addedCard.CardView.HealthPoints.ValueChanged += OnAnyCardHealthValueChanged;
        }

        private void OnCardRemoved(PlayerHand sender, BaseCard removedCard)
        {
            removedCard.CardView.HealthPoints.ValueChanged -= OnAnyCardHealthValueChanged;
        }

        private void OnAnyCardHealthValueChanged(CardValueCounter sender, int healthValue)
        {
            if(healthValue > 0)
                return;
            var card = sender.CardReference;
            RemoveCard(card);

            if (_removeCardPoint == null)
            {
                Destroy(card.gameObject);
                return;
            }

            card.CachedTransform
                .DOMoveY(_removeCardPoint.position.y, 0.5f)
                .SetEase(Ease.InBack)
                .OnComplete(() => Destroy(card.gameObject))
                .SetAutoKill(true)
                .Play();
        }

        private async UniTask RepositionCards(float moveDuration = 0.2f, float rotateDuration = 0.05f)
        {
            const float scaleYFactor = 100f;
            bool isCardCountEven = _cards.Count % 2 == 0;
            int roundHalfCardsCount = _cards.Count / 2;
            int middleLeftCardIndex = _cards.Count / 2 - 1;
            int middleRightCardIndex = middleLeftCardIndex + 1;
            float startX;
            float startRotationCardValue;
            
            if (isCardCountEven)
            {
                startX = -(_cardXSpacing * roundHalfCardsCount) + _cardXSpacing / 2f;
                startRotationCardValue = _cardRotationStep * roundHalfCardsCount - _cardRotationStep;
            }
            else
            {
                startX = -(_cardXSpacing * roundHalfCardsCount);
                startRotationCardValue = _cardRotationStep * roundHalfCardsCount;
            }
            
            Vector3 cardEulerRotation = Vector3.forward * startRotationCardValue;
            float startY = startX * cardEulerRotation.z / scaleYFactor;
            Vector2 cardPosition = new Vector2(startX, startY);
            
            if(_repositionSequence.IsActive())
                _repositionSequence.Kill();
            _repositionSequence = DOTween.Sequence();
            
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card.TryGetExtension(out InPlayerHandCardExtension inPlayerHandCardExtension))
                {
                    inPlayerHandCardExtension.InHandPosition = cardPosition;
                    inPlayerHandCardExtension.InHandRotation = Quaternion.Euler(cardEulerRotation);
                }
                _repositionSequence
                    .Join(card.CachedTransform.DOLocalMove(cardPosition, moveDuration))
                    .Join(card.CachedTransform.DOLocalRotate(cardEulerRotation, rotateDuration));

                
                cardPosition.x += _cardXSpacing;
                if (isCardCountEven && i + 1 == middleLeftCardIndex || i + 1 == middleRightCardIndex)
                {
                    cardEulerRotation.z = 0;
                    cardPosition.y = 0;
                }
                else
                {
                    cardEulerRotation.z -= _cardRotationStep;
                    cardPosition.y = cardPosition.x * cardEulerRotation.z / scaleYFactor;
                }
            }

            await _repositionSequence.SetEase(Ease.InOutExpo).SetAutoKill(true).Play().AsyncWaitForCompletion();
        }
    }
}


