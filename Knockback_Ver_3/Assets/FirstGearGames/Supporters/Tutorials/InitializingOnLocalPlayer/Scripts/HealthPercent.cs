using UnityEngine;

public class HealthPercent : MonoBehaviour
{

    /// <summary>
    /// 
    /// </summary>
    [Tooltip("Current health percent of this unit.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _currentPercent;
    /// <summary>
    /// Current health percent of this unit.
    /// </summary>
    public float CurrentPercent { get { return _currentPercent; } }

}
