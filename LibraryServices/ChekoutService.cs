using LibraryData;
using LibraryData.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class ChekoutService : ICheckout
    {
        private readonly LibraryContext _context;
        public ChekoutService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }       

        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts.ToList();
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll()
                .SingleOrDefault(a => a.Id == checkoutId);
        }

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(h => h.LibraryAsset)
                .ToList()
                .Where(h => h.LibraryAsset.Id == id);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .ToList()
                .Where(h => h.LibraryAsset.Id == id);
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets.FirstOrDefault(a => a.Id == assetId);

            // Remove any existing checkout on the item
            RemoveExistingCheckOut(assetId);
            // Close any existing checkout history
            ClosingExistingCheckoutHistory(assetId, now);
            // Look for existing hold in the item
            var currentHolds = _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .ToList()
                .Where(h => h.LibraryAsset.Id == assetId);
            // if there are hold, checkout the item to the libraryCard with the earliest hold
            if (currentHolds.Any())
            {
                CheckouToEarliestHold(assetId, currentHolds);
                return;
            }
            // otherwise update the items status to Available
            UpdateAssetStatus(assetId, "Available");

            _context.SaveChanges();
        }

        private void CheckouToEarliestHold(int assetId, IEnumerable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(hold => hold.HoldPlaced)
                .FirstOrDefault();
            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckInItem(/*assetId*/   card.Id);
        }

        public void CheckoutItem(int assetId, int libraryCardId)
        {
            if (IsCheckedOut(assetId))
            {
                return;
                // Add logic to handle fidback to user
            }
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == assetId);
            UpdateAssetStatus(assetId, "Checked Out");
            var libraryCard = _context.LibraryCards
                .Include(c => c.Checkouts)
                .ToList()
                .FirstOrDefault(c => c.Id == libraryCardId);

            var now = DateTime.Now;
            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckoutTime(now),
            };
            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard,

            };
            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public bool IsCheckedOut(int assetId)
        {
            return _context.Checkouts
                .Where(co => co.LibraryAsset.Id == assetId)
                .Any();
        }

        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(c => c.LibraryAsset.Id == id)
                .OrderByDescending(c => c.Since)
                .FirstOrDefault();
        }      

        public void MarkFound (int id)
        {
            var now = DateTime.Now;          

            UpdateAssetStatus(id, "Available");
            RemoveExistingCheckOut(id);
            ClosingExistingCheckoutHistory(id, now);
            
            _context.SaveChanges();
        }

        public void MarkLost(int id)
        {
            UpdateAssetStatus(id, "Lost");
            _context.SaveChanges();
        }

        private void UpdateAssetStatus(int id, string newStatus)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(s => s.Name == newStatus);
        }

        private void ClosingExistingCheckoutHistory(int id, DateTime now)
        {
            // Close existing checkout history
            var history = _context.CheckoutHistories
                .FirstOrDefault(h => h.LibraryAsset.Id == id && h.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckOut(int id)
        {
            //Remove existing checkout
            var checkout = _context.Checkouts
               .FirstOrDefault(a => a.LibraryAsset.Id == id);

            if (checkout != null)
            {
                _context.Remove(checkout);
            }
        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(a => a.Status)
                .ToList()
                .FirstOrDefault(a => a.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(a => a.Id == libraryCardId);

            _context.Update(asset);

            if (asset.Status.Name == "Available")
            {
                //asset.Status = _context.Statuses.FirstOrDefault(a => a.Name == "On Hold");
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }
        public string GetCurrentHoldPatron(int holdId)
        {
            var hold = _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .ToList()
                .Where(v => v.Id == holdId);

            var cardId = hold
                //.Include(a => a.LibraryCard)
                .Select(a => a.LibraryCard.Id)
                .FirstOrDefault();

            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return _context.Holds
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .ToList()
                .FirstOrDefault(v => v.Id == holdId)
                .HoldPlaced;
        }
        
        public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetcheckoutByAssetId(assetId);
            if(checkout == null)
            {
                return " ";
            };

            var cardId = checkout.LibraryCard.Id;
            var patron = _context.Patrons
                .Include(p => p.LibraryCard)
                .FirstOrDefault(p => p.LibraryCard.Id == cardId);
            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetcheckoutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(a => a.LibraryAsset)
                .Include(a => a.LibraryCard)
                .ToList()
                .FirstOrDefault(a => a.LibraryAsset.Id == assetId);
        }

        public int GetAvailableCopies(int id)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfCopies(int id)
        {
            throw new NotImplementedException();
        }

        public Checkout Get(int id)
        {
            throw new NotImplementedException();
        }
    }
}
