using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class mExternalAudioLoader : MonoBehaviour
{
    public string externalAudioPath = "UserAudios"; // 相对路径
    public List<AudioClip> loadedClips = new();

    public void LoadExternalAudios()
    {
        StartCoroutine(LoadAudiosCoroutine());
    }

    IEnumerator LoadAudiosCoroutine()
    {
        loadedClips.Clear();

        string fullPath = Path.Combine(Application.dataPath, "..", externalAudioPath);
        if (!Directory.Exists(fullPath))
        {
            Debug.LogWarning("路径不存在: " + fullPath);
            yield break;
        }

        string[] supportedExtensions = new[] { "*.wav", "*.ogg", "*.mp3" };
        List<string> audioFiles = new();

        foreach (var ext in supportedExtensions)
        {
            audioFiles.AddRange(Directory.GetFiles(fullPath, ext));
        }

        foreach (string filePath in audioFiles)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            AudioType audioType = AudioType.UNKNOWN;

            switch (extension)
            {
                case ".wav": audioType = AudioType.WAV; break;
                case ".ogg": audioType = AudioType.OGGVORBIS; break;
                case ".mp3": audioType = AudioType.MPEG; break;
            }

            if (audioType == AudioType.UNKNOWN)
                continue;

            string url = "file:///" + filePath.Replace("\\", "/");

            using UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                clip.name = Path.GetFileNameWithoutExtension(filePath); // 使用文件名作为 clip 名
                loadedClips.Add(clip);
                Debug.Log("Loaded clip: " + clip.name);
            }
            else
            {
                Debug.LogError("Failed to load: " + filePath + "\n" + www.error);
            }
        }

        Debug.Log($"加载完成，共 {loadedClips.Count} 个音频文件");
    }
}
