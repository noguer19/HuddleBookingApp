using DeskBooker.Core.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace DeskBooker.Web.Pages;

public class BookDeskConfirmationModel : PageModel
{
    public DeskBookingResult DeskBookingResult { get; set; }

    public void OnGet(DeskBookingResult deskBookingResult)
    {
        DeskBookingResult = deskBookingResult;
    }
}
