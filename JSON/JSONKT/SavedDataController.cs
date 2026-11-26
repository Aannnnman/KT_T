using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class SavedDataController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    private string _filePath;
    private float _lastSavedVolume;

    private void Start()
    {
        _filePath = Path.Combine(Application.persistentDataPath, "SoundData.txt");

        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            SoundData savedData = JsonConvert.DeserializeObject<SoundData>(json);

            if (savedData != null)
            {
                _audioSource.volume = Mathf.Clamp01(savedData.Volume);
            }
        }

        _lastSavedVolume = _audioSource.volume;

        if (!File.Exists(_filePath))
        {
            SaveVolume();
        }
    }

    private void Update()
    {
        if (!Mathf.Approximately(_audioSource.volume, _lastSavedVolume))
        {
            SaveVolume();
        }
    }

    private void SaveVolume()
    {
        SoundData data = new SoundData(_audioSource.volume);

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        File.WriteAllText(_filePath, json);

        _lastSavedVolume = _audioSource.volume;
    }
}
