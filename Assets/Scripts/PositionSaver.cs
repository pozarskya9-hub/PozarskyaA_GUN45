using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
		[Serializable]
		public struct Data
		{
			public Vector3 Position;
			public float Time;
		}

		[SerializeField, ReadOnly]
		[Tooltip("Чтобы заполнить это поле, откройте контекстное меню компонента (три точки / ПКМ по заголовку) и выберите команду \"Create File\".")]
		private TextAsset _json;

		[field: SerializeField, HideInInspector]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			// Ответ: метод продолжил бы выполняться и дошёл бы до _json.text при _json == null -> NullReferenceException
			// (ошибка в консоли). Объект мы уже отключили, но сам Awake без return всё равно дошёл бы до конца.
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");
				return;
			}

			// FromJsonOverwrite перезаписывает и поле _json (оно теперь сериализуемое), поэтому запоминаем ссылку и возвращаем её обратно,
			// иначе в OnDestroy можно потерять ассет, в который нужно сохранять данные.
			var asset = _json;
			if (!string.IsNullOrWhiteSpace(_json.text))
				JsonUtility.FromJsonOverwrite(_json.text, this);
			_json = asset;

			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			// Ответ: если файл пустой (или в нём нет записей), список Records остаётся null. Без проверки EditorMover и ReplayMover
			// упали бы с NullReferenceException при обращении к Records (Clear, Add, Count). Здесь мы создаём пустой список.
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
			//todo comment: Зачем нужны эти проверки (что они позволляют избежать)?
			// Ответ: Gizmos рисуются и вне плеймода, когда Awake ещё не вызывался и Records == null (NullReferenceException).
			// Проверка Count == 0 защищает от ArgumentOutOfRangeException на строке data[0] при пустом списке.
			if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			// Ответ: нулевая точка уже обработана до цикла (нарисована сфера и она записана в prev). Линию можно провести
			// только между двумя точками, а у нулевой точки предыдущей нет. Если начать с 0, мы бы нарисовали вторую такую же
			// сферу и "линию" из точки в саму себя.
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}

#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			// Ответ: Application.dataPath - путь к папке Assets проекта; Path.Combine склеивает его с именем "Path.txt" с правильным
			// разделителем для ОС; File.Create создаёт на диске пустой файл Path.txt (или обнуляет существующий) и возвращает
			// открытый поток FileStream для этого файла.
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав)
			// Ответ: закрывает поток и освобождает файл. Пока поток открыт, файл занят нашим процессом, и Unity (AssetDatabase.Refresh)
			// или последующая запись в него могут выдать ошибку "файл используется другим процессом".
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				// Ответ: FindAssets вернул ВСЕ текстовые ассеты проекта, а нужен только наш. asset != null защищает от случая, когда
				// ассет не удалось загрузить (иначе будет NullReferenceException на asset.name), а asset.name == "Path" отбирает
				// именно файл Path.txt (name - имя без расширения) среди остальных текстовых файлов.
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					// Ответ: нужный ассет уже найден и записан в _json. Продолжать перебор нет смысла: это лишняя работа, и можно было бы
					// случайно перезаписать _json другим ассетом с таким же именем.
					return;
				}
			}
		}

		private void OnDestroy()
		{
			// Сохраняем текущие значения (в том числе Records) в текстовый ассет из поля _json.
			if (_json == null) return;

			var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
			File.WriteAllText(path, JsonUtility.ToJson(this));
			UnityEditor.AssetDatabase.Refresh();
		}
#endif
	}
}
