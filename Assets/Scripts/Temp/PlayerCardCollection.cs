using Cysharp.Threading.Tasks;
using Game.Cards;
using UnityEngine;

namespace Game.Player
{
    [CreateAssetMenu(fileName = "PlayerCardCollection", menuName = "Game/Create player card collection data asset")]
    public class PlayerCardCollection : ScriptableObject
    {
        [SerializeField] private CardData[] _cards;

        public CardData[] Cards => _cards;


        public async UniTask InitializeCardCollection()
        {
            var cardDataInitTasks = new UniTask[_cards.Length];
            for (int i = 0; i < _cards.Length; i++)
            {
                cardDataInitTasks[i] = _cards[i].Initialize();
            }

            await UniTask.WhenAll(cardDataInitTasks);
        }
    }
}