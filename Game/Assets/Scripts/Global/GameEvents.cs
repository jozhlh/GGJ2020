using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameEvents
{
    public static Action OnRoundStart = () => {};
    public static Action OnRoundEnd = () => {};

    public static Action<int> OnDamage = amount => {};

    public static Action<Cosmonaut> OnPlayerDied = player => {};
}
