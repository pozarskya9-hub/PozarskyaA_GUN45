using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Netologia.Necro.Managers
{
    public class InputManager : MonoBehaviour
    {
        private SceneController _sceneController;   //injected
        private Controls.GameActions _controls;     //injected

        private Coroutine _restartCoroutine;

        [SerializeField]
        private GameObject _restartUI;
        [SerializeField]
        private Image _restartFill;
        [SerializeField, Range(0.1f, 3f)]
        private float _restartPushInSec = 1f;       //сколько секунд нужно держать кнопку


        private void OnRestartPerformed(InputAction.CallbackContext obj)
        {
            _restartUI.SetActive(true);
            _restartCoroutine = StartCoroutine(Restarter());
        }
        private void OnRestartCanceled(InputAction.CallbackContext obj)
        {
            if (_restartFill == null)       //объекты сцены уже уничтожены (выход из Play или перезагрузка)
                return;

            if (_restartCoroutine != null)
                StopCoroutine(_restartCoroutine);
            _restartFill.fillAmount = 0f;
            _restartUI.SetActive(false);
        }
        private IEnumerator Restarter()
        {
            while (true)
            {
                var value = _restartFill.fillAmount + Time.deltaTime / _restartPushInSec;
                _restartFill.fillAmount = value;
                if (value >= 1f)
                {
                    _sceneController.OpenGameScene();
                    yield break;
                }
                yield return null;
            }
        }
        private void Start()
        {
            _controls.Restart.performed += OnRestartPerformed;
            _controls.Restart.canceled += OnRestartCanceled;
            _restartFill.fillAmount = 0f;
            _restartUI.SetActive(false);
        }

        private void OnDestroy()
        {
            _controls.Restart.performed -= OnRestartPerformed;
            _controls.Restart.canceled -= OnRestartCanceled;
        }
        [Inject]
        private void Construct(SceneController sceneController, Controls.GameActions controls)
        {
            _sceneController = sceneController;
            _controls = controls;
        }
    }
}
