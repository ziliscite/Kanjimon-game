using System;

[Serializable]
public class SaveData
{
    // Stats
    public int playerHP;
    public int potionsLeft;
    public int expPlayer;
    public int playerLevel;

    // Location
    public float posX;
    public float posY;
    public float posZ;
    public int currentFloor; // Ganti lastFloor jadi currentFloor biar jelas
    
    // Meta
    public int enemyBattledID; // ID musuh jika save dilakukan saat battle (opsional)
    public string sceneName;
    public string timestamp;
}