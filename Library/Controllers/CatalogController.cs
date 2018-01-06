using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Catalog;
using Library.Models.CheckoutModels;
using LibraryData;
using LibraryServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _asset;
        private readonly ICheckout _checkouts;
        public CatalogController(ILibraryAsset asset, ICheckout checkouts)
        {
            _asset = asset;

            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            var assetModels = _asset.GetAll();

            var ListingResult = assetModels
                .Select(result => new AssetIndexListingModels
                {
                    Id = result.Id,
                    AuthorOrDirector = _asset.AuthorOrDirector(result.Id),
                    DeweyCallNumber = _asset.GetDeweyIndex(result.Id),
                    ImageUrl = result.ImageUrl,
                    Title = result.Title,
                    Type = _asset.GetType(result.Id),
                });

            var model = new AssetIndexModel()
            {
                Asset = ListingResult,
            };

            return View(model);
        }

        //public IActionResult Detail(int id)
        //{
        //    var asset = _asset.GetById(id);

        //    var model = new AseetDetailModel
        //    {
        //        AssetId = asset.Id,
        //        Title = asset.Title,
        //        Cost = asset.Cost,
        //        Year = asset.Year,
        //        ImageURL = asset.ImageUrl,
        //        Status = asset.Status.Name,
        //        CurrentLocation = _asset.GetCurrentLocations(id).Name,
        //        DeweyCallNumber = _asset.GetDeweyIndex(id),
        //        ISBN = _asset.GetIsbn(id),
        //        AuthorOrDirector = _asset.AuthorOrDirector(id),
        //    };

        //    return View(model);
        //}

        public IActionResult Detail(int id)
        {
            var asset = _asset.GetById(id);

            var currentHolds = _checkouts.GetCurrentHolds(id).Select(a => new AssetHoldModel
            {
                HoldPlaced = _checkouts.GetCurrentHoldPlaced(a.Id).ToString("d"),
                PatronName = _checkouts.GetCurrentHoldPatron(a.Id)
            });

            var model = new AseetDetailModel
            {
                AssetId = asset.Id,
                Title = asset.Title,
                Cost = asset.Cost,
                Year = asset.Year,
                ImageURL = asset.ImageUrl,
                Status = asset.Status.Name,
                CurrentLocation = _asset.GetCurrentLocations(id).Name,
                DeweyCallNumber = _asset.GetDeweyIndex(id),
                ISBN = _asset.GetIsbn(id),
                AuthorOrDirector = _asset.AuthorOrDirector(id),
                CheckoutHistory = _checkouts.GetCheckoutHistory(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                PatronName = _checkouts.GetCurrentCheckoutPatron(id),
                CurrentHold = currentHolds
            };
            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = _asset.GetById(id);
            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkouts.CheckInItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkouts.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkLost(int assetId)
        {
            _checkouts.MarkLost(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult MarkFound(int assetId)
        {
            _checkouts.MarkFound(assetId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        public IActionResult Hold(int id)
        {
            var asset = _asset.GetById(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkouts.IsCheckedOut(id),
                HoldCount = _checkouts.GetCurrentHolds(id).Count(),
            };
            return View(model);
        }

    }
}