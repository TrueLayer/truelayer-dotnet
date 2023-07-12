using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Mandates.Model.MandateDetail
{
    public static class Status
    {
        public const string AuthorizationRequired = "authorization_required";
        public const string Authorizing = "authorizing";
        public const string Authorized = "authorized";
        public const string Failed = "failed";
        public const string Revoked = "revoked";

    }
}
