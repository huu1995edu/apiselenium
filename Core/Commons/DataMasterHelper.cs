
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
namespace DockerApi
{
    public static class DataMasterHelper
    {

        #region List link check
        public static List<string> _linkChecks;
        public static List<string> getLinkChecked()
        {
             if (_linkChecks == null)
                {
                    var l = FileHelper.GetValue("LinkChecks")?.ToObject<List<String>>();
                _linkChecks = l;
                 }

             return _linkChecks;
        }

        public static void setLinkChecked(List<string> value)
        {
            _linkChecks = value;
            FileHelper.Add("LinkChecks", value);
        }
        
        #endregion List account

        #region List link check
        public static List<string> _accounts;
        public static List<string> getAccounts()
        {
             if (_accounts == null)
                {
                    var l = FileHelper.GetValue("Accounts")?.ToObject<List<String>>();
                _accounts = l;
                 }
            if(_accounts ==null || _accounts.Count == 0) _accounts=Variables.SELENIUM_ACCOUNTS_DEFAULT;
             return _accounts;
        }

        public static void setAccounts(List<string> value)
        {
            _accounts = value;
            FileHelper.Add("Accounts", value);
        }
        
        #endregion List link check

    }
}
