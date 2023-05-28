using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Location first;
    [SerializeField] private Location second;

    public Location First => first;
    public Location Second => second;

    private bool _isBlocked;

    public void SetIsBlocked(bool blocked)
    {
        _isBlocked = blocked;
    }

    public Location GetOtherEnd(Location input)
    {
        var other = input == first ? second : first;
        if (_isBlocked || other.IsBlocked()) return null;
        return other;
    }

    public void OnValidate()
    {
        if (first == null || second == null)
        {
            Debug.LogError($"Path does not link two locations", gameObject);
            return;
        }
        gameObject.name = $"{first.gameObject.name} - {second.gameObject.name}";
    }
}
