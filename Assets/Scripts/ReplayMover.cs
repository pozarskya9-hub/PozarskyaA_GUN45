using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
			////todo comment: зачем нужны эти проверки?
			// Ответ: TryGetComponent проверяет, что на объекте вообще есть PositionSaver, а Records.Count == 0 - что в нём есть записанные
			// точки. Без этих проверок Update упал бы с NullReferenceException (нет компонента) или ArgumentOutOfRangeException
			// на Records[_index] (список пуст). Если первое условие истинно, то второе не вычисляется (_save был бы null).
			if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				// Ответ: чтобы Unity перестал вызывать Update, который работал бы с некорректными данными и спамил ошибками каждый кадр.
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)?
			// Ответ: проверяет, наступило ли уже время текущей целевой точки. Если да, мы её достигли: она становится предыдущей
			// (_prev), а целью становится следующая точка (_index++). То есть происходит переход к следующему отрезку пути.
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				// Ответ: проверяет, не закончились ли точки. Если _index вышел за пределы списка, то в следующем кадре
				// Records[_index] вызвал бы ArgumentOutOfRangeException. Поэтому воспроизведение завершается: компонент выключается.
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			// Ответ: это прогресс движения по текущему отрезку от 0 до 1: сколько времени прошло с момента предыдущей точки,
			// делённое на длительность всего отрезка (разница времён двух точек). Дальше delta используется как параметр t
			// в Vector3.Lerp, чтобы понять, в какой части пути между точками должен находиться объект.
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
			//todo comment: Зачем нужна эта проверка?
			// Ответ: если время двух точек совпадает, то получается деление 0 на 0, то есть NaN. Если передать NaN в Lerp, позиция
			// объекта станет NaN, и он "пропадёт" со сцены. Поэтому в этом случае берём delta = 0 (остаёмся в предыдущей точке).
			if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			// Ответ: Vector3.Lerp(a, b, t) - линейная интерполяция между двумя векторами: возвращает a + (b - a) * t, считая
			// покомпонентно по x, y, z, причём t ограничивается диапазоном от 0 до 1 (выйти за пределы отрезка нельзя).
			// При t = 0 результат равен _prev.Position (предыдущая точка), при t = 1 - curr.Position (целевая точка), а при
			// промежуточных значениях - точка на прямой между ними (например, при 0.5 ровно посередине). Полученный вектор
			// записывается в transform.position, то есть объект ставится в эту мировую позицию. Так как строка выполняется
			// каждый кадр и delta постепенно растёт, объект плавно движется от предыдущей точки к текущей за то же время,
			// за какое это было при записи.
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}
