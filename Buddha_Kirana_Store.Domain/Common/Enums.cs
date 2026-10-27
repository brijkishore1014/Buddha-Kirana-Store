namespace Buddha_Kirana_Store.Domain;

public enum PaymentStatus { Paid = 1, Partial = 2, Unpaid = 3 }
public enum BillingMode { PerItem = 1, LumpSum = 2 }
public enum AdjustmentReason { Damaged = 1, Expired = 2, Theft = 3, AuditCorrection = 4, OpeningStock = 5 }
public enum PartyType { Customer = 1, Supplier = 2 }
public enum PaymentMode { Cash = 1, UPI = 2, BankTransfer = 3 }
public enum StockTransactionType { Purchase = 1, Sale = 2, PurchaseReturn = 3, SalesReturn = 4, Adjustment = 5 }
public enum DocumentType { SalesInvoice = 1, SalesReturn = 2, PurchaseReturn = 3 }
public enum CashFlowDirection { In = 1, Out = 2 }
public enum CashBookSource
{
    SalesReceipt = 1,
    CustomerPayment = 2,
    SalesReturnRefund = 3,
    PurchasePayment = 4,
    SupplierPayment = 5,
    PurchaseReturnRefund = 6,
    Expense = 7
}
