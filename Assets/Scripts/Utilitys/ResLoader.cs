using UnityEngine;

public class ResLoader
{
    public static T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public static T[] LoadAll<T>(string path) where T:Object
    {
        return Resources.LoadAll<T>(path);
    }

    public static Sprite LoadSprite(string spriteName)
    {
        return Load<Sprite>(PathConfig.SpritePath + spriteName);
    }

    public static Sprite[] LoadSpriteAnimation(string spriteName)
    {
        return LoadAll<Sprite>(PathConfig.AnimationPath + spriteName);
    }
}
