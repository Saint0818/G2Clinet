//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// An example of basic Unibill functionality.
/// </summary>
public class UnibillDemo : MonoBehaviour {
	public delegate void HandlePurchased(string id, string Receipt);
	private HandlePurchased handlePurchased = null;
    private PurchasableItem[] items;

    void Start () {
        if (UnityEngine.Resources.Load ("unibillInventory.json") == null) {
            Debug.LogError("You must define your purchasable inventory within the inventory editor!");
            this.gameObject.SetActive(false);
            return;
        }

        // We must first hook up listeners to Unibill's events.
        Unibiller.onBillerReady += onBillerReady;
        Unibiller.onTransactionsRestored += onTransactionsRestored;
        Unibiller.onPurchaseCancelled += onCancelled;
	    Unibiller.onPurchaseFailed += onFailed;
		Unibiller.onPurchaseCompleteEvent += onPurchased;

        // Now we're ready to initialise Unibill.
        Unibiller.Initialise();
		items = Unibiller.AllPurchasableItems;
    }
    
    /// <summary>
    /// This will be called when Unibill has finished initialising.
    /// </summary>
    private void onBillerReady(UnibillState state) {
        UnityEngine.Debug.Log("onBillerReady:" + state);
    }

    /// <summary>
    /// This will be called after a call to Unibiller.restoreTransactions().
    /// </summary>
    private void onTransactionsRestored (bool success) {
        Debug.Log("Transactions restored.");
    }

    /// <summary>
    /// This will be called when a purchase completes.
    /// </summary>
	private void onPurchased(PurchaseEvent e) {
		Debug.Log("Purchase OK: " + e.PurchasedItem.Id);
        Debug.Log(string.Format ("{0} has now been purchased {1} times.",
								 e.PurchasedItem.name,
								 Unibiller.GetPurchaseCount(e.PurchasedItem)));

		if (handlePurchased != null)
			handlePurchased(e.PurchasedItem.Id, e.Receipt);
    }

    /// <summary>
    /// This will be called if a user opts to cancel a purchase
    /// after going to the billing system's purchase menu.
    /// </summary>
    private void onCancelled(PurchasableItem item) {
        Debug.Log("Purchase cancelled: " + item.Id);
    }
    
    /// <summary>
    /// This will be called is an attempted purchase fails.
    /// </summary>
    private void onFailed(PurchasableItem item) {
    Debug.Log("Purchase failed: " + item.Id);
    }

	public void BuyItem(int index, HandlePurchased purchased) {
		if (index >=0 && index < items.Length) {
			handlePurchased = purchased;
			Unibiller.initiatePurchase(items[index].Id);
		}
	}
}
