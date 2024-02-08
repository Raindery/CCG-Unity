using System;
using DG.Tweening;
using Game.Boot;
using Game.Gameplay;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Cards
{
    public class InPlayerHandCardExtension : CardExtensionComponent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Camera _mainCamera;
        private Vector2 _inHandPosition;
        private Quaternion _inHandRotation;
        private CardPlayField _cardPlayField;
        private Sequence _returnInHandTween;
        private bool _isMouseHasBeenDown;
        private InPlayerHandFrontViewCardExtension _inPlayerHandFrontViewCardExtension;
        
        public event Action<BaseCard> CardDragStart;
        public event Action<BaseCard> CardDragged;
        public event Action<BaseCard> CardDragEnd;


        public Vector2 InHandPosition
        {
            get => _inHandPosition;
            set => _inHandPosition = value;
        }
        public Quaternion InHandRotation
        {
            get => _inHandRotation;
            set => _inHandRotation = value;
        }

        
        [Inject]
        private void Construct([Inject(Id = SceneCameraType.Main)] Camera mainCamera, CardPlayField cardPlayField)
        {
            _mainCamera = mainCamera;
            _cardPlayField = cardPlayField;
        }
        

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!Active)
                return;
            if (_inPlayerHandFrontViewCardExtension == null)
                Card.TryGetExtension(out _inPlayerHandFrontViewCardExtension);
            if(_returnInHandTween.IsActive())
                _returnInHandTween.Kill();
            
            _isMouseHasBeenDown = true;
            _cardPlayField.SetCurrentMovableCard(this);
            CardDragStart?.Invoke(Card);
            Card.CachedTransform.localRotation = Quaternion.identity;
            Card.CachedTransform.position = (Vector2)_mainCamera.ScreenToWorldPoint(eventData.position);
            
            if(_inPlayerHandFrontViewCardExtension != null)
                _inPlayerHandFrontViewCardExtension.BringViewToFront();

            Card.SetActiveShine(true);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if(!Active)
                return;
            if(!_isMouseHasBeenDown)
                OnBeginDrag(eventData);
            
            CardDragged?.Invoke(Card);
            Card.CachedTransform.position = (Vector2)_mainCamera.ScreenToWorldPoint(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!Active)
                return;
            if(_returnInHandTween.IsActive())
                _returnInHandTween.Kill();
            
            if (!_cardPlayField.IsMovableCardInPlayZone() || !_cardPlayField.IsCanAddCardToField())
            {
                SetExtensionActive(false);

                _returnInHandTween = DOTween.Sequence();
                _returnInHandTween
                    .Join(Card.CachedTransform
                        .DOLocalMove(_inHandPosition, 0.5f)
                        .SetEase(Ease.OutExpo))
                    .Join(Card.CachedTransform.DOLocalRotate(_inHandRotation.eulerAngles, 0.1f))
                    .OnComplete(() =>
                    {
                        if (_inPlayerHandFrontViewCardExtension != null)
                            _inPlayerHandFrontViewCardExtension.BringViewToDefault();
                        SetExtensionActive(true);
                    })
                    .SetAutoKill(true)
                    .Play();
            }
            else
            {
               if(_inPlayerHandFrontViewCardExtension != null)
                   _inPlayerHandFrontViewCardExtension.BringViewToDefault();
            }

            _isMouseHasBeenDown = false;
            Card.SetActiveShine(false);
            CardDragEnd?.Invoke(Card);
        }
    }
}