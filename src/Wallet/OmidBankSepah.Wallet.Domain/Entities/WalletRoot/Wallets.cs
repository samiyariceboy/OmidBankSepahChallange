using Taday.Corelibrary.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using OmidBankSepah.Wallet.Domain.Constraint;
using OmidBankSepah.Wallet.Domain.Entities.WalletRoot.Aggregates;

namespace OmidBankSepah.Wallet.Domain.Entities.WalletRoot;
public class Wallets : AggregateRoot, ICloneable, IValidatableObject
{
    private Wallets()
    {
    }

    private Wallets(decimal balance, string customerId)
    {
        Id = ImmediateFillId();
        Balance = balance;
        CustomerId = customerId;
    }

    public decimal Balance { get; private set; }
    public decimal CreditBalance { get; private set; }
    public decimal CreditBalanceRemaining { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public decimal WithdrawableBalance { get; private set; }

    private readonly List<WalletTransaction> _transactions = [];
    public IReadOnlyList<WalletTransaction> Transactions => _transactions.AsReadOnly();

    protected override string[] IntegrityProperties => [];

    public static Wallets Create(decimal balance, string customerId)
    {
        WalletAssert.IsSuccess(
            !string.IsNullOrWhiteSpace(customerId),
            [WalletErrors.CustomerIdInvalid]
        );

        return new Wallets(balance, customerId);
    }

    public void Credit(decimal amount)
    {
        Balance += amount;
    }

    public void Debit(decimal amount)
    {
        Balance -= amount;
    }

    public void CreditCredit(decimal amount)
    {
        CreditBalanceRemaining += amount;
    }

    public void DebitCredit(decimal amount)
    {
        CreditBalanceRemaining -= amount;
    }

    public void AssignCredit(decimal amount)
    {
        CreditBalance += amount;
        CreditBalanceRemaining += amount;
    }

    public void AddTransaction(WalletTransaction transaction)
    {
        WalletAssert.NotNull(transaction, nameof(AddTransaction), WalletErrors.TransactionRequired);
        
        WalletAssert.IsSuccess(
            !_transactions.Contains(transaction),
            [WalletErrors.TransactionAlreadyAttached]);

        transaction.AttachTo(this);
        _transactions.Add(transaction);
    }

    protected override bool SetHasIntegrityChecksum()
    {
        return false;
    }

    public override object GetId()
    {
        return Id;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }

    public object Clone()
    {
        return new();
    }
}
