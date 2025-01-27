using Godot;
using System;
using Godot.Collections;
// ReSharper disable All

public class PlayerStats : Node
{
    [Export] private NodePath playerPath;
    private Player player;

    [Export] private NodePath collisionPath;
    private Area2D collisionArea;

    [Export] private Timer invencibillityTimer;

    public bool shielding = false;

    private int baseHealth = 20;
    private int baseMana = 10;
    private int baseAttack = 2;
    private int baseAttackMagic = 4;
    private int baseDefence = 2;

    private int bonusHealth = 0;
    private int bonusMana = 0;
    private int bonusAttack = 0;
    private int bonusAttackMagic = 0;
    private int bonusDefence = 0;

    private int currentHealth;
    private int currentMana;

    private int maxHealth;
    private int maxMana;

    private int currentXP = 0;

    private int level = 1;

    private Dictionary<string, int> levelDict = new Dictionary<string, int>
    {
        {"1", 26}, {"2", 36}, {"3", 56}, {"4", 78},
        {"5", 92}, {"6", 120}, {"7", 154}, {"8", 192},
        {"9", 202}, {"10", 268},
    };

    public override void _Ready()
    {
        player = GetNode<Player>(playerPath);
        collisionArea = GetNode<Area2D>(collisionPath);
        invencibillityTimer = GetNode<Timer>("InvencibilityTimer");

        currentMana = baseMana + bonusMana;
        maxMana = currentMana;

        currentHealth = baseHealth + bonusHealth;
        maxHealth = currentHealth;
    }
    
    public void UpdateExp(int value) // Update the Xp when the character level up
    {
        currentXP += value;
        if (currentXP >= levelDict[level.ToString()] && level < 9)
        {
            int leftover = currentXP - levelDict[level.ToString()];
            currentXP = leftover;
            OnLevelUp();
            level += 1;
        } else if (currentXP >= levelDict[level.ToString()] && level == 10)
        {
            currentXP = levelDict[level.ToString()];
        }
    }

    public void OnLevelUp() // Increases the current mana and health
    {
        currentMana = baseMana + bonusMana;
        currentHealth = baseHealth + bonusHealth;
    }

    public void UpdateHealth(string type, int value) // Update the health when the player level up or get hits or die
    {
        switch (type)
        {
            case "Increase":
                currentHealth = Math.Min(currentHealth + value, maxHealth);
                break;

            case "Decrease":
                int previousHealth = currentHealth; // Armazena o valor antes de calcular o dano
                ApplyDamage(value);

                // Verifica se o jogador morreu
                if (currentHealth <= 0)
                {
                    currentHealth = 0; // Garante que não fique negativo
                    if (previousHealth > 0) // Evita "matar" repetidamente
                    {
                        player.dead = true;
                        GD.Print("O jogador morreu!");
                    }
                }
                else
                {
                    player.onHit = true;
                    player.Attacking = false;
                }
                break;
        }
    }

    private void ApplyDamage(int value) // Checks if the player is defending when he is under attack
    {
        if (shielding)
        {
            int damage = Math.Max(0, value - (baseDefence + bonusDefence));
            currentHealth -= damage;
        }
        else
        {
            currentHealth = Math.Max(currentHealth - value, 0);
        }
    }

    public void UpdateMana(string type, int value) // Update the mana when the player level up or when he uses magic attacks
    {
        switch (type)
        {
            case "Increase":
                currentMana = Math.Min(currentMana + value, maxMana);
                break;

            case "Decrease":
                currentMana = Math.Max(currentMana - value, 0);
                break;
        }
    }

    public override void _Process(float delta)
    {
        // Apenas para testes
        if (Input.IsActionJustPressed("ui_select"))
        {
            UpdateHealth("Decrease", 5);
        }
    }

    private void On_CollisionArea_Entered(object area) // Detects if the Enemy is attacking the player
    {
        if (area.Name == "EnemyAttackArea")
        {
            UpdateHealth("Decrease", area.Damage);
            collisionArea.SetDeferred("monitoring", false);
            invencibillityTimer.Start(area.invencibillityTimer);
        }
    }

    private void On_InvencibilityTimer_timeout() // Invencibility timer
    {
        collisionArea.SetDeferred("monitoring", false);
    }
}