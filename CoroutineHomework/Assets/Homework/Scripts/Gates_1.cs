using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Collider))]
	public class Gates : MonoBehaviour
	{
		private int _score;

		// Вызывается, когда компонент добавляют в редакторе (или жмут Reset):
		// сразу делаем коллайдер триггером.
		private void Reset()
		{
			GetComponent<Collider>().isTrigger = true;
		}

		private void OnTriggerEnter(Collider other)
		{
			// Коллайдер может лежать на дочернем объекте мяча, поэтому ищем Ball вверх по иерархии.
			var ball = other.GetComponentInParent<Ball>();
			if (ball == null) return;

			Destroy(ball.gameObject);
			_score++;
			Debug.Log($"Score: {_score}");
		}
	}
}
