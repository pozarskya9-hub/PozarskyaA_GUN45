using Zenject;

public class MainInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var sceneController = GetComponent<SceneController>();
        Container.BindInstance(sceneController).AsSingle();
    }
}