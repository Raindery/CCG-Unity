using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Cards
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class BaseCard : MonoBehaviour
    {
        [Header("Base")]
        [SerializeField] private CardView _cardView;
        [SerializeField] private BoxCollider2D _boxCollider2D;
        
        private readonly Dictionary<Type, CardExtensionComponent> _cardExtensions = new Dictionary<Type, CardExtensionComponent>();
        private Transform _cachedTransform;
        private DiContainer _diContainer;

        
        public Transform CachedTransform
        {
            get
            {
                _cachedTransform ??= transform;
                return _cachedTransform;
            }
        }
        public BoxCollider2D BoxCollider2D => _boxCollider2D;
        public CardView CardView => _cardView;
        protected DiContainer DiContainer => _diContainer;


        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            _boxCollider2D ??= GetComponent<BoxCollider2D>();
            _cardView ??= GetComponent<CardView>();

            if (_boxCollider2D != null)
            {
                _boxCollider2D.isTrigger = true;
            }
        }
#endif

        
        public void SetupData(CardData cardDataAsset)
        {
            if(_cardView == null)
                return;
            _cardView.SetupViewData(this, cardDataAsset);
        }

        public void SetActiveShine(bool active)
        {
            _cardView.SetActiveShine(active);
        }

        public T AddExtension<T>()
            where T : CardExtensionComponent
        {
            var extensionComponent = _diContainer.InstantiateComponent<T>(gameObject);
            extensionComponent.SetCard(this);
            _cardExtensions.Add(typeof(T), extensionComponent);
            return extensionComponent;
        }

        public void RemoveExtension<T>()
            where T : CardExtensionComponent
        {
            Type type = typeof(T);
            if (!_cardExtensions.TryGetValue(type, out CardExtensionComponent extension))
            {
                Debug.LogWarning($"Card has no extension with type {type}");
                return;
            }
            
            Destroy(extension);
            _cardExtensions.Remove(type);
        }

        public bool TryGetExtension<T>(out T extension)
            where T : CardExtensionComponent
        {
            Type type = typeof(T);
            if (!_cardExtensions.TryGetValue(type, out CardExtensionComponent extensionComponent))
            {
                extension = null;
                return false;
            }
            
            extension = extensionComponent as T; 
            return true;
        }

        public bool HasExtension<T>()
        {
            return _cardExtensions.ContainsKey(typeof(T));
        }
    }
}