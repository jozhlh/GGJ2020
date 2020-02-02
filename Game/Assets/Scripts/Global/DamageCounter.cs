using UnityEngine;
using UnityEngine.SceneManagement;
public class DamageCounter : MonoBehaviour
{
    public int Health { get; private set; } = MaxHealth;
    public float TimeLeft { get; private set; } = MaxTime;

    public const float MaxTime = 240;

    public const int MaxHealth = 10000;

    private MultiplayerManager multiplayer;

    private void Awake()
    {
        GameEvents.OnRoundStart += OnRoundStart;
        GameEvents.OnDamage += OnDamage;

        multiplayer = FindObjectOfType<MultiplayerManager>();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;

        if (TimeLeft <= 0)
        {
            GameEvents.OnRoundEnd();
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnRoundStart -= OnRoundStart;
        GameEvents.OnDamage -= OnDamage;
    }

    private void OnRoundStart()
    {
        Health = MaxHealth;
        TimeLeft = MaxTime;
    }

    private void OnDamage(int damage)
    {
        // don't let damage happen before game begins
        if (!GameEvents.InGame)
        {
            return;
        }

        Health = Mathf.Max(0, Health - damage);
        if (Health == 0)
        {
            GameEvents.OnRoundEnd();
            SceneManager.LoadScene( SceneManager.GetActiveScene().name );
        }
    }
}