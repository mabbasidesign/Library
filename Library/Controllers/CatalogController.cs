using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Models.Catalog;
using LibraryData;
using LibraryServices;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _asset;
        public CatalogController(ILibraryAsset asset)
        {
            _asset = asset;
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
    }
}