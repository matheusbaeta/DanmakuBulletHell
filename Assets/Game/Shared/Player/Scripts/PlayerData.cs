using System;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int MaxLifes;
    public int RemainingLifes;
    public decimal Damage;
    public bool isDead;

    public PlayerData(int maxLifes, decimal damage)
    {
        MaxLifes = maxLifes;
        RemainingLifes = maxLifes;
        Damage = damage;
        isDead = false;
    }

    public void DecreaseRemaningLifes()
    {
        if(RemainingLifes > 0 && !isDead) RemainingLifes--;
        else isDead = true;
    }
}
