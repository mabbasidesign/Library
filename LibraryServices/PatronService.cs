﻿using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class PatronService : IPatron
    {
        private readonly LibraryContext _context;
        public PatronService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(Patron newPatron)
        {
            _context.Add(newPatron);
            _context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return GetAll().FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
                .Include(p => p.LibraryCard)
                .Include(p => p.HomeLibraryBranch)
                .ToList();
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int patronId)
        {
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.CheckoutHistories
                .Include(c => c.LibraryCard)
                .Include(c => c.LibraryAsset)
                .Where(c => c.LibraryCard.Id == cardId)
                .OrderByDescending(c => c.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int id)
        {
            var cardId = Get(id).LibraryCard.Id;

            return _context.Checkouts
                .Include(c => c.LibraryCard)
                .Include(c => c.LibraryAsset)
                .Where(c => c.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.Holds
                .Include(h => h.LibraryAsset)
                .Include(h => h.LibraryCard)
                .Where(h => h.LibraryCard.Id == cardId)
                .OrderByDescending(h => h.HoldPlaced);
        }
    }
}
