using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.Provider;

namespace TrueLayer.Mandates.Model.MandateDetail
{
    using Provider = OneOf<UserSelected, Preselected>;
    using Beneficiary = OneOf<Beneficiary.ExternalAccount, Beneficiary.MerchantAccount>;

    public record MandateDetail(
        string id,
        string currency,
        Beneficiary beneficiary,
        string reference,
        PaymentUser user,
        DateTime createdAt,
        Constraints constraints,
        Dictionary<string, string> metadata,
        Provider providerSelection
    );
}
