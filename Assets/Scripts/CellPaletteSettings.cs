using UnityEngine;

namespace Netologia.Necro.Settings
{
    [CreateAssetMenu(fileName = "New CellPaletteSettings", menuName = "Settings/CellPaletteSettings", order = 52)]
    public class CellPaletteSettings : ScriptableObject
    {
        [field: SerializeField, Space(20f)]
        [field: Tooltip("Клетка под выбранным юнитом")]
        public Material SelectCell { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Клетка доступная для движения")]
        public Material MoveCell { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Клетка доступная для атаки")]
        public Material AttackCell { get; private set; }

        [field: SerializeField]
        [field: Tooltip("Клетка доступная для атаки и для движения")]
        public Material MoveAndAttackCell { get; private set; }
    }
}
