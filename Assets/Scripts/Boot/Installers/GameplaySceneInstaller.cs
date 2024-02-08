using Game.Gameplay;
using UnityEngine;
using Zenject;

namespace Game.Boot
{
    public class GameplaySceneInstaller : MonoInstaller
    {
        [SerializeField] private PlayerHand _playerHand;
        [SerializeField] private CardPlayField _cardPlayField;
        [SerializeField] private CardDispenser _cardDispenser;
        
        public override void InstallBindings()
        {
            BindGameStateDispatcher();
            BindCardPlayField();
            BindCardDispenser();
            BindMainCamera();
            BindPlayerHand();
        }

        private void BindGameStateDispatcher()
        {
            Container.Bind<GameStateDispatcher>().FromNew().AsSingle();
        }

        private void BindCardDispenser()
        {
            Container.Bind<CardDispenser>().FromInstance(_cardDispenser).AsSingle();
        }

        private void BindCardPlayField()
        {
            Container.Bind<CardPlayField>().FromInstance(_cardPlayField).AsSingle();
        }

        private void BindMainCamera()
        {
            Container.Bind<Camera>().WithId(SceneCameraType.Main).FromInstance(Camera.main).AsSingle();
        }

        private void BindPlayerHand()
        {
            Container.Bind<PlayerHand>().FromInstance(_playerHand).AsSingle().NonLazy();
        }
    }
}


