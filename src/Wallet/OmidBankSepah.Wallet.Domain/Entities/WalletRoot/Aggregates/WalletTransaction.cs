using OmidBankSepah.Wallet.Domain.Enums;
using Taday.Corelibrary.Domain.Entities;
using OmidBankSepah.Wallet.Domain.Constraint;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot;

namespace OmidBankSepah.Wallet.Domain.Entities.WalletRoot.Aggregates;
public class WalletTransaction : BaseEntity
{
    public const int IdempotencyKeyMaxLength = 500;
    public const string IdempotencyKeyUniqueIndexName = "UX_WalletTransaction_IdempotencyKey";

    private WalletTransaction()
    {
    }

    private WalletTransaction(
        decimal creditor,
        decimal debtor,
        string description,
        DateTime? commitDateTime,
        bool isSuccess,
        string referenceId,
        int? installmentBookId,
        OrderType orderType,
        decimal payableAmount,
        bool withdrawWalletBalance,
        OrderPaymentType paymentType)
    {
        Creditor = creditor;
        Debtor = debtor;
        Description = description;
        CommitDateTime = commitDateTime;
        IsSuccess = isSuccess;
        ReferenceId = referenceId;
        InstallmentBookId = installmentBookId;
        OrderType = orderType;
        PayableAmount = payableAmount;
        WithdrawWalletBalance = withdrawWalletBalance;
        PaymentType = paymentType;
    }

    public Guid WalletsId { get; private set; }
    public Wallets? Wallets { get; private set; }
    public decimal Creditor { get; private set; }
    public decimal Debtor { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime? CommitDateTime { get; private set; }
    public bool IsSuccess { get; private set; }
    public string ReferenceId { get; private set; } = string.Empty;
    public int? InstallmentBookId { get; private set; }
    public string? EndorsementId { get; private set; }
    public OrderType OrderType { get; private set; }
    public decimal PayableAmount { get; private set; }
    public bool WithdrawWalletBalance { get; private set; }
    public OrderPaymentType PaymentType { get; private set; }
    public decimal? BalanceAfter { get; private set; }
    public string? IdempotencyKey { get; private set; }

    protected override string[] IntegrityProperties => [];

    public static string BuildCreditIdempotencyKey
    (
        Guid walletsId,
        OrderType orderType, 
        string referenceId)
        => $"credit:{walletsId}:{(int)orderType}:{referenceId}";

    public static WalletTransaction Credit(
        decimal creditor,
        string description,
        string referenceId,
        int? installmentBookId,
        OrderType orderType,
        decimal payableAmount,
        bool withdrawWalletBalance,
        OrderPaymentType paymentType)
    {
        return new WalletTransaction(
            creditor,
            0,
            description,
            null,
            false,
            referenceId,
            installmentBookId,
            orderType,
            payableAmount,
            withdrawWalletBalance,
            paymentType);
    }

    public static WalletTransaction Debit(
        decimal debtor,
        string description,
        string referenceId,
        int? installmentBookId,
        OrderType orderType,
        decimal payableAmount,
        bool withdrawWalletBalance,
        OrderPaymentType paymentType)
    {
        return new WalletTransaction(
            0,
            debtor,
            description,
            DateTime.Now,
            true,
            referenceId,
            installmentBookId,
            orderType,
            payableAmount,
            withdrawWalletBalance,
            paymentType);
    }

    internal void AttachTo(Wallets wallet)
    {
        WalletAssert.NotNull(wallet, nameof(AttachTo), WalletErrors.WalletRequired);
        WalletAssert.IsSuccess(
            Wallets is null || ReferenceEquals(Wallets, wallet),
            [WalletErrors.TransactionAlreadyAttached]);

        Wallets = wallet;
        WalletsId = wallet.Id;
    }

    public void SuccessPayment(DateTime dateTime, bool isSuccess)
    {
        CommitDateTime = dateTime;
        IsSuccess = isSuccess;
    }

    public void SetBalanceAfter(decimal balanceAfter)
    {
        WalletAssert.IsSuccess(IsSuccess, [WalletErrors.TransactionNotSuccessful]);
        BalanceAfter = balanceAfter;
    }

    public void SetIdempotencyKey(string? idempotencyKey)
    {
        WalletAssert.IsSuccess(
            idempotencyKey is null || idempotencyKey.Length <= IdempotencyKeyMaxLength,
            [WalletErrors.IdempotencyKeyTooLong]);

        IdempotencyKey = idempotencyKey;
    }

    protected override bool SetHasIntegrityChecksum()
    {
        return true;
    }

    public override object GetId()
    {
        return Id;
    }
}
