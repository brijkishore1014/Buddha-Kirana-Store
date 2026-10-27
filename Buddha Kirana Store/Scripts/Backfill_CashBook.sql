-- OPTIONAL one-time script (PostgreSQL).
--
-- Fills the new "CashBookEntries" table with money movements that were recorded BEFORE the
-- cash book existed (old sales receipts, payments, return refunds and expenses).
--
-- When to run: once, after the app has started with the new migration (so the table exists),
-- and only if you already have real data you want in the cash book.
-- Run it in pgAdmin > Query Tool on the buddha_kirana database.
-- Safe to run twice: it does nothing if the table already has rows.
--
-- Codes used below
--   Direction: In = 1, Out = 2
--   Source:    SalesReceipt = 1, CustomerPayment = 2, SalesReturnRefund = 3, PurchasePayment = 4,
--              SupplierPayment = 5, PurchaseReturnRefund = 6, Expense = 7

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM "CashBookEntries") THEN
        RAISE NOTICE 'CashBookEntries already has rows - nothing to do.';
        RETURN;
    END IF;

    -- 1. Money received from customers (sale receipts and Udhaar recoveries)
    INSERT INTO "CashBookEntries" ("EntryDate", "PaymentMode", "Direction", "Amount", "Source", "SourceId", "Description")
    SELECT "TransactionDate",
           "PaymentMode",
           1,
           "Amount",
           CASE WHEN "ReferenceInvoiceId" IS NULL THEN 2 ELSE 1 END,
           CASE WHEN "ReferenceInvoiceId" IS NULL THEN "Id" ELSE "ReferenceInvoiceId" END,
           LEFT(COALESCE("Remarks", 'Customer receipt'), 200)
    FROM "PaymentTransactions"
    WHERE "PartyType" = 1 AND "Amount" > 0;

    -- 2. Money paid to suppliers (purchase payments and payouts)
    INSERT INTO "CashBookEntries" ("EntryDate", "PaymentMode", "Direction", "Amount", "Source", "SourceId", "Description")
    SELECT "TransactionDate",
           "PaymentMode",
           2,
           "Amount",
           CASE WHEN "ReferenceInvoiceId" IS NULL THEN 5 ELSE 4 END,
           CASE WHEN "ReferenceInvoiceId" IS NULL THEN "Id" ELSE "ReferenceInvoiceId" END,
           LEFT(COALESCE("Remarks", 'Supplier payment'), 200)
    FROM "PaymentTransactions"
    WHERE "PartyType" = 2 AND "Amount" > 0;

    -- 3. Refunds paid to customers on sales returns
    INSERT INTO "CashBookEntries" ("EntryDate", "PaymentMode", "Direction", "Amount", "Source", "SourceId", "Description")
    SELECT "ReturnDate",
           "RefundMode",
           2,
           "RefundPaidAmount",
           3,
           "Id",
           LEFT('Refund for sales return ' || "ReturnNumber", 200)
    FROM "SalesReturns"
    WHERE "RefundPaidAmount" > 0;

    -- 4. Refunds received from suppliers on purchase returns
    INSERT INTO "CashBookEntries" ("EntryDate", "PaymentMode", "Direction", "Amount", "Source", "SourceId", "Description")
    SELECT "ReturnDate",
           "RefundMode",
           1,
           "RefundReceivedAmount",
           6,
           "Id",
           LEFT('Refund from supplier for return ' || "ReturnNumber", 200)
    FROM "PurchaseReturns"
    WHERE "RefundReceivedAmount" > 0;

    -- 5. Shop expenses
    INSERT INTO "CashBookEntries" ("EntryDate", "PaymentMode", "Direction", "Amount", "Source", "SourceId", "Description")
    SELECT "ExpenseDate",
           "PaymentMode",
           2,
           "Amount",
           7,
           "Id",
           LEFT("Category" || ': ' || "Title", 200)
    FROM "ShopExpenses"
    WHERE "Amount" > 0;

    RAISE NOTICE 'Cash book back-filled: % entries.', (SELECT COUNT(*) FROM "CashBookEntries");
END
$$;
