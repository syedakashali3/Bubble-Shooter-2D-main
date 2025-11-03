using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CoinCollectEffect : MonoBehaviour
{
    [Header("References")]
    public RectTransform targetUI;        // Target (coin counter)
    public GameObject coinPrefab;         // UI coin prefab (Image or UI element)
    public RectTransform spawnParent;     // Usually your Canvas

    [Header("Settings")]
    public int coinCount = 10;
    public float spawnRadius = 100f;
    public float flyDuration = 0.8f;
    public float delayBetweenCoins = 0.05f;

    public void PlayEffect()
    {
        if (coinPrefab == null || targetUI == null || spawnParent == null)
        {
            Debug.LogWarning("CoinCollectEffect: Missing references!");
            return;
        }

        // ✅ Spawn from the center of the parent RectTransform (canvas)
        Vector2 screenCenter = spawnParent.rect.center;

        for (int i = 0; i < coinCount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, spawnParent);
            RectTransform coinRect = coin.GetComponent<RectTransform>();

            // Start from center with small random offset
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            coinRect.anchoredPosition = screenCenter + randomOffset;
            coinRect.localScale = Vector3.zero;

            // Animate: scale up → fly to target → destroy
            Sequence seq = DOTween.Sequence();
            seq.Append(coinRect.DOScale(1f, 0.2f).SetEase(Ease.OutBack));
            seq.AppendInterval(i * delayBetweenCoins);
            seq.Append(coinRect.DOMove(targetUI.position, flyDuration).SetEase(Ease.InOutQuad));
            seq.Join(coinRect.DOScale(0.3f, flyDuration));
            seq.OnComplete(() => Destroy(coin));
        }
    }
}
