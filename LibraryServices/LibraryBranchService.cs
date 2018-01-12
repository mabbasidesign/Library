using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    class LibraryBranchService : ILibraryBranch
    {
        private readonly LibraryContext _context;
        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return GetAll()
                .SingleOrDefault(b => b.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .Include(b => b.LibraryAssets)
                .ToList();
        }

        public int GetAssetCount(IEnumerable<LibraryAsset> libraryAssets)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.LibraryAssets)
                .ToList()
                .FirstOrDefault(b => b.Id == branchId)
                .LibraryAssets;
        }
        
        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hour = _context.BranchHours.Where(h => h.Branch.Id == branchId);
            return DataHelpers.HumanizeBizHours(hour);
        }        

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .ToList()
                .FirstOrDefault(b => b.Id == branchId)
                .Patrons;
        }       

        public bool IsBranchOpen(int branchId)
        {
            var currentTimeHour = DateTime.Now.Hour;
            var currentDayOfWeek = (int)DateTime.Now.DayOfWeek + 1;
            var hour = _context.BranchHours.Where(h => h.Branch.Id == branchId);

        }

        public int GetPatronCount(IEnumerable<Patron> patrons)
        {
            throw new NotImplementedException();
        }

        public decimal GetAssetsValue(int id)
        {
            throw new NotImplementedException();
        }

    }
}
