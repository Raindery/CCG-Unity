using UnityEngine;

namespace Game.Cards
{
    public abstract class CardExtensionComponent : MonoBehaviour
    {
        [SerializeField] private BaseCard _card;
        [SerializeField] private bool _active = true;

        
        public BaseCard Card => _card;
        
        public bool Active => _active;

        
        public void SetCard(BaseCard card)
        {
            _card = card;
        }

        public void SetExtensionActive(bool active)
        {
            _active = active;
        }
    }
}