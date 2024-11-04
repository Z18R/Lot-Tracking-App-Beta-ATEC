using Microsoft.AspNetCore.Mvc;
using LotTrackingApp.Services;
using LotTrackingApp.Models;
using System.Collections.Generic;

public class LotTrackingController : Controller
{
    private readonly LotTrackingService _trackingService;

    public LotTrackingController(LotTrackingService trackingService)
    {
        _trackingService = trackingService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult GetLotDetails(string lotAlias)
    {
        LotTracking lotDetails = _trackingService.GetLotDetails(lotAlias);
        return View("LotDetails", lotDetails); // Ensure this view is created
    }

    [HttpGet]
    public IActionResult GetTransactionDetails(long lotCode)
    {
        List<TransactionDetail> transactionDetails = _trackingService.GetTransactionDetails(lotCode);
        return PartialView("_TransactionDetailsPartial", transactionDetails);
    }

    [HttpPost]
    public IActionResult TrackIn(string lotAlias, int qtyIn, int equipmentCode, int trackInUser)
    {
        bool success = _trackingService.TrackIn(lotAlias, qtyIn, equipmentCode, trackInUser);

        if (success)
        {
            return Json(new { success = true, message = "Track-In successful." });
        }
        else
        {
            return Json(new { success = false, message = "No matching sequence found or Track-In failed." });
        }
    }



}
