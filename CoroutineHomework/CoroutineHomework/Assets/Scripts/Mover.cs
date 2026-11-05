using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Rigidbody))]
	public class Mover : MonoBehaviour
	{
		// Точки задаются как смещение от позиции объекта в момент старта сцены,
		// поэтому уровень можно двигать/копировать, и путь поедет вместе с объектом.
		[SerializeField]
		private Vector3 _start;
		[SerializeField]
		private Vector3 _end;
		[SerializeField, Min(0.01f)]
		private float _speed = 1f;
		[SerializeField, Min(0f)]
		private float _delay = 1f;

		private Rigidbody _body;
		private Vector3 _origin;

		private IEnumerator Start()
		{
			_body = GetComponent<Rigidbody>();
			_origin = _body.position;

			// Первый физический кадр: дальше двигаем тело только в физическом цикле.
			yield return new WaitForFixedUpdate();

			var from = _origin + _start;
			var to = _origin + _end;

			while (true)
			{
				// Один и тот же метод для прямого и обратного пути.
				yield return Move(from, to);
				yield return new WaitForSeconds(_delay);
				(from, to) = (to, from);
			}
		}

		// Двигает тело от точки from к точке to с постоянной скоростью _speed.
		private IEnumerator Move(Vector3 from, Vector3 to)
		{
			var position = from;
			_body.MovePosition(position);

			while (position != to)
			{
				yield return new WaitForFixedUpdate();
				position = Vector3.MoveTowards(position, to, _speed * Time.fixedDeltaTime);
				_body.MovePosition(position);
			}
		}

		// Рисуем путь в окне Scene, чтобы левел-дизайнеру было удобно настраивать.
		private void OnDrawGizmos()
		{
			// В режиме игры объект уже уехал, поэтому берём запомненную стартовую позицию.
			var origin = Application.isPlaying ? _origin : transform.position;
			var start = origin + _start;
			var end = origin + _end;

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(start, 0.3f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(end, 0.3f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawLine(start, end);
		}
	}
}
