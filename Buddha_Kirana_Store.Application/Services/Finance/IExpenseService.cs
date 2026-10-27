using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

public interface IExpenseService
{
    Task<List<ShopExpense>> GetExpensesAsync();
    Task AddExpenseAsync(ShopExpense expense);
    Task DeleteExpenseAsync(int expenseId);
}
