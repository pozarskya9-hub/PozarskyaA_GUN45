using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Necro.Units
{
    public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField, Min(0.1f)]
        private float _speed = 3f;
        [field: SerializeField]
        public Team Team { get; private set; }

        private Coroutine _moveCoroutine;

        public Cell Cell { get; set; }

        public event Action OnMoveEndCallback;

        public void Move(Cell cell)
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(Mover(cell));
        }

        private IEnumerator Mover(Cell cell)
        {
            var cellPosition = cell.transform.position;
            var target = new Vector3(cellPosition.x, transform.position.y, cellPosition.z);

            while (transform.position != target)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
                yield return null;
            }

            if (Cell != null && Cell.Unit == this)
                Cell.Unit = null;
            Cell = cell;
            cell.Unit = this;

            _moveCoroutine = null;
            OnMoveEndCallback?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
            => Cell.OnPointerEnter(eventData);

        public void OnPointerClick(PointerEventData eventData)
            => Cell.OnPointerClick(eventData);

        public void OnPointerExit(PointerEventData eventData)
            => Cell.OnPointerExit(eventData);
    }
}
