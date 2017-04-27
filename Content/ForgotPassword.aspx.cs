using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OWASP.WebGoat.NET.App_Code;
using OWASP.WebGoat.NET.App_Code.DB;
using Encoder = OWASP.WebGoat.NET.App_Code.Encoder;

namespace OWASP.WebGoat.NET
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        private IDbProvider du = Settings.CurrentDbProvider;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                PanelForgotPasswordStep2.Visible = false;
                PanelForgotPasswordStep3.Visible = false;
            }
        }

        protected void ButtonCheckEmail_Click(object sender, EventArgs e)
        {
            string[] result = new string[] {"Default Question", "Default Answer"};
            bool errorOccured = false;
            try
            {
                result = du.GetSecurityQuestionAndAnswer(txtEmail.Text);
            }
            catch (Exception ex)
            {
                errorOccured = true;
                result = new string[] {"Error occurred while trying to read from DB"};
            }
            if (((result.Length > 0) && string.IsNullOrEmpty(result[0])) // empty result
                || (result.Length > 2) || (result.Length == 0)) // invalid result // 
            {
                labelQuestion.Text = "That email address was not found in our database!";
                PanelForgotPasswordStep2.Visible = false;
                PanelForgotPasswordStep3.Visible = false;
                return; // block invalid results
            }
            labelQuestion.Text = "<strong>" + result[0] + "</strong>";
            labelQuestion.Visible = true;
            PanelForgotPasswordStep2.Visible = true;
            PanelForgotPasswordStep3.Visible = false;
            if (result.Length > 1)
            {
                labelQuestion.Text = "Here is the question we have on file for you: <strong>" + result[0] + "</strong>";
                HttpCookie cookie = new HttpCookie("encr_sec_qu_ans");
                //encode twice for more security!cookie.Value = Encoder.Encode(Encoder.Encode(result[1]));
                Response.Cookies.Add(cookie);
            }
            else
            { // no answer
                txtAnswer.Visible = false;
                ButtonCheckAnswer.Visible = false;
            }
        }

        protected void ButtonRecoverPassword_Click(object sender, EventArgs e)
        {
            try
            {
                //get the security question answer from the cookie
                string encrypted_password = Request.Cookies["encr_sec_qu_ans"].Value.ToString();
                
                //decode it (twice for extra security!)
                string security_answer = Encoder.Decode(Encoder.Decode(encrypted_password));
                
                if (security_answer.Trim().ToLower().Equals(txtAnswer.Text.Trim().ToLower()))
                {
                    PanelForgotPasswordStep1.Visible = false;
                    PanelForgotPasswordStep2.Visible = false;
                    PanelForgotPasswordStep3.Visible = true;
                    labelPassword.Text = "Security Question Challenge Successfully Completed! <br/>Your password is: " + getPassword(txtEmail.Text);
                }
            }
            catch (Exception ex)
            {
                labelMessage.Text = "An unknown error occurred - Do you have cookies turned on? Further Details: " + ex.Message;
            }
        }

        string getPassword(string email)
        {
            string password = du.GetPasswordByEmail(email);
            return password;
        }

    }
}