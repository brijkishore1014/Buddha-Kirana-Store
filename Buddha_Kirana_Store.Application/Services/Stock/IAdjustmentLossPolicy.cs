using Buddha_Kirana_Store.Domain;

namespace Buddha_Kirana_Store.Services;

/// <summary>Decides whether a stock adjustment is a financial loss for the shop.</summary>
public interface IAdjustmentLossPolicy
{
    bool IsCompanyLoss(AdjustmentReason reason, decimal quantityChanged);
}

internal sealed class DefaultAdjustmentLossPolicy : IAdjustmentLossPolicy
{
    // Damaged, Expired and Theft are strictly logged as company loss when stock goes down.
    public bool IsCompanyLoss(AdjustmentReason reason, decimal quantityChanged) =>
        quantityChanged < 0 &&
        reason is AdjustmentReason.Damaged or AdjustmentReason.Expired or AdjustmentReason.Theft;
}
