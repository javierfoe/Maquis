using TMPro;
using UnityEngine;

public class MaquisBehaviour : MonoBehaviour
{
    private static Location _selectedLocation;
    private static int _maxActions;
    public static bool PlaceAgents = true;

    public static Location SelectedLocation
    {
        get => _selectedLocation;
        set
        {
            _selectedLocation = value;
            _maxActions = _selectedLocation != null ? _selectedLocation.Actions.Count : 0;
        }
    }

    [SerializeField] private Location[] locations;
    [SerializeField] private Path[] paths;
    [SerializeField] private TMP_Text food, money, weapons, medicine, information, explosives, poison, fakeId, agents, morale, soldiers, days;
    [SerializeField] private DifficultyLevel difficultyLevel;

    private int _selectedAction;

    private void UpdateResourceTexts(ResourcesDto resourcesDto)
    {
        food.text = resourcesDto.Food.ToString();
        money.text = resourcesDto.Money.ToString();
        weapons.text = resourcesDto.Weapons.ToString();
        medicine.text = resourcesDto.Medicine.ToString();
        information.text = resourcesDto.Information.ToString();
        explosives.text = resourcesDto.Explosives.ToString();
        poison.text = resourcesDto.Poison.ToString();
        fakeId.text = resourcesDto.FakeId.ToString();
    }

    private void Start()
    {
        Maquis.StartGame(locations, paths, difficultyLevel);
        Maquis.ResourcesEvent.AddListener(UpdateResourceTexts);
    }

    private void Update()
    {
        if (_maxActions != 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _selectedAction -= _selectedAction > 0 ? 1 : 0;
                Debug.Log(_selectedAction);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _selectedAction += _selectedAction < _maxActions - 1 ? 1 : 0;
                Debug.Log(_selectedAction);
            }

            if (Input.GetKeyDown(KeyCode.Return) && SelectedLocation)
            {
                SelectedLocation.Actions[_selectedAction].PerformAction();
                _selectedAction = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlaceAgents = true;
            if (!Maquis.NextDay())
            {
                Debug.Log("Game Over");
            }
        }
    }
}