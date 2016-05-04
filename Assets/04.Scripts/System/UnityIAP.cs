#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// You must obfuscate your secrets using Window > Unity IAP > Receipt Validation Obfuscator
// before receipt validation will compile in this sample.
#define RECEIPT_VALIDATION
#endif

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
#if RECEIPT_VALIDATION
using UnityEngine.Purchasing.Security;
#endif

public delegate void PurchaseCallback(PurchaseEventArgs e);

/// <summary>
/// An example of basic Unity IAP functionality.
/// To use with your account, configure the product ids (AddProduct)
/// and Google Play key (SetPublicKey).
/// </summary>
[AddComponentMenu("Unity IAP/Demo")]
public class UnityIAP : MonoBehaviour, IStoreListener
{
    // Unity IAP objects 
    private IStoreController m_Controller;
    private IAppleExtensions m_AppleExtensions;
    private PurchaseCallback purchaseCallback;
    private bool m_PurchaseInProgress;

    #if RECEIPT_VALIDATION
    private CrossPlatformValidator validator;
    #endif

    /// <summary>
    /// This will be called when Unity IAP has finished initialising.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        m_Controller = controller;
        m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();

        // On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.
        // On non-Apple platforms this will have no effect; OnDeferred will never be called.
        m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred);

        Debug.Log("Available items:");
        foreach (var item in controller.products.all)
        {
            if (item.availableToPurchase)
            {
                Debug.Log(string.Join(" - ",
                    new[]
                    {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString
                    }));
            }
        }
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase OK: " + e.purchasedProduct.definition.id);
        Debug.Log("Receipt: " + e.purchasedProduct.receipt);

        m_PurchaseInProgress = false;
        #if RECEIPT_VALIDATION
        if (Application.platform == RuntimePlatform.Android ||
        Application.platform == RuntimePlatform.IPhonePlayer ||
        Application.platform == RuntimePlatform.OSXPlayer) {
        try {
        var result = validator.Validate(e.purchasedProduct.receipt);
        Debug.Log("Receipt is valid. Contents:");
        foreach (IPurchaseReceipt productReceipt in result) {
        Debug.Log(productReceipt.productID);
        Debug.Log(productReceipt.purchaseDate);
        Debug.Log(productReceipt.transactionID);

        GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
        if (null != google) {
        Debug.Log(google.purchaseState);
        Debug.Log(google.purchaseToken);
        }

        AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
        if (null != apple) {
        Debug.Log(apple.originalTransactionIdentifier);
        Debug.Log(apple.cancellationDate);
        Debug.Log(apple.quantity);
        }
        }
        } catch (IAPSecurityException) {
        Debug.Log("Invalid receipt, not unlocking content");
        return PurchaseProcessingResult.Complete;
        }
        }
        #endif

        // You should unlock the content here.

        // Indicate we have handled this purchase, we will not be informed of it again.x
        if (purchaseCallback != null)
            purchaseCallback(e);
        
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product item, PurchaseFailureReason r)
    {
        Debug.Log("Purchase failed: " + item.definition.id);
        Debug.Log(r);
        StartCoroutine(waitCancel());
    }

    IEnumerator waitCancel() {
        yield return new WaitForEndOfFrame();

        m_PurchaseInProgress = false;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Billing failed to initialize!");
        switch (error)
        {
            case InitializationFailureReason.AppNotKnown:
                Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
                break;
            case InitializationFailureReason.PurchasingUnavailable:
                // Ask the user if billing is disabled in device settings.
                Debug.Log("Billing disabled!");
                break;
            case InitializationFailureReason.NoProductsAvailable:
                // Developer configuration error; check product metadata.
                Debug.Log("No products available for purchase!");
                break;
        }
    }

    /// <summary>
    /// This will be called after a call to IAppleExtensions.RestoreTransactions().
    /// </summary>
    private void OnTransactionsRestored(bool success)
    {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// iOS Specific.
    /// This is called as part of Apple's 'Ask to buy' functionality,
    /// when a purchase is requested by a minor and referred to a parent
    /// for approval.
    /// 
    /// When the purchase is approved or rejected, the normal purchase events
    /// will fire.
    /// </summary>
    /// <param name="item">Item.</param>
    private void OnDeferred(Product item)
    {
        Debug.Log("Purchase deferred: " + item.definition.id);
    }

    public void OnBuy(int index) {
        if (!m_PurchaseInProgress) {
            m_PurchaseInProgress = true;
            if (index < m_Controller.products.all.Length)
                m_Controller.InitiatePurchase(m_Controller.products.all[index]);
            else
                Debug.Log("Product index error " + index.ToString());
        }
    }

    public void InitProducts(GameStruct.TMall[] malls, PurchaseCallback callback)
    {
        purchaseCallback = callback;
        var module = StandardPurchasingModule.Instance();

        // The FakeStore supports: no-ui (always succeeding), basic ui (purchase pass/fail), and 
        // developer ui (initialization, purchase, failure code setting). These correspond to 
        // the FakeStoreUIMode Enum values passed into StandardPurchasingModule.useFakeStoreUIMode.
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

        var builder = ConfigurationBuilder.Instance(module);
        // This enables the Microsoft IAP simulator for local testing.
        // You would remove this before building your release package.
        //builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
        builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAg5VOnFNmCKbpGvWJIDT73ocwTvZc0iaHwitsog6KyvuPHT+ioohvYGQPxya83noWlrjGJ/H2To+rc1zVd/dMQLAOZ3zHFzbJf8jUEuIjiuTzfyxiaLeW8Gsvct5NSZ/tTvUjxJqWnSbpwzhl80tMGKjDNY7IBSVa3WjAz2ukq3bHdRTldXp+N2zY4NXy4KNO+lhRoSgnEVqPs+PSvV8OGBHmC0uivHsBE1XXMbvjCv5VO3LsJGa+zKd0cvJs8HlKX8JH7we5JaAa1McFvJ5/SS4WJjbkxNYCATtK0wvTb/oG0ktCjewvr7ES+WCAhqsHdDuAisSzz+qFrpgZfsTbxwIDAQAB");

        // Define our products.
        // In this case our products have the same identifier across all the App stores,
        // except on the Mac App store where product IDs cannot be reused across both Mac and
        // iOS stores.
        // So on the Mac App store our products have different identifiers,
        // and we tell Unity IAP this by using the IDs class.

        for (int i = 0; i < malls.Length; i++) {
            if (!string.IsNullOrEmpty(malls[i].Android)) 
                builder.AddProduct(malls[i].Android, ProductType.Consumable);
        }

        // Write Amazon's JSON description of our products to storage when using Amazon's local sandbox.
        // This should be removed from a production build.
        //builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);

        #if RECEIPT_VALIDATION
        validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.bundleIdentifier);
        #endif

        // Now we're ready to initialize Unity IAP.
        UnityPurchasing.Initialize(this, builder);
    }

    public bool IAPinProcess {
        get { return m_PurchaseInProgress; } 
    }
}
