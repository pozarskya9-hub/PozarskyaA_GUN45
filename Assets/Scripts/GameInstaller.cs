using Netologia.Necro;
using Netologia.Necro.Managers;
using Netologia.Necro.Settings;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    private Controls _controls;
    [SerializeField]
    private CellManager _cellManager;
    [SerializeField]
    private SceneController _controller;

    [SerializeField, Space(15f)]
    private CellPaletteSettings _cellPaletteSettings;

    public override void InstallBindings()
    {
        _controls = new Controls();
        _controls.Game.Enable();
        Container.BindInstance(_controls.Game).AsSingle();

        Container.BindInstance(_cellManager).AsSingle();
        Container.BindInstance(_controller).AsSingle();

        Container.BindInstance(_cellPaletteSettings).AsSingle();

        //for test
        _cellManager.OnCellClicked += OnCellClicked;
    }

    private void OnCellClicked(Cell cell)
    {
        cell.SetSelect(_cellPaletteSettings.SelectCell);
    }

    private void OnDestroy()
    {
        _cellManager.OnCellClicked -= OnCellClicked;
        _controls.Disable();
    }
}
