using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    bool global;
    static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != GetComponent<T>())
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = GetComponent<T>();
        if (global)
            DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
}
