using UnityEngine;

public class MaquisBehaviour : MonoBehaviour
{
    [SerializeField] private Location[] locations;
    [SerializeField] private Path[] paths;
    [SerializeField] private DifficultyLevel difficultyLevel;

    private void Start()
    {
        Maquis.StartGame(locations, paths, difficultyLevel);
    }
}
