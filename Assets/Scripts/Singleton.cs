public class Singleton<T> where T:new()
{
    static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }
}
