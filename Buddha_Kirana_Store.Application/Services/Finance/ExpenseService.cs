using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class ExpenseService(IUnitOfWorkFactory uowFactory, ICashBookRecorder cashBook) : IExpenseService
{
    public async Task<List<ShopExpense>> GetExpensesAsync()
    {
        await using var uow = uowFactory.Create();
        return await uow.Expenses.GetAllAsync();
    }

    public async Task AddExpenseAsync(ShopExpense expense)
    {
        if (expense.Amount <= 0)
            throw new DomainException("Expense amount must be greater than zero.");

        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        uow.Expenses.Add(expense);
        await uow.SaveChangesAsync(); // gives the expense its id for the cash book reference

        cashBook.Record(uow, CashFlowDirection.Out, expense.ExpenseDate, expense.PaymentMode, expense.Amount,
            CashBookSource.Expense, expense.Id, $"{expense.Category}: {expense.Title}");

        await uow.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task DeleteExpenseAsync(int expenseId)
    {
        await using var uow = uowFactory.Create();
        await using var tx = await uow.BeginTransactionAsync();

        var expense = await uow.Expenses.GetByIdAsync(expenseId);
        if (expense is null)
            return;

        // The money did not leave the shop after all, so take the cash book entry out too.
        foreach (var entry in await uow.CashBook.GetBySourceAsync(CashBookSource.Expense, expenseId))
            uow.CashBook.Remove(entry);

        uow.Expenses.Remove(expense);
        await uow.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
