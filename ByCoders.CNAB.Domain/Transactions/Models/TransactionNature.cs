namespace ByCoders.CNAB.Domain.Transactions.Models;

/// <summary>
/// 1 - Cash In: Entry of funds
/// 2 - Cash Out: Withdrawal of funds
/// </summary>
public enum TransactionNature : byte
{
    /// <summary>
    /// 1 - Cash In: Entry of funds
    /// </summary>
    CashIn = 1,

    /// <summary>
    /// 2 - Cash Out: Withdrawal of funds
    /// </summary>
    CashOut
}
