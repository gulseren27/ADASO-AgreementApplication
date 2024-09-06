namespace ADASO_AgreementApp.Helper
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using ADASO_AgreementApp.Models.Entity;


    public static class UserHelper
    {


        public static Adminn GetCurrentUser()
        {
            return (Adminn)System.Web.HttpContext.Current.Session["user"];

        }
        public static void SetCurrentUser(Adminn currentUser)
        {
            System.Web.HttpContext.Current.Session["user"] = currentUser;

        }
    }
}