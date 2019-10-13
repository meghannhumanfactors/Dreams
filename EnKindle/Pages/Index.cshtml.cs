using EnKindle.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace EnKindle.Pages
{
    public class IndexModel : PageModel
    {

        // DEFAULT MODE ...
        public void OnGet()
        {
 
        }
                             
        // BRIDGE TO GREETINGS MODEL
        [BindProperty]
        public Greetings _Greetings { get; set; }

        private IConfiguration _myConfiguration { get; set; }


        // HEY, CONNECT MY DATABASE TO THIS MODEL
        private DB _DB;
        public IndexModel(DB myDB, IConfiguration myConfiguration)
        {
            _DB = myDB;
            _myConfiguration = myConfiguration;

        }
        //private readonly ILogger _logger;
        //public IndexModel(
        //    DB dB,
        //    ILogger<UpdateModel> logger
        //    )
        //{
        //    _DB = dB;
        //    _logger = logger;
        //}





        // PREVIEW MODE (AFTER SUBMITTING)
        public async Task<IActionResult> OnPost()
        {
            if (await IsValid())
            {
                if (ModelState.IsValid)
                {

                    try
                    {
                        // DB-RELATED: CUSTOMIZE VALUES TO BE ADDED TO THE DB
                        _Greetings.CreateDate = DateTime.Now.ToString();
                        _Greetings.CreateIP = this.HttpContext.Connection.RemoteIpAddress.ToString();



                        // DB-RELATED: ADD NEW RECORD TO THE DATABASE 
                        _DB.Greetings.Add(_Greetings);
                        _DB.SaveChanges();

                        // DB-RELATED: SEND USER TO THE PREVIEW PAGE SHOWING THE NEW RECORD
                        return RedirectToPage("Preview", new { id = _Greetings.ID });
                    }
                    catch
                    {
                        return RedirectToPage("Index");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("_Greetings.ReCaptcha", "Please select the checkbox!");
            }

            return Page();
        }


        // RE-CAPTCHA VALIDATION
        private async Task<bool> IsValid()
        {
            var response = this.HttpContext.Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(response))
                return false;

            try
            {
                using (var client = new HttpClient())
                {
                    var values = new Dictionary<string, string>();
                    values.Add("secret", _myConfiguration["Recaptcha:PrivateKey"]);
                    values.Add("response", response);
                    values.Add("remoteip", this.HttpContext.Connection.RemoteIpAddress.ToString());

                    var query = new FormUrlEncodedContent(values);


                    var post = client.PostAsync("https://www.google.com/recaptcha/api/siteverify", query);

                    var json = await post.Result.Content.ReadAsStringAsync();

                    if (json == null)
                        return false;

                    var results = JsonConvert.DeserializeObject<dynamic>(json);

                    return results.success;
                }
            }
            catch { }

            return false;
        }
    }
}
