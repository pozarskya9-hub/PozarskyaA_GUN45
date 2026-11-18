using System;
using System.Collections.Generic;
using Netologia.Necro.Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Netologia.Necro
{
    public class Cell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField]
        private MeshRenderer _focus;
        [SerializeField]
        private MeshRenderer _select;

        private readonly Dictionary<NeighbourType, Cell> _neighbours = new Dictionary<NeighbourType, Cell>();

        public Unit Unit { get; set; }

        public event Action<Cell> OnPointerClickEvent;

        public void SetNeighbour(NeighbourType type, Cell cell)
            => _neighbours[type] = cell;

        public bool TryGetNeighbour(NeighbourType type, out Cell cell)
            => _neighbours.TryGetValue(type, out cell);

        public void SetSelect(Material material)
            => (_select.enabled, _select.sharedMaterial) = (true, material);

        public void ResetSelect()
            => _select.enabled = false;

        public void OnPointerClick(PointerEventData eventData)
            => OnPointerClickEvent?.Invoke(this);

        public void OnPointerEnter(PointerEventData eventData)
        {
            _focus.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _focus.enabled = false;
        }
    }
}
