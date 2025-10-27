namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// Transaction Types
/// Represent the type of transaction
/// 1 - Debit: Cash In
/// 2 - BankSlip: Cash Out
/// 3 - Funding: Cash Out
/// 4 - Credit: Cash In
/// 5 - Loan Receipt: Cash In
/// 6 - Sales: Cash In
/// 7 - TED Receipt: Cash In
/// 8 - DOC Receipt: Cash In
/// 9 - Rent: Cash Out
/// </summary>
public enum TransactionTypes
{
    /// <summary>
    /// 1 - Debit: Cash In
    /// </summary>
    Debit = 1,
    /// <summary>
    /// 2 - BankSlip: Cash Out
    /// </summary>
    BankSlip,
    /// <summary>
    /// 3 - Funding: Cash Out
    /// </summary>
    Funding,
    /// <summary>
    /// 4 - Credit: Cash In
    /// </summary>
    Credit,
    /// <summary>
    /// 5 - Loan Receipt: Cash In
    /// </summary>
    LoanReceipt,
    /// <summary>
    /// 6 - Sales: Cash In
    /// </summary>
    Sales,
    /// <summary>
    /// 7 - TED Receipt: Cash In
    /// </summary>
    TEDReceipt,
    /// <summary>
    /// 8 - DOC Receipt: Cash In
    /// </summary>
    DOCReceipt,
    /// <summary>
    /// 9 - Rent: Cash Out
    /// </summary>
    Rent
}
