using DG.Tweening;
using UnityEngine;

namespace Game.Gameplay
{
    public class CardPlayFieldView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _fieldImage;
        [SerializeField] private Color _activeColor;

        private Color _defaultColor;
        private Tween _changeViewActiveTween;
        private bool _active;


        private void Awake()
        {
            _defaultColor = _fieldImage.color;
        }

        
        public void SetFieldToActive(bool active)
        {
            if(_active == active)
                return;
            
            if(_changeViewActiveTween.IsActive())
                _changeViewActiveTween.Kill();
            _changeViewActiveTween = _fieldImage.DOColor(active ? _activeColor : _defaultColor, 0.2f).SetAutoKill(true).Play();
            
            _active = active;
        }
    }
}