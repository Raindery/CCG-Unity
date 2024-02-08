using Cysharp.Threading.Tasks;
using Game.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Game.Boot
{
    public class Boot : MonoBehaviour
    {
        private PlayerCardCollection _playerCardCollection;

        
        [Inject]
        private void Construct(PlayerCardCollection playerCardCollection)
        {
            _playerCardCollection = playerCardCollection;
        }
        
        
        public async UniTaskVoid Start()
        {
            await _playerCardCollection.InitializeCardCollection();
            await SceneManager.LoadSceneAsync(1);
        }
    }
}