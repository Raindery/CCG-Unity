using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Cards
{
    [CreateAssetMenu(fileName = "CardData", menuName = "Game/Create card data asset")]
    public class CardData : ScriptableObject
    {
        [SerializeField] private string _title = "Title";
        [SerializeField, TextArea] private string _description = "Description";
        [SerializeField, Min(1)] private int _healthPoints = 1;
        [SerializeField, Min(0)] private int _attackPoints;
        [SerializeField, Min(0)] private int _manaPoints;
        [SerializeField, Min(10)] private int _imageSize = 200;
        
        private Sprite _cardImage;

        
        public string Title => _title;
        public string Description => _description;
        public int HealthPoints => _healthPoints;
        public int AttackPoints => _attackPoints;
        public int ManaPoints => _manaPoints;
        public Sprite CardImage => _cardImage;
        
        
        public async UniTask Initialize()
        {
            string downloadImageUri = $@"https://picsum.photos/{_imageSize}";
            Texture2D cardImageTexture;
            using (var textureRequest = UnityWebRequestTexture.GetTexture(downloadImageUri))
            {
                await textureRequest.SendWebRequest();
                cardImageTexture = DownloadHandlerTexture.GetContent(textureRequest);
            }
            
            _cardImage = Sprite.Create(cardImageTexture, new Rect(Vector2.zero, Vector2.one * _imageSize),
                Vector2.zero);
        }
    }
}