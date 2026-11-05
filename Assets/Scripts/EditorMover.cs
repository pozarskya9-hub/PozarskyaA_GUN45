using UnityEngine;

namespace DefaultNamespace
{

	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

		//todo comment: Что произойдёт, если _delay > _duration?
		// Ответ: таймер задержки не успеет сработать второй раз до конца _duration. Запись происходит сразу в первом кадре
		// (_currentDelay = 0), а следующая должна была бы пройти только через _delay секунд, но _duration к тому моменту уже
		// закончится. В итоге запишется всего одна точка (стартовая), и воспроизводить будет нечего.
		[SerializeField, Range(0.2f, 1f)] private float _delay = 0.5f;
		[SerializeField, Min(0.2f)] private float _duration = 5f;

		private void Start()
		{
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			// Ответ: Start вызывается один раз, а Update - каждый кадр. GetComponent - не самая дешёвая операция, поэтому
			// компонент находят один раз и кэшируют в поле _save, а потом просто используют. Очистка Records тоже нужна один раз.
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();

			// _duration должен быть больше _delay, иначе запишется меньше двух точек.
			if (_duration <= _delay)
				_duration = _delay * 5f;
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}

			//todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			// Ответ: _delay - это настройка (интервал между записями), и она должна оставаться неизменной. Если её уменьшать,
			// то после первого срабатывания интервал пропадёт, и взять значение для перезапуска таймера будет неоткуда.
			// Поэтому для обратного отсчёта используется отдельное поле _currentDelay, которое каждый раз заново получает _delay.
			// _duration отсчитывается один раз до нуля, поэтому его можно уменьшать напрямую.
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					// Ответ: чтобы знать, в какой момент времени объект был в этой точке. ReplayMover по этому времени понимает,
					// когда нужно оказаться в каждой точке, и воспроизводит движение с той же скоростью, что и при записи.
					Time = Time.time,
				});
			}
		}
	}
}
