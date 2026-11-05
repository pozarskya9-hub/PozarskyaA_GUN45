using System.Collections;
using UnityEngine;

namespace Netologia.Homework
{
	[RequireComponent(typeof(Rigidbody))]
	public class Rotator : MonoBehaviour
	{
		// Скорость вращения в градусах в секунду по каждой оси.
		[SerializeField]
		private Vector3 _rotate;

		private IEnumerator Start()
		{
			var body = GetComponent<Rigidbody>();

			// Ждём первый физический кадр, чтобы не крутить тело вне физического цикла.
			yield return new WaitForFixedUpdate();

			while (true)
			{
				// Градусы/сек * секунды кадра = градусы за один физический шаг.
				var step = Quaternion.Euler(_rotate * Time.fixedDeltaTime);
				body.MoveRotation(body.rotation * step);
				yield return new WaitForFixedUpdate();
			}
		}
	}
}
