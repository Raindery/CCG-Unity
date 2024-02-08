using Game.Player;
using UnityEngine;

namespace Game.Boot
{
    public class GlobalInstaller : Zenject.MonoInstaller
    {
        [SerializeField] private PlayerCardCollection _playerCardCollection;


        public override void InstallBindings()
        {
            BindPlayerCardCollection();
        }

        private void BindPlayerCardCollection()
        {
            Container.Bind<PlayerCardCollection>().FromInstance(_playerCardCollection).AsSingle();
        }
    }
}