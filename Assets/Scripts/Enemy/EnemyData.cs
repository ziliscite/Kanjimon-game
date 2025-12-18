using UnityEngine;
using UnityEngine.UI;

public class EnemyData : MonoBehaviour
{
    public EnemyDataSO enemyDataSO;
    public Slider healthSlider;
    
    public int GetAttackDamage()
    {
        return enemyDataSO.attackPower;
    }
}
