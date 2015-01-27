//-----------------------------------------------------------------
//  Copyright 2013 Alex McAusland and Ballater Creations
//  All rights reserved
//  www.outlinegames.com
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace Unibill.Impl {

    /// <summary>
    /// Callback interface for <see cref="IBillingService"/>s.
    /// </summary>
    public interface IBillingServiceCallback {
        void onSetupComplete(bool successful);

        // This variant should be called when we have a receipt for the purchase.
        void onPurchaseSucceeded(string platformSpecificId, string receipt);
        void onPurchaseCancelledEvent(string item);
        void onPurchaseRefundedEvent(string item);
        void onPurchaseFailedEvent(string item);
        void onPurchaseDeferredEvent(string item);

        void onTransactionsRestoredSuccess();
        void onTransactionsRestoredFail(string error);

		void onActiveSubscriptionsRetrieved (IEnumerable<string> subscriptions);
        void onPurchaseReceiptRetrieved (string productId, string receipt);
        void setAppReceipt (string receipt);

        void logError(UnibillError error, params object[] args);
        void logError(UnibillError error);
    }
}
