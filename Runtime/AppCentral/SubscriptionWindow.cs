
namespace AppCentral
{
    #region using
    using System;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.Purchasing;
    using UnityEngine.Serialization;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;
    #endregion

    [Serializable] public struct ProductIDs
    {
        public string subscriptionProductID;
    }

    /// <summary>This is the graphical interface, the view, of the shop.
    /// It should pull a developer uploaded assets to display a custom made shop interface for the user.</summary>
    public class SubscriptionWindow : MonoBehaviour
    {
        #region inspector fields
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private TextMeshProUGUI titleTMP;
        [SerializeField] private TextMeshProUGUI descriptionTMP;
        [SerializeField] private TextMeshProUGUI termsLinkTMP;
        [SerializeField] private TextMeshProUGUI restoreLinkTMP;
        [SerializeField] private TextMeshProUGUI encouragementTitleTMP;
        [SerializeField] private TextMeshProUGUI priceTMP;
        [SerializeField] private Image subscribeButtonImage;
        [SerializeField] private TextMeshProUGUI subscribeButtonTMP;
        #endregion // inspector fields

        private AppCentralStoreListener storeListener;

        // It must be unique, always available and always accessible. It's the interface's presence in the game.
        public static SubscriptionWindow Instance { get; private set; }

        public void OpenAppCentralTerms()
        {
            // TODO: Should be a const field or from a configuration file
            Application.OpenURL("https://www.app-central.com/terms");
        }

        public void RestorePurchases()
        {
            AppCentralStoreListener.RestorePurchases();
        }

        public void BuySubscription()
        {
            AppCentralStoreListener.BuyProduct(AppCentralStoreListener.ProductType.Subscription);
        }

        public void Initialise(ProductIDs productIDs)
        {
            static void OpenWindow()
            {
                SubscriptionWindow.Instance.titleTMP.text = AppCentralStoreListener.LocalizedTitle;
                SubscriptionWindow.Instance.descriptionTMP.text = AppCentralStoreListener.LocalizedDescription;
                SubscriptionWindow.Instance.priceTMP.text = "Just " + AppCentralStoreListener.LocalizedPriceString + " per month";

                SubscriptionWindow.Instance.gameObject.SetActive(true);
            }

            if (AppCentralStoreListener.IsUserSubscribed())
            {
                Debug.Log("User already subscribed, not showing paywall");
                return;
            }

			AnalyticsCommunicator.SendApplicationStartRequest();

            SubscriptionWindow.Instance.storeListener = new AppCentralStoreListener(productIDs, OpenWindow);
        }

        public static void HidePanel()
        {
            SubscriptionWindow.Instance.gameObject.SetActive(false);
        }

        /// <summary>Find a component in a child that corresponds to the given matches.</summary>
        /// <param name="match1">The first string to match the name to.</param>
        /// <param name="match2">The second string to match the name to.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        /// <returns>The component if found, null otherwise.</returns>
        private T GetComponent<T>(string match1, string match2 = null) where T : Component
        {
            return this.GetComponentsInChildren<T>().FirstOrDefault(
                    t => t.gameObject.name.ToLower().Contains(match1)
                         && (match2 == null || t.gameObject.name.ToLower().Contains(match2)));
        }

        private void CheckAssignments()
        {
            this.backgroundImage ??= this.GetComponent<Image>("background");
            this.foregroundImage ??= this.GetComponent<Image>("foreground");

            this.titleTMP ??= this.GetComponent<TextMeshProUGUI>("title");
            this.descriptionTMP ??= this.GetComponent<TextMeshProUGUI>("description");
            this.termsLinkTMP ??= this.GetComponent<TextMeshProUGUI>("terms", "button");

            this.restoreLinkTMP ??= this.GetComponent<TextMeshProUGUI>("restore", "button");
            this.encouragementTitleTMP ??= this.GetComponent<TextMeshProUGUI>("encouragement");
            this.priceTMP ??= this.GetComponent<TextMeshProUGUI>("price");

            this.subscribeButtonImage ??= this.GetComponent<Image>("subscribe", "button");
            this.subscribeButtonTMP ??= this.GetComponent<TextMeshProUGUI>("subscribe", "tmp");
        }

        private void OnValidate()
        {
            this.CheckAssignments();
        }

        private void Awake()
        {
            if (SubscriptionWindow.Instance != null && SubscriptionWindow.Instance != this)
            {
                Object.DestroyImmediate(this.gameObject);
                return;
            }
            SubscriptionWindow.Instance = this;
        }
    }
}