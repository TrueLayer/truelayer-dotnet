namespace TrueLayer.AcceptanceTests.Clients;

public enum MockBankPaymentAction
{
    Cancel,
    RejectAuthorisation,
    Execute,
    RejectExecution,
}

public enum MockBankMandateAction
{
    Authorise,
    RejectAuthorisation,
    Revoke,
    Cancel,
}
