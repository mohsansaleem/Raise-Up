using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Game.Managers
{
	public class PrefabsManager
	{
		private static PrefabsManager _instance;
		public static PrefabsManager Instance {
			get {
				if (_instance == null)
					_instance = new PrefabsManager ();

				return _instance;
			}
		}


		private PrefabsManager ()
		{
		}


		private class LoadedEntry
		{
			public enum State
			{
				Idle,
				Loading,
				Completed
			}

			private State _currentState = State.Idle;
			private string _path = null;
			private GameObject _prefab = null;
			private PrefabLoader _loader = null;
			private List<Action<GameObject>> _callbacks = new List<Action<GameObject>> ();

			public string Path { get { return _path; } }
			public GameObject Prefab { get { return _prefab; } }
			public State CurrentState { get { return _currentState; } }


			public LoadedEntry (string path)
			{
				_path = path;
			}

			public GameObject Get ()
			{
				if (_currentState == State.Completed)
					return _prefab;

				if (_currentState == State.Loading)
					throw new InvalidOperationException ();

				_currentState = State.Loading;

				_loader = PrefabLoader.Create ();
				_prefab = _loader.Load (_path);

				_currentState = State.Completed;

				return _prefab;
			}

			public void GetAsync (Action<GameObject> callback)
			{
				if (_currentState == State.Completed) {
					callback (_prefab);
					return;
				}

				_callbacks.Add (callback);
				if (_currentState == State.Loading)
					return;

				_currentState = State.Loading;

				_loader = PrefabLoader.Create ();
				_loader.LoadAsync (_path, OnCompleted);
			}

			void OnCompleted (GameObject prefab)
			{
				_prefab = prefab;
				_currentState = State.Completed;

				foreach (var callback in _callbacks)
					callback (_prefab);

				_callbacks.Clear ();
			}

			public void Unload ()
			{
				_prefab = null;

				if (_loader != null) {
					_loader.Unload ();
					_loader = null;
				}
			}
		}



		private List<LoadedEntry> _entries = new List<LoadedEntry> ();


		public GameObject Load (string path)
		{
			var entry = _entries.Find (x => x.Path == path);
			if (entry == null) {
				entry = new LoadedEntry (path);
				_entries.Add (entry);
			}

			return entry.Get ();
		}

		public void LoadAsync (string path, Action<GameObject> callback)
		{
			var entry = _entries.Find (x => x.Path == path);
			if (entry == null) {
				entry = new LoadedEntry (path);
				_entries.Add (entry);
			}

			entry.GetAsync (callback);
		}

		public void Unload (GameObject prefab)
		{
			var index = _entries.FindIndex (x => x.Prefab == prefab);
			if (index >= 0) {
				var entry = _entries [index];
				_entries.RemoveAt (index);
				entry.Unload ();
			}
		}



		private Dictionary<GameObject, GameObject> _instantiatedObjects = new Dictionary<GameObject, GameObject> ();

		public GameObject Instantiate (GameObject prefab)
		{
			var go = (GameObject)UnityEngine.Object.Instantiate (prefab);
			_instantiatedObjects.Add (go, prefab);
			return go;
		}

		public T Instantiate<T> (T prefab) where T : Component
		{
			return Instantiate (prefab.gameObject).GetComponent<T> ();
		}

		public bool Destroy (GameObject obj)
		{
			GameObject prefab = null;
			if (_instantiatedObjects.TryGetValue (obj, out prefab)) {
				UnityEngine.Object.Destroy (obj);

				_instantiatedObjects.Remove (obj);
				if (_instantiatedObjects.Values.Count (x => x == prefab) <= 0)
					Unload (prefab);

				return true;
			}

			return false;
		}

		public bool Destroy<T> (T obj) where T : Component
		{
			return Destroy (obj.gameObject);
		}

		public void NotifyOnDestroy (GameObject obj)
		{
			GameObject prefab = null;
			if (_instantiatedObjects.TryGetValue (obj, out prefab)) {
				_instantiatedObjects.Remove (obj);
				if (_instantiatedObjects.Values.Count (x => x == prefab) <= 0)
					Unload (prefab);
			}
		}

		public void NotifyOnDestroy<T> (T obj) where T : Component
		{
			NotifyOnDestroy (obj.gameObject);
		}
	}
}
