using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using OWASP.WebGoat.NET.App_Code.DB;
using OWASP.WebGoat.NET.App_Code;

namespace OWASP.WebGoat.NET
{
	public partial class SQLInjection : System.Web.UI.Page
	{
    
        private IDbProvider du = Settings.CurrentDbProvider;
        
		protected void Page_Load (object sender, EventArgs e)
		{

		}

		protected void btnFind_Click(object sender, EventArgs e)
		{
		    DataSet ds = null;
            string name = txtName.Text;
	//	    try
	//	    {
		        ds = du.GetEmailByName(name);
                /*
		    }
		    catch (Exception ex)
		    {
                throw new System.Data.SQLite.SQLiteException("SQL Injection Internal");
		    }
                */
		    if (ds != null)
            {
                grdEmail.DataSource = ds.Tables[0];
                grdEmail.DataBind();
            }
		}

        protected void btnFindSecured_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Replace("'","''");
            DataSet ds = du.GetEmailByName(name);

            if (ds != null)
            {
                grdEmail.DataSource = ds.Tables[0];
                grdEmail.DataBind();
            }
        }

        protected void btnFindNumeric_Click(object sender, EventArgs e)
        {
            string name = txtName.Text;
            DataSet ds = du.GetCustomerDetails(name);

            if (ds != null)
            {
                grdEmail.DataSource = ds.Tables[0];
                grdEmail.DataBind();
            }
        }
	}
}