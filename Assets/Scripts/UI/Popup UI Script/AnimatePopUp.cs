using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private RectTransform textRect;

    [Header("Animation")]
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float stayDuration = 0.7f;
    [SerializeField] private float offscreenX = 800f;

    private Vector2 centerPos;

    void Awake()
    {
        centerPos = textRect.anchoredPosition;
        turnText.alpha = 0f;
    }

    public void ShowYourTurn()
    {
        Play("Your Turn");
    }

    public void ShowEnemyTurn()
    {
        Play("Enemy Turn");
    }

    private void Play(string message)
    {
        DOTween.Kill(textRect);
        DOTween.Kill(turnText);

        turnText.text = message;

        // Start dari kiri
        textRect.anchoredPosition = centerPos + Vector2.left * offscreenX;
        turnText.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        seq.Append(textRect.DOAnchorPos(centerPos, moveDuration).SetEase(Ease.OutCubic));
        seq.Join(turnText.DOFade(1f, moveDuration));

        seq.AppendInterval(stayDuration);

        seq.Append(textRect.DOAnchorPos(centerPos + Vector2.right * offscreenX, moveDuration).SetEase(Ease.InCubic));
        seq.Join(turnText.DOFade(0f, moveDuration));
    }
}
