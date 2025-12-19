using UnityEngine;
using TMPro;
using DG.Tweening;
using System; // [Import this for Action]

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

    // Added 'Action onComplete' parameter
    public void ShowYourTurn(Action onComplete = null)
    {
        Play("Your Turn", onComplete);
    }

    // Added 'Action onComplete' parameter
    public void ShowEnemyTurn(Action onComplete = null)
    {
        Play("Enemy Turn", onComplete);
    }

    private void Play(string message, Action onComplete)
    {
        DOTween.Kill(textRect);
        DOTween.Kill(turnText);

        turnText.text = message;

        // Reset positions
        textRect.anchoredPosition = centerPos + Vector2.left * offscreenX;
        turnText.alpha = 0f;

        Sequence seq = DOTween.Sequence();

        // Slide In
        seq.Append(textRect.DOAnchorPos(centerPos, moveDuration).SetEase(Ease.OutCubic));
        seq.Join(turnText.DOFade(1f, moveDuration));

        // Wait
        seq.AppendInterval(stayDuration);

        // Slide Out
        seq.Append(textRect.DOAnchorPos(centerPos + Vector2.right * offscreenX, moveDuration).SetEase(Ease.InCubic));
        seq.Join(turnText.DOFade(0f, moveDuration));

        seq.OnComplete(() => {
            onComplete?.Invoke();
        });
    }
}