using System;
using System.Collections.Generic;
using Netologia.Necro.Units;
using UnityEngine;

namespace Netologia.Necro.Managers
{
    public class CellManager : MonoBehaviour
    {
        private static readonly Dictionary<Vector2Int, NeighbourType> _directions = new Dictionary<Vector2Int, NeighbourType>
        {
            { new Vector2Int(-1, 0), NeighbourType.Left },
            { new Vector2Int(1, 0), NeighbourType.Right },
            { new Vector2Int(0, 1), NeighbourType.Forward },
            { new Vector2Int(0, -1), NeighbourType.Back },
            { new Vector2Int(-1, 1), NeighbourType.ForwardLeft },
            { new Vector2Int(1, 1), NeighbourType.ForwardRight },
            { new Vector2Int(-1, -1), NeighbourType.BackLeft },
            { new Vector2Int(1, -1), NeighbourType.BackRight },
        };

        [SerializeField, Min(0.1f)]
        private float _cellSize = 1f;

        private Cell[] _cells;

        public event Action<Cell> OnCellClicked;

        private void Start()
        {
            _cells = FindObjectsOfType<Cell>();

            foreach (var cell in _cells)
            {
                FindNeighbours(cell);
                cell.OnPointerClickEvent += OnCellClick;
            }

            foreach (var unit in FindObjectsOfType<Unit>())
            {
                var cell = FindCellUnderUnit(unit);
                unit.Cell = cell;
                cell.Unit = unit;
            }
        }

        private void FindNeighbours(Cell cell)
        {
            foreach (var other in _cells)
            {
                if (other == cell)
                    continue;

                var delta = other.transform.position - cell.transform.position;
                var offset = new Vector2Int(
                    Mathf.RoundToInt(delta.x / _cellSize),
                    Mathf.RoundToInt(delta.z / _cellSize));

                if (_directions.TryGetValue(offset, out var type))
                    cell.SetNeighbour(type, other);
            }
        }

        private Cell FindCellUnderUnit(Unit unit)
        {
            Cell result = null;
            var minDistance = float.MaxValue;

            foreach (var cell in _cells)
            {
                var delta = cell.transform.position - unit.transform.position;
                delta.y = 0f;
                if (delta.sqrMagnitude < minDistance)
                {
                    minDistance = delta.sqrMagnitude;
                    result = cell;
                }
            }
            return result;
        }

        private void OnCellClick(Cell cell)
            => OnCellClicked?.Invoke(cell);

        private void OnDestroy()
        {
            if (_cells == null)
                return;
            foreach (var cell in _cells)
                cell.OnPointerClickEvent -= OnCellClick;
        }
    }
}
