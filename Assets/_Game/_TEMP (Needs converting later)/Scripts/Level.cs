using UnityEngine;

[System.Serializable]
public class Level
{
    [field: SerializeField] public string LevelName { get; set; }
    [field: SerializeField] public string SceneName { get; set; }
    [field: SerializeField] public Difficulty Difficulty { get; set; }
}