using System.Security.Cryptography;

using System;
using System.Web.Mvc;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;


namespace MD5.Controllers
{
    public class UserController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string textToHash)
        {
            if (!string.IsNullOrEmpty(textToHash))
            {
                string hashString = null;

                // Creating an MD5 hash object
                using (MD5Cng md5Hash = new MD5Cng())
                {
                    // Computing the hash of the input string
                    byte[] hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(textToHash));

                    // Converting the hashed bytes to a string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("x2"));
                    }
                    hashString = sb.ToString();
                }

                // Saving the hashed string to the database
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString);
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO HashedText (TextToHash, HashedText) VALUES (@TextToHash, @HashedText)", conn);
                    command.Parameters.AddWithValue("@TextToHash", textToHash);
                    command.Parameters.AddWithValue("@HashedText", hashString);
                    command.ExecuteNonQuery();
                    conn.Close();
                }

                ViewBag.TextToHash = textToHash; // Passing the original text to the view
                ViewBag.Hash = hashString; // Passing the hash string to the view
            }
            else
            {
                ViewBag.TextToHash = null; // Clearing the ViewBag properties
                ViewBag.Hash = null;
            }

            return View();
        }
    }
}
