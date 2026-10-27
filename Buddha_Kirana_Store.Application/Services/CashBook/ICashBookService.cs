namespace Buddha_Kirana_Store.Services;

public interface ICashBookService
{
    /// <summary>Opening / in / out / closing per payment mode, plus one page of entries.</summary>
    Task<CashBookReport> GetReportAsync(CashBookQuery query, int page, int pageSize);
}
