using System;
using System.Collections.Generic;

namespace TrueLayer.PayDirect.Model
{
    public class DepositRequest
    {
        public DepositRequest(Guid userId, DepositRequestDetails deposit, AuthFlowRequestDetails authFlow)
        {
            UserId = userId;
            Deposit = deposit ?? throw new ArgumentNullException(nameof(deposit));
            AuthFlow = authFlow ?? throw new ArgumentNullException(nameof(authFlow));
        }

        /// <summary>
        /// The user identifier
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// Specifies the details of the payment to be created.
        /// </summary>
        public DepositRequestDetails Deposit { get; }

        /// <summary>
        /// Specifies how the payment should be authorised.
        /// </summary>
        public AuthFlowRequestDetails AuthFlow { get; }

        /// <summary>
        /// An address to which payment webhooks with the status of the payment should be sent. 
        /// Has to be https.
        /// </summary>
        public string? WebhookUri { get; set; }

        public class DepositRequestDetails
        {
            public DepositRequestDetails(
                long amountInMinor,
                string currency,
                string providerId,
                Guid? depositId = null
            )
            {
                AmountInMinor = amountInMinor;
                Currency = currency.NotNullOrWhiteSpace(nameof(currency));
                ProviderId = providerId.NotNullOrWhiteSpace(nameof(providerId));
                DepositId = depositId ?? Guid.NewGuid();
            }

            /// <summary>
            /// The unique id of the payment you want to create. If two requests are received with the same id, only the first one will be processed.
            /// </summary>
            public Guid DepositId { get; }

            /// <summary>
            /// The amount, specified in terms of the fractional monetary unit of the payment currency, to be paid.
            /// </summary>
            /// <value></value>
            public long AmountInMinor { get; }

            /// <summary>
            /// The ISO 4217 code of the currency for the payment.
            /// </summary>
            public string Currency { get; }

            /// <summary>
            /// The id of the provider, as given on our /providers endpoint.
            /// </summary>
            public string ProviderId { get; }

            /// <summary>
            /// The id of the scheme to make the payment over. This must be one of the schemes advertised by the provider on the providers endpoint.
            /// If omitted, the default value of faster_payments_service will be used.
            /// </summary>
            public string? SchemeId { get; set; }

            /// <summary>
            /// The id indicating the desired distribution of fees between the beneficiary and remitter. 
            /// Must be a fee_option_id from one of the options advertised under that scheme on the /v1/deposits/providers endpoint. 
            /// Should be omitted if there are no fee options (the scheme is free).
            /// </summary>
            /// <value></value>
            public string? FeeOptionId { get; set; }

            /// <summary>
            /// The details of the remitter sending the payment.
            /// </summary>
            public ParticipantDetails? Remitter { get; set; }

            /// <summary>
            /// The reference to appear on the remitters’ statement.
            /// </summary>
            public string? RemitterReference { get; set; }
        }


        public class ParticipantDetails
        {
            public ParticipantDetails(AccountIdentifierDetails account)
            {
                Account = account.NotNull(nameof(account));
            }


            public AccountIdentifierDetails Account { get; set; }
            public string? Name { get; set; }
        }

        // TODO create factory methods to ensure different Account type fields are populated
        public class AccountIdentifierDetails
        {
            public AccountIdentifierDetails(string type)
            {
                Type = type.NotNullOrWhiteSpace(nameof(type));
            }

            public string Type { get; }
            public string? SortCode { get; set; }
            public string? AccountNumber { get; set; }
            public string? Iban { get; set; }
            public string? Bban { get; set; }
            public string? Nrb { get; set; }
        }

        // TODO create factory methods to ensure different auth flow type fields are populated
        public class AuthFlowRequestDetails
        {
            public AuthFlowRequestDetails(string type)
            {
                Type = type.NotNullOrWhiteSpace(nameof(type));
            }

            /// <summary>
            /// The type of authorisation flow. Must be redirect or embedded. 
            /// Providers requiring additional authorisation flows may be available in future.
            /// </summary>
            public string Type { get; }

            /// <summary>
            /// The URI we will return the user to after authorising the payment. 
            /// When the user is redirected to this URI, we will append a deposit_id query parameter specifying the payment id and a type query parameter with value deposit.
            /// </summary>
            public string? ReturnUri { get; set; }

            /// <summary>
            /// The IP address of the end user.
            /// </summary>
            public string? PsuIpAddress { get; set; }

            /// <summary>
            /// If the provider only allows a single active consent across both AIS and PIS services, 
            /// in order to prevent invalidating an existing AIS consent, you can pass the data access token 
            /// on this field, and we will preserve the data consent when requesting authorisation for the payment.
            /// </summary>
            public string? DataAccessToken { get; set; }

            /// <summary>
            /// A dictionary of additional values required on a per-provider basis. 
            /// Please refer to our providers endpoint for details of the providers that require these.
            /// </summary>
            public Dictionary<string, string>? AdditionalInputs { get; set; }
        }
    }
}
