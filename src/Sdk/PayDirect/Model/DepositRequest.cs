using System;
using System.Collections.Generic;

namespace TrueLayer.PayDirect.Model
{
    /// <summary>
    /// Request a deposit into a user's account
    /// </summary>
    public class DepositRequest
    {
        /// <summary>
        /// Creates a new <see cref="DepositRequest"/>
        /// </summary>
        /// <param name="userId">The identifier of the user to which you wish to deposit funds</param>
        /// <param name="deposit">The deposit details</param>
        /// <param name="authFlow">The authorization flow used to deposit the details</param>
        public DepositRequest(Guid userId, DepositDetails deposit, DepositAuthFlow authFlow)
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
        public DepositDetails Deposit { get; }

        /// <summary>
        /// Specifies how the payment should be authorised.
        /// </summary>
        public DepositAuthFlow AuthFlow { get; }

        /// <summary>
        /// An address to which payment webhooks with the status of the payment should be sent. 
        /// Has to be https.
        /// </summary>
        public string? WebhookUri { get; set; }

        /// <summary>
        /// Represents the details of the deposit
        /// </summary>
        public class DepositDetails
        {
            public DepositDetails(
                long amountInMinor,
                string currency,
                string providerId,
                Guid? depositId = null
            )
            {
                AmountInMinor = amountInMinor.GreaterThan(0, nameof(amountInMinor));
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
            public Remitter? Remitter { get; set; }

            /// <summary>
            /// The reference to appear on the remitters’ statement.
            /// </summary>
            public string? RemitterReference { get; set; }
        }


        /// <summary>
        /// Represents the remitter of the deposit
        /// </summary>
        public class Remitter
        {
            public Remitter(AccountIdentifier account)
            {
                Account = account.NotNull(nameof(account));
            }

            /// <summary>
            /// The identifier of the remitters account used to deposit the funds.
            /// </summary>
            /// <value></value>
            public AccountIdentifier Account { get; }
            
            
            /// <summary>
            /// The name on the remitter's account.
            /// </summary>
            /// <value></value>
            public string? Name { get; set; }
        }


        /// <summary>
        /// Represents the identity of a bank account
        /// </summary>
        public class AccountIdentifier
        {
            public AccountIdentifier(string type)
            {
                Type = type.NotNullOrWhiteSpace(nameof(type));
            }

            /// <summary>
            /// The type of account identifier
            /// </summary>
            public string Type { get; }
            
            /// <summary>
            /// 6 digit sort code (no spaces or dashes)
            /// </summary>
            public string? SortCode { get; set; }
            
            /// <summary>
            /// 8 digit account number
            /// </summary>
            public string? AccountNumber { get; set; }
            
            /// <summary>
            /// Valid International Bank Account Number (no spaces). 
            /// Consists of a 2 letter country code, followed by 2 check digits, and then by up to 30 alphanumeric characters (also known as the BBAN).
            /// </summary>
            public string? Iban { get; set; }
            
            /// <summary>
            /// Valid Basic Bank Account Number (no spaces). Consists of up to 30 alphanumeric characters, with a fixed length per country. 
            /// </summary>
            public string? Bban { get; set; }
            
            /// <summary>
            /// Valid Polish NRB (no spaces). Consists of 2 check digits, followed by an 8 digit bank branch number, and then by a 16 digit bank account number. 
            /// Equivalent to a Polish IBAN with the country code removed.
            /// </summary>
            public string? Nrb { get; set; }
        }

        /// <summary>
        /// Specifies how the deposit should be authorised
        /// </summary>
        public class DepositAuthFlow
        {
            public DepositAuthFlow(string type)
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
