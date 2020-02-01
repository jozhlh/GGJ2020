using UnityEngine;

public class DamageCounter : MonoBehaviour
{
    public int Health { get; private set; } = MaxHealth;
    public const int MaxHealth = 1000;

    private MultiplayerManager multiplayer;

    private void Awake()
    {
        GameEvents.OnRoundStart += OnRoundStart;
        GameEvents.OnDamage += OnDamage;

        multiplayer = FindObjectOfType<MultiplayerManager>();
    }

    private void OnRoundStart()
    {
        Health = MaxHealth;
    }

    private void OnDamage(int damage)
    {
        // don't let damage happen before game begins
        if (multiplayer && multiplayer.ActivePlayerCount == 0)
        {
            return;
        }

        Health = Mathf.Max(0, Health - damage);
        if (Health == 0)
        {
            GameEvents.OnRoundEnd();
        }
    }
}