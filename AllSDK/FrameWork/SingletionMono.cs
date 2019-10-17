
using UnityEngine;
public class SingletionMono<T> : MonoBehaviour where T : Component
{

	public static Transform parent;
	static T instance;
	public static T Instance
	{
		get
		{
			if (parent == null)
			{
				parent = FindObjectOfType<SDKController>().transform;
			}
			if (instance == null)
				instance = FindObjectOfType<T>();
			if (instance == null)
			{
				GameObject g = new GameObject(typeof(T).Name);
				instance = g.AddComponent<T>();
				instance.transform.parent = parent;
				DontDestroyOnLoad(instance);
			}
			return instance;
		}

	}

	private void Awake()
	{
		if (instance == null)
			instance = this as T;
		else
			if (instance != this)
			Destroy(gameObject);

	}
}
