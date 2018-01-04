using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private readonly LibraryContext _context;
        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public string AuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>()
               .Where(a => a.Id == id).Any();
            var isVideo = _context.LibraryAssets.OfType<Video>()
                .Where(a => a.Id == id).Any();
            return isBook ?
                _context.Books.SingleOrDefault(b => b.Id == id).Author :
            _context.Videos.SingleOrDefault(v => v.Id == id).Director
            ?? "Unknown";
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Status)
                .Include(asset => asset.Location)
                .ToList();
        }

        public LibraryAsset GetById(int id)
        {
            return 
                GetAll()
                .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocations(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(a => a.Id == id))
            {
                return _context.Books
                    .SingleOrDefault(a => a.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(a => a.Id == id))
            {
                return _context.Books
                    .SingleOrDefault(a => a.Id == id).ISBN;
            }
            else return "";
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets
                .FirstOrDefault(a => a.Id == id)
                .Title;
        }

        public string GetType(int id)
        {
            var Book = _context.LibraryAssets.OfType<Book>()
                 .Where(b => b.Id == id);
            return Book.Any() ? "Book" : "Video";
        }
    }
}
