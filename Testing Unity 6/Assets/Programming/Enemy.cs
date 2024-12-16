using UnityEngine;

public class Enemy : MonoBehaviour
{

    public int health;

    internal void TakeDamage(int damageToInflict)
    {
        health -= damageToInflict;
    }


}
