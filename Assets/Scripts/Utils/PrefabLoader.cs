using UnityEngine;
using System;
using System.Collections;
using System.IO;

namespace Game
{
	public class PrefabLoader : MonoBehaviour
	{
		public static PrefabLoader Create ()
		{
			var go = new GameObject ("PrefabLoader");
			GameObject.DontDestroyOnLoad (go);

			var loader = go.AddComponent<PrefabLoader> ();
			return loader;
		}


		private enum State
		{
			Idle,
			Loading,
			Completed
		}

		private State _currentState = State.Idle;
		private string _path = null;
		private ResourceRequest _request = null;
		private Action<UnityEngine.GameObject> _completed = null;
		private GameObject _prefab = null;

		public GameObject Load (string path)
		{
			if (_currentState != State.Idle)
				throw new InvalidOperationException ();

			_currentState = State.Loading;
			_path = path;

			GameObject.Destroy (gameObject);
			_prefab = (GameObject)Resources.Load (_path, typeof(GameObject));

			_currentState = State.Completed;

			return _prefab;
		}

		public void LoadAsync (string path, Action<UnityEngine.GameObject> completed)
		{
			if (_currentState != State.Idle)
				throw new InvalidOperationException ();

			_currentState = State.Loading;
			_path = path;
			_completed = completed;

			_request = Resources.LoadAsync (path, typeof(GameObject));
		}

		void Update ()
		{
			if (_currentState == State.Loading) {
				if (_request.isDone) {
					_currentState = State.Completed;

					GameObject.Destroy (gameObject);

					_prefab = (GameObject)_request.asset;
					_completed (_prefab);
				}
			}
		}

		public void Unload ()
		{
			if (_prefab != null) {
				Resources.UnloadAsset (_prefab);
				_prefab = null;

				Resources.UnloadUnusedAssets ();
			}
		}
	}
}
