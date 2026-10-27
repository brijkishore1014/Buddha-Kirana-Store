using Buddha_Kirana_Store.Abstractions;
using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

internal sealed class CashBookService(IUnitOfWorkFactory uowFactory) : ICashBookService
{
    public async Task<CashBookReport> GetReportAsync(CashBookQuery query, int page, int pageSize)
    {
        (page, pageSize) = PagingRules.Normalize(page, pageSize);

        await using var uow = uowFactory.Create();

        var fromDate = query.From?.Date;
        var toExclusive = query.To?.Date.AddDays(1);

        // Opening = everything before the first day of the range (nothing if the range is open-ended).
        var opening = fromDate is null
            ? new List<CashBookModeTotal>()
            : await uow.CashBook.GetModeTotalsAsync(null, fromDate, query.Mode);
        var movement = await uow.CashBook.GetModeTotalsAsync(fromDate, toExclusive, query.Mode);

        var modes = query.Mode is { } only ? new List<PaymentMode> { only } : Enum.GetValues<PaymentMode>().ToList();
        var balances = modes.Select(mode =>
        {
            var before = opening.FirstOrDefault(t => t.Mode == mode);
            var during = movement.FirstOrDefault(t => t.Mode == mode);
            return new CashBookBalance(
                mode,
                Opening: before is null ? 0 : before.MoneyIn - before.MoneyOut,
                MoneyIn: during?.MoneyIn ?? 0,
                MoneyOut: during?.MoneyOut ?? 0);
        }).ToList();

        var total = await uow.CashBook.CountAsync(query);
        var entries = await uow.CashBook.SearchAsync(query, (page - 1) * pageSize, pageSize);

        return new CashBookReport(balances, new PagedResult<CashBookEntry>(entries, total, page, pageSize));
    }
}
