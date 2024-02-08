using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Cards;
using UnityEngine;
using Zenject;

namespace Game.Gameplay
{
    public class CardPlayField : MonoBehaviour
    {
        [Header("View")] 
        [SerializeField] private CardPlayFieldView _fieldView;
        [Header("Field Settings")]
        [SerializeField] private Bounds _fieldBounds;
        [SerializeField] private Transform _cardParent;
        [SerializeField] private float _cardXSpacing = 2f;
        [SerializeField] private int _maxCardOnField = 5;
        
        private readonly List<BaseCard> _cardsOnField = new List<BaseCard>();
        private InPlayerHandCardExtension _currentMovableInPlayerHandCardExtension;
        private PlayerHand _playerHand;
        private Sequence _recalculateCardsPositionsSequence;

        
        [Inject]
        private void Construct(PlayerHand playerHand)
        {
            _playerHand = playerHand;
        }
        

#if UNITY_EDITOR
        private void OnValidate()
        {
            _cardParent ??= transform;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(_fieldBounds.center, _fieldBounds.size);
        }
#endif

        private async UniTask RecalculateCardsPositions()
        {
            if(_recalculateCardsPositionsSequence.IsActive())
                _recalculateCardsPositionsSequence.Kill();
            _recalculateCardsPositionsSequence = DOTween.Sequence();
            
            bool isCardCountEvent = _cardsOnField.Count % 2 == 0;
            int roundHalfCardCount = _cardsOnField.Count / 2;
            float startX;
            if (isCardCountEvent)
                startX = -(_cardXSpacing * roundHalfCardCount) + _cardXSpacing / 2f;
            else
                startX = -(_cardXSpacing * roundHalfCardCount);
            
            Vector2 newCardPosition = new Vector2(startX, 0f);
            for (int i = 0; i < _cardsOnField.Count; i++)
            {
                var card = _cardsOnField[i];

                _recalculateCardsPositionsSequence
                    .Join(card.CachedTransform.DOLocalMove(newCardPosition, 0.2f))
                    .Join(card.CachedTransform.DOLocalRotate(Quaternion.identity.eulerAngles, 0.1f));
                    
                newCardPosition.x += _cardXSpacing;
            }
            
            await _recalculateCardsPositionsSequence.SetEase(Ease.InOutExpo).SetAutoKill(true).Play().AsyncWaitForCompletion();
        }

        private void OnCurrentMovableCardDragged(BaseCard card)
        {
            if (IsMovableCardInPlayZone() && _cardsOnField.Count < _maxCardOnField)
                _fieldView.SetFieldToActive(true);
            else
                _fieldView.SetFieldToActive(false);
        }

        private void OnCurrentMovableCardEndDrag(BaseCard card)
        {
            _fieldView.SetFieldToActive(false);
            if (IsMovableCardInPlayZone() && IsCanAddCardToField())
            {
                _playerHand.RemoveCard(card);
                AddCardOnField(card);
            }
            SetCurrentMovableCard();
        }

        public bool IsMovableCardInPlayZone()
        {
            return _currentMovableInPlayerHandCardExtension != null && 
                   _fieldBounds.Intersects(_currentMovableInPlayerHandCardExtension.Card.BoxCollider2D.bounds);
        }

        public void SetCurrentMovableCard(InPlayerHandCardExtension inPlayerHandCardExtension = null)
        {
            if (inPlayerHandCardExtension == null)
            {
                _currentMovableInPlayerHandCardExtension.CardDragged -= OnCurrentMovableCardDragged;
                _currentMovableInPlayerHandCardExtension.CardDragEnd -= OnCurrentMovableCardEndDrag;
            }
            else
            {
                inPlayerHandCardExtension.CardDragged += OnCurrentMovableCardDragged;
                inPlayerHandCardExtension.CardDragEnd += OnCurrentMovableCardEndDrag;
            }
            
            _currentMovableInPlayerHandCardExtension = inPlayerHandCardExtension;
        }

        public bool IsCanAddCardToField()
        {
            return _cardsOnField.Count < _maxCardOnField;
        }

        public void AddCardOnField(BaseCard card)
        {
            card.CachedTransform.SetParent(_cardParent);
            _cardsOnField.Add(card);
            RecalculateCardsPositions().Forget();
        }

        public void RemoveCardFromField(BaseCard card)
        {
            if(!_cardsOnField.Contains(card))
                return;

            _cardsOnField.Remove(card);
            Destroy(card);
            RecalculateCardsPositions().Forget();
        }
    }
}