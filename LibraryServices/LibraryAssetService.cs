using LibraryData;
using System;
using System.Linq;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;

        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {
            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets
                .Include(asset => asset.Location)
                .Include(asset => asset.Status);
        }

        public string GetAthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>()
                  .Where(a => a.Id == id).Any();

            var isVideo = _context.LibraryAssets.OfType<Video>()
                .Where(a => a.Id == id).Any();

            return isBook ?
            _context.Books.FirstOrDefault(a => a.Id == id).Author :
            _context.Videos.FirstOrDefault(a => a.Id == id).Director
                ?? "Unknown";
        }

        public LibraryAsset GetById(int id)
        {
            return _context.LibraryAssets
               .Include(asset => asset.Location)
               .Include(asset => asset.Status)
               .FirstOrDefault(asset => asset.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(b => b.Id == id))
            {
                return _context.Books
                    .FirstOrDefault(b => b.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetIsbn(int id)
        {
            if (_context.Books.Any(a => a.Id == id))
            {
                return _context.Books
                    .FirstOrDefault(a => a.Id == id).ISBN;
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
                .Where(a => a.Id == id);

            return Book.Any() ? "Book" : "Video";
        }
    }
}
