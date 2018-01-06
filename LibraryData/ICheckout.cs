using LibraryData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryData
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout Get(int id);
        Checkout GetById(int checkoutId);
        void Add(Checkout newCheckout);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        void PlaceHold(int assetId, int libraryCardId);
        void CheckoutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId, int LibraryCardId);
        Checkout GetLatestCheckout(int id);
        int GetNumberOfCopies(int id);
        int GetAvailableCopies(int id);
        bool IsCheckedOut(int id);

        string GetCurrentHoldPatron(int id);
        DateTime GetCurrentHoldPlaced(int id);
        string GetCurrentCheckoutPatron(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        void MarkLost(int id);
        void MarkFound(int id);
    }
}
