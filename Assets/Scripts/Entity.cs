using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityType
{
    NPC,
    ZOMBIE,
}

public abstract class Entity : MonoBehaviour
{
    //General attributes every entity has
    //For this small game, I decided to go for the OOP way for creating entities, instead of the ECS (Entity component system) data oriented way
    public int health;
    public int strength;
    public int level;

    public EntityType entityType;
}
