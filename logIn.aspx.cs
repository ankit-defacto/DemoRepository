using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
public partial class logIn : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        FormsAuthentication.Initialize();   // Initializes the FormsAuthentication object based on the configuration settings for the application.

        SqlConnection con = new SqlConnection("server=saurabh-pc;database=ss;integrated security=true");
     

        SqlCommand cmd = new SqlCommand("select role from roles r inner join users u on r.ID=u.roleid where u.username=@username and u.password=@password",con);
        
        cmd.Parameters.Add("@username", SqlDbType.VarChar, 30).Value = TextBox1.Text;
        cmd.Parameters.Add("@password", SqlDbType.VarChar, 30).Value = TextBox2.Text;
        FormsAuthentication.HashPasswordForStoringInConfigFile(TextBox2.Text, "md5"); // Or "sha1"

        con.Open();
        SqlDataReader dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            // Create a new ticket used for authentication
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                                                                                1,  // Ticket version
                                                                                TextBox1.Text, // Username associated with ticket
                                                                                DateTime.Now,  // Date/time issued
                                                                                DateTime.Now.AddMinutes(30), // Date/time to expire
                                                                               true,    // "true" for a persistent user cookie
                                                                               dr.GetString(0), // User-data, in this case the roles
                                                                               FormsAuthentication.FormsCookiePath // Path cookie valid for
                                                                                );

            // Encrypt the cookie using the machine key for secure transport

            string hash = FormsAuthentication.Encrypt(ticket);

            HttpCookie ck = new HttpCookie(FormsAuthentication.FormsCookieName,// Name of auth cookie
                                            hash); // Hashed ticket


            // Set the cookie's expiration time to the tickets expiration time

            if (ticket.IsPersistent)
            {
                ck.Expires = ticket.Expiration;
            }
            // Add the cookie to the list for outgoing response
            Response.Cookies.Add(ck);
            // Redirect to requested URL, or homepage if no previous page
            // requested
            string returnUrl=Request.QueryString["ReturnUrl"];
            if (returnUrl == null)
            {
                returnUrl = "home.aspx";
            }
            // Don't call FormsAuthentication.RedirectFromLoginPage since it
            // could
            // replace the authentication ticket (cookie) we just added
            Response.Redirect(returnUrl);

        }
        else
        {
            Label1.Text = "incorrect username/password";
        }



    }
}