using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game.Cards
{
    public class CardValueCounter : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TMP_Text _textValue;

        private BaseCard _cardReference;
        private Tween _counterAnimationTween;
        private int _value;
        
        public event Action<CardValueCounter, int> ValueChanged;
        public event Action<CardValueCounter, int> ValueDecreased;
        public event Action<CardValueCounter, int> ValueIncreased;

        
        public int Value => _value;
        public BaseCard CardReference => _cardReference;
        

        public void Setup(BaseCard cardReference, int startValue)
        {
            _cardReference = cardReference;
            _value = startValue;
            _textValue.text = _value.ToString();
        }

        public void DecreaseValue(int decreasedValue, float counterAnimationDuration = 0.25f)
        {
            int newValue;
            if (decreasedValue > _value)
                newValue = 0;
            else
                newValue = _value - decreasedValue;
            
            ChangeValue(newValue, counterAnimationDuration);
            ValueDecreased?.Invoke(this, _value);
        }

        public void IncreaseValue(int increasedValue, float counterAnimationDuration = 0.25f)
        {
            if(increasedValue < 0)
                throw new ArgumentOutOfRangeException(nameof(increasedValue), "Cannot increase value! Increased value is less than zero.");
            if(increasedValue == 0)
                return;

            int newValue = _value + increasedValue;
            ChangeValue(newValue, counterAnimationDuration);
            ValueIncreased?.Invoke(this, _value);
        }

        public void ChangeValue(int newValue, float counterAnimationDuration = 0.25f)
        {
            int oldValue = _value;
            _value = newValue;
            ValueChanged?.Invoke(this, _value);
            
            if(_counterAnimationTween.IsActive())
                _counterAnimationTween.Kill();
            _counterAnimationTween = DOTween.To(() => oldValue, value => oldValue = value, _value, counterAnimationDuration)
                .OnUpdate(() => _textValue.text = oldValue.ToString())
                .SetAutoKill(true)
                .Play();
        }
    }
}


