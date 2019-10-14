using EnKindle.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace EnKindle.Pages
{
    public class UpdateModel : PageModel

    {
        //SECTION 1 ... DECLARATIONS GO HERE AT THE TOP OF PAGE
        private DB _DB;
        private readonly ILogger _logger;

        public string Message { get; set; }

        //////TELL ON POST WHAT DATABASE TABLE TO UPDATE...knows where to map your form... tells the onpost where to map your database table to update.... changed to _Greetings
        [BindProperty]
        public Greetings _Greetings { get; set; }

        //SECTION 2 ... CONSTRUCTORS 
        //THIS IS THE CONSTRUCTOR. VERY IMPORTANT! 
        public UpdateModel(
            DB dB,
            ILogger<UpdateModel> logger
            )
        {
            _DB = dB;
            _logger = logger;
        }

 
       //SECTION 3...ONGET THIS HAPPENS BEFORE THE PAGE LOADS
        public IActionResult OnGet(int id = 0)
        {
            if (id > 0)
            {
                _Greetings = _DB.Greetings.Find(id);
                return Page();
            }

            else
            {
                return RedirectToPage("Index");
            }

        }

        //SECTION 4... ONPOST, OCCURS WHEN PAGE IS SUBMIITED USUALLY BY A CLICK
        //PATTERN BELOW:

        // 1. GET THE RECORD FROM THE DATABASE TABLE YOU WANT TO UPDATE USING THE"[BindProperty]" DECLARED EARLIER IN SECTION 1 ABOVE.

        // 2. ONCE YOU FIND THE RECORD, ASSIGN IT TO A TEMPORARY VARIABLE.

        // 3. UPDATE THE TEMPORARY VARIABLE WITH THE FORM FIELDS USING THE "[BindProperty]" DECLARED EARLIER IN SECTION 1 ABOVE.

        // 4. USE YOUR  "_DB TO UPATE THE DATABASE.

        // 5. USE YOUR "_DB" TO SAVE THE CHANGES SUBMITTED FROM THE FORM.
        
        public async Task<IActionResult> OnPost()
        {
            try
            {
                //Read pattern. This is (1) + (2) 
                var tempGreetings = _DB.Greetings.Find(_Greetings.ID);

                if (ModelState.IsValid)
                {
                    //Read pattern above. This is (3)
                    await TryUpdateModelAsync(tempGreetings, "_Greetings");

                    //UPDATE RECORD ON THE DB
                    _DB.Greetings.Update(_Greetings); // This is 4
                    _DB.SaveChanges();  // This is 5

                    return RedirectToPage("Preview", new { id = _Greetings.ID });
                }
            }
            catch (Exception ex)
            {

                _logger.LogInformation($"Update OnPost {ex.Message}", ex);
                return Page();
            }
        }
    }
}
