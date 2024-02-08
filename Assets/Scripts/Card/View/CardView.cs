using DG.Tweening;
using Game.Boot;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Cards
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Canvas _overlay;
        [SerializeField] private Image _art;
        [SerializeField] private Image _shine;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private CardValueCounter _healthPoints;
        [SerializeField] private CardValueCounter _attackPoints;
        [SerializeField] private CardValueCounter _manaPoints;

        private int _cachedSortingLayerIndex;
        private Tween _shineTween;
        private BaseCard _cardReference;


        public BaseCard CardReference => _cardReference;
        public CardValueCounter ManaPoints => _manaPoints;
        public CardValueCounter AttackPoints => _attackPoints;
        public CardValueCounter HealthPoints => _healthPoints;

        
        [Inject]
        private void Construct([Inject(Id = SceneCameraType.Main)]Camera mainCamera)
        {
            _overlay.worldCamera = mainCamera;
        }

        
        private void Awake()
        {
            SetActiveShine(false);
        }
        

        public void SetupViewData(BaseCard cardReference, CardData cardDataAsset)
        {
            _cardReference = cardReference;
            
            if(cardDataAsset.CardImage != null)
                _art.sprite = cardDataAsset.CardImage;
            
            _title.text = cardDataAsset.Title;
            _description.text = cardDataAsset.Description;
            _healthPoints.Setup(_cardReference, cardDataAsset.HealthPoints);
            _attackPoints.Setup(_cardReference, cardDataAsset.AttackPoints);
            _manaPoints.Setup(_cardReference, cardDataAsset.ManaPoints);
            _cachedSortingLayerIndex = _overlay.sortingOrder;
        }

        public void SetActiveShine(bool shineActive)
        {
            _shine.gameObject.SetActive(shineActive);
            if(_shineTween.IsActive())
                _shineTween.Kill();
            
            if (shineActive)
            {
                _shineTween = _shine.DOFade(0f, 1f)
                    .ChangeStartValue(Color.white)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetAutoKill(true)
                    .Play();
            }
        }

        public void BringViewToFront()
        {
            _overlay.sortingOrder += 10;
        }

        public void BringViewToDefault()
        {
            _overlay.sortingOrder = _cachedSortingLayerIndex;
        }
    }
}