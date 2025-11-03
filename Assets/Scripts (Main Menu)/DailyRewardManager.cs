using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Threading;
using Unity.VisualScripting;

public class DailyRewardManager : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [Header("Daily Reward Panel")]
    [SerializeField] private Image dailyRewardPanel;
    [Header("Reward Buttons (Day 1 → Day 7)")]
    [SerializeField] private List<Button> collectRewardButtons;
    [SerializeField] private List<Image> selectionImage;
    [SerializeField] private Button claimButton;
    [Header("Message Panel")]
    [SerializeField] private Image messagePanel;
    [SerializeField] private Image loadingRewardPanel;
    //[SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Text rewardMessageText;

    private const string FirstOpenKey = "FirstOpenDate";
    private const string LastClaimedKey = "LastClaimedDate";

    public const string IS_REWARD_COLLECTED_KEY = "RewardCollected";

    private const string CurrentRewardDayKey = "CurrentRewardDay";
    private const string WeekCompletedKey = "WeekCompleted"; // 0 = not completed, 1 = completed (waiting for next week)

    private DateTime currentInternetDate;

    public bool rewardChecked;

    private void Start()
    {
        rewardChecked = true;
        StartCoroutine(InitializeDailyRewards());

    }

    public void CheckReward()
    {
        StartCoroutine(CheckRewardSequence());
    }

    private IEnumerator CheckRewardSequence()
    {
        yield return StartCoroutine(InitializeDailyRewards());
        yield return StartCoroutine(PanelActiveWait());
    }

    IEnumerator PanelActiveWait()
    {
        yield return new WaitForEndOfFrame();
        dailyRewardPanel.gameObject.SetActive(true);
    }
    IEnumerator InitializeDailyRewards()
    {
        // Check if device is online
        if (!IsConnectedToInternet())
        {
            Debug.LogWarning("No internet connection, cannot update rewards.");
            //yield break;
        }

        // Get accurate UTC time from server
        yield return StartCoroutine(GetInternetDate(date =>
        {
            currentInternetDate = date.Date;
        }));

        // If cannot fetch date, fallback to UTC
        if (currentInternetDate == default)
            currentInternetDate = DateTime.UtcNow.Date;

        // Get or create first open date
        DateTime firstOpenDate;
        if (!PlayerPrefs.HasKey(FirstOpenKey))
        {
            firstOpenDate = currentInternetDate;
            PlayerPrefs.SetString(FirstOpenKey, firstOpenDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.SetInt(CurrentRewardDayKey, 0); // Start at day 0
            PlayerPrefs.Save();
        }
        else
        {
            firstOpenDate = DateTime.Parse(PlayerPrefs.GetString(FirstOpenKey));
        }

        // Get current reward day
        int currentRewardDay = PlayerPrefs.GetInt(CurrentRewardDayKey, 0);

        // Normalize currentRewardDay in case stored value is out of range
        if (collectRewardButtons != null && collectRewardButtons.Count > 0)
            currentRewardDay = currentRewardDay % collectRewardButtons.Count;
        else
            currentRewardDay = 0;

        // Get last claimed day
        DateTime lastClaimedDate = PlayerPrefs.HasKey(LastClaimedKey)
            ? DateTime.Parse(PlayerPrefs.GetString(LastClaimedKey))
            : DateTime.MinValue;

        // Only allow claim if at least one day has passed since last claim
        bool canClaim = (currentInternetDate > lastClaimedDate);

        // If a week was completed previously, reset visuals only when the new week's day 1 becomes available
        bool weekCompletedFlag = PlayerPrefs.GetInt(WeekCompletedKey, 0) == 1;
        if (weekCompletedFlag && currentRewardDay == 0 && canClaim)
        {
            // Reset all visuals & interactable states for a fresh week
            for (int i = 0; i < collectRewardButtons.Count; i++)
            {
                var btn = collectRewardButtons[i];

                // reset button main image
                var img = btn.GetComponent<Image>();
                if (img != null) img.color = Color.white;

                // reset all child Images to white (icons, backgrounds, etc.)
                var childImages = btn.GetComponentsInChildren<Image>(true);
                foreach (var ci in childImages)
                {
                    ci.color = Color.white;
                }

                // reset plain Unity UI Text components to white
                var uiTexts = btn.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                foreach (var t in uiTexts)
                {
                    t.color = Color.white;
                }

                // reset TextMeshPro text components to white
                var tmpTexts = btn.GetComponentsInChildren<TextMeshProUGUI>(true);
                foreach (var tt in tmpTexts)
                {
                    tt.color = Color.white;
                }

                btn.interactable = true;

                if (selectionImage != null && i >= 0 && i < selectionImage.Count)
                    selectionImage[i].gameObject.SetActive(false);
            }

            // Clear the week completed flag so reset happens only once
            PlayerPrefs.SetInt(WeekCompletedKey, 0);
            PlayerPrefs.Save();
            weekCompletedFlag = false;
            Debug.Log("Weekly cycle reset: visuals cleared for new week.");
        }

        // Set panel active based on claim status
        dailyRewardPanel.gameObject.SetActive(canClaim);

        // Update claim button interactability
        claimButton.interactable = canClaim;

        // Remove previous listeners to avoid stacking
        claimButton.onClick.RemoveAllListeners();
        if (canClaim)
        {
            // capture normalized currentRewardDay so it's consistent
            int rewardDayForCallback = currentRewardDay;
            claimButton.onClick.AddListener(() =>
            {
                OnClaimReward(rewardDayForCallback);
                DailyRewardMessage(rewardDayForCallback + 1); // +1 for human-readable day (1-based)
            });
        }

        // Update day buttons visually and selectionImage active state
        for (int i = 0; i < collectRewardButtons.Count; i++)
        {
            bool isToday = (i == currentRewardDay);
            bool isGray = (i < currentRewardDay); // already claimed in this cycle

            // manage button color and its children (images/text)
            var btnImage = collectRewardButtons[i].GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = isGray ? Color.gray : Color.white;

            // Apply color to all child Images (icons, backgrounds) and Text (UI text and TMP)
            var childImgs = collectRewardButtons[i].GetComponentsInChildren<Image>(true);
            foreach (var ci in childImgs)
            {
                ci.color = isGray ? Color.gray : Color.white;
            }

            var uiTxts = collectRewardButtons[i].GetComponentsInChildren<Text>(true);
            foreach (var t in uiTxts)
            {
                t.color = isGray ? Color.gray : Color.white;
            }

            var tmpTxts = collectRewardButtons[i].GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tt in tmpTxts)
            {
                tt.color = isGray ? Color.gray : Color.white;
            }

            collectRewardButtons[i].interactable = !isGray;

            // selection image: only active for today's reward when it's claimable
            if (selectionImage != null && i >= 0 && i < selectionImage.Count)
            {
                selectionImage[i].gameObject.SetActive(isToday && canClaim);
            }
        }
        loadingImage.gameObject.SetActive(false);
        loadingRewardPanel.gameObject.SetActive(false);
        Debug.Log($"Day {currentRewardDay + 1} reward available.");
    }

    /// <summary>
    /// Called when player claims today's reward.
    /// </summary>
    private void OnClaimReward(int dayIndex)
    {
        if (dayIndex < 0 || dayIndex >= collectRewardButtons.Count) return;

        collectRewardButtons[dayIndex].interactable = false;
        var img = collectRewardButtons[dayIndex].GetComponent<Image>();
        if (img != null) img.color = Color.gray;

        // also set all child images/text to gray for the claimed button
        var childImages = collectRewardButtons[dayIndex].GetComponentsInChildren<Image>(true);
        foreach (var ci in childImages)
            ci.color = Color.gray;

        var uiTexts = collectRewardButtons[dayIndex].GetComponentsInChildren<UnityEngine.UI.Text>(true);
        foreach (var t in uiTexts)
            t.color = Color.gray;

        var tmpTexts = collectRewardButtons[dayIndex].GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var tt in tmpTexts)
            tt.color = Color.gray;

        // hide selection image for the claimed day
        if (selectionImage != null && dayIndex >= 0 && dayIndex < selectionImage.Count)
            selectionImage[dayIndex].gameObject.SetActive(false);

        claimButton.interactable = false;
        PlayerPrefs.SetString(LastClaimedKey, currentInternetDate.ToString("yyyy-MM-dd"));

        // Wrap to 0 after the last day (e.g., 7 days)
        int nextDay = (dayIndex + 1) % collectRewardButtons.Count;
        PlayerPrefs.SetInt(CurrentRewardDayKey, nextDay);

        // If we wrapped to 0 it means the cycle completed — mark week completed.
        if (nextDay == 0)
            PlayerPrefs.SetInt(WeekCompletedKey, 1);
        else
            PlayerPrefs.SetInt(WeekCompletedKey, 0);

        PlayerPrefs.Save();

        // TODO: Give actual reward here (coins, items, etc.)
    }

    /// <summary>
    /// Get current UTC date from a reliable internet source.
    /// </summary>
    IEnumerator GetInternetDate(Action<DateTime> onDateReceived)
    {
        // https endpoint must be set in inspector replacement - set a working URL here
        // https://timeapi.io/api/Time/current/zone?timeZone=UTC
        UnityWebRequest req = UnityWebRequest.Get("");
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            string utcString = ExtractBetween(json, "\"dateTime\":\"", "\"");
            Debug.Log(utcString);
            if (DateTime.TryParse(utcString, out DateTime utcDateTime))
                onDateReceived?.Invoke(utcDateTime);
            else
                onDateReceived?.Invoke(DateTime.UtcNow);
        }
        else
        {
            Debug.LogWarning("Failed to fetch time from server. Using UTC now.");
            onDateReceived?.Invoke(DateTime.UtcNow);
        }
    }

    /// <summary>
    /// Simple helper to get text between two strings.
    /// </summary>
    private string ExtractBetween(string text, string start, string end)
    {
        int startIndex = text.IndexOf(start) + start.Length;
        int endIndex = text.IndexOf(end, startIndex);
        return text.Substring(startIndex, endIndex - startIndex);
    }

    /// <summary>
    /// Check for internet connectivity.
    /// </summary>
    public bool IsConnectedToInternet()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
    public void HideDailyRewardPanel()
    {
        dailyRewardPanel.gameObject.SetActive(false);
        //DailyRewardMessage();
        messagePanel.gameObject.SetActive(true);
    }
    private void DailyRewardMessage(int day)
    {
        int coins;
        int gems;
        int[] possibleCoins = { 5, 10, 20, 50 };
        int[] possibleGems = { 2, 5, 7, 10 };
        coins = possibleCoins[UnityEngine.Random.Range(0, possibleCoins.Length)];
        gems = possibleGems[UnityEngine.Random.Range(0, possibleGems.Length)];
        int rewardCoins = coins * day;
        int rewardGems = gems * day;
        if(day % 2 != 0)
        {
            rewardMessageText.text = "You have been rewarded " + coins + " x " + day + " = " + rewardCoins + " coins!";
            CurrencyManager.instance.AddCoins(rewardCoins);
           // MainMenuManager.instance.UpdateCurrency(rewardCoins, 0);
        }
        else
        {
            rewardMessageText.text = "You have been rewarded " + gems + " x " + day + " = " + rewardGems + " gems!";
            CurrencyManager.instance.AddGems(rewardGems);
           // MainMenuManager.instance.UpdateCurrency(0, rewardGems);
        }
    }
}
