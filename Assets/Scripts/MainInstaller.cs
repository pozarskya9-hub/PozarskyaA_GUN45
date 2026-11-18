using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField]
    private SceneController _controller;

    public override void InstallBindings()
    {
        Container.BindInstance(_controller).AsSingle();
    }
}
