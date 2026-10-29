using SRP.src.SrpLab;
using SrpLab;


// 1. SupportTicket

var priorityClassifier = new TicketPriorityClassifier();
var ticket = new SupportTicket("TCK-1001", "Outage in system", "Users cannot login", DateTimeOffset.UtcNow, priorityClassifier);
var slaCalc = new SlaCalculator();
var publicFormatter = new PublicReplyFormatter(slaCalc);
var internalFormatter = new InternalEscalationFormatter(slaCalc);

Console.WriteLine($"Ticket Priority: {ticket.Priority}");
Console.WriteLine($"Public Reply: {publicFormatter.DraftPublicReply(ticket, "Agent Sarah").Replace('\n', ' ')}");
Console.WriteLine($"Internal Blurb: {internalFormatter.DraftInternalEscalationBlurb(ticket)}");
Console.WriteLine();

// 2. WardBoard

var acuityScorer = new AcuityScorer();
var pagerService = new WardPagerService();
var wardBoard = new WardBoard(acuityScorer, pagerService);
wardBoard.AssignBed(101, "PAT-77", 130, 88);

var handoffFormatter = new NurseHandoffFormatter();
var censusExporter = new CensusCsvExporter();
Console.WriteLine($"Handoff Note: {handoffFormatter.BuildHandoffNote(wardBoard, 101)}");
Console.WriteLine($"Pager Log Count: {pagerService.DrainPagerLog().Count}");
Console.WriteLine($"CSV Line Count: {censusExporter.ExportCensusCsv(wardBoard).Split('\n').Length}");
Console.WriteLine();

// 3. WarehousePickList

var pickList = new WarehousePickList();
pickList.AddNeed("SKU-MOUSE", "A1", 12, 5, 10);
pickList.AddNeed("SKU-KEYBOARD", "B2", 4, 3, 2);

var pathOptimizer = new PickingPathOptimizer();
var scriptFormatter = new PickerScriptFormatter(pathOptimizer);
var wmsExporter = new WmsXmlExporter();

Console.WriteLine($"Picker Script Steps: {scriptFormatter.BuildPickerScript(pickList).Split('\n').Length}");
Console.WriteLine($"WMS XML Output: {wmsExporter.ExportWmsXmlBatch(pickList, "BATCH-001")}");
Console.WriteLine();

// 4. AppointmentDesk

var hoursPolicy = new BusinessHoursPolicy(new TimeOnly(9, 0), new TimeOnly(17, 0));
var appointmentDesk = new AppointmentDesk(hoursPolicy, slotMinutes: 30);
var testSlot = new DateTimeOffset(2026, 10, 5, 10, 0, 0, TimeSpan.FromHours(2));

if (appointmentDesk.TryBook(testSlot))
{
    var smsFormatter = new SmsReminderFormatter();
    Console.WriteLine("Slot booked successfully!");
    Console.WriteLine($"SMS Reminder: {smsFormatter.FormatSmsReminder(testSlot, "01000000000")}");
}
Console.WriteLine();

// 5. CheckoutBasket

var basket = new CheckoutBasket();
basket.AddLine("SKU-LAPTOP", 1200m, 1);
basket.ApplyCouponText("SAVE10");

var discountCalc = new DiscountCalculator();
var wrapCalc = new GiftWrapFeeCalculator();
var grandTotal = basket.GrandTotal(discountCalc, wrapCalc);
var paymentGateway = new PaymentGatewayStub();

Console.WriteLine($"Basket SubTotal: {basket.SubTotal():C}");
Console.WriteLine($"Grand Total: {grandTotal:C}");
Console.WriteLine($"Payment Auth Code: {paymentGateway.AuthorizePayment(basket, grandTotal, "4321")}");
Console.WriteLine();

// 6. GradeBook

var gradeBook = new GradeBook();
gradeBook.Record("STD-101", 88m);
gradeBook.Record("STD-101", 92m);

var gradingPolicy = new GradingPolicy();
var transcriptFormatter = new TranscriptFormatter(gradingPolicy);

var avg = gradeBook.Average("STD-101");
Console.WriteLine($"Average Grade: {avg}");
Console.WriteLine($"Letter Grade: {gradingPolicy.CalculateLetter(avg)}");
Console.WriteLine();

// 7. SubscriptionBilling


var invoiceGenerator = new InvoiceNumberGenerator();
var periodStart = DateOnly.FromDateTime(DateTime.Today);
var periodEnd = periodStart.AddMonths(1);

var subscription = new SubscriptionBilling("CUST-9901", 100m, periodStart, periodEnd);
subscription.RegisterFailedPayment();

var dunningFormatter = new DunningEmailFormatter(invoiceGenerator);
var ledgerExporter = new LedgerJournalExporter(invoiceGenerator);

var activeFrom = periodStart.AddDays(5);
var proratedAmount = subscription.Prorate(activeFrom);

Console.WriteLine($"Customer: {subscription.CustomerId}");
Console.WriteLine($"Monthly Price: {subscription.MonthlyPrice:C}, Prorated Amount: {proratedAmount:C}");
Console.WriteLine($"Failed Payments: {subscription.FailedPayments}");
Console.WriteLine();
Console.WriteLine("Draft Dunning Email:");
Console.WriteLine(dunningFormatter.DraftDunningEmail(subscription, "Asmaa", DateOnly.FromDateTime(DateTime.Today)));
Console.WriteLine("Ledger Journal Export:");
Console.WriteLine(ledgerExporter.ExportLedgerJournalLine(subscription, activeFrom));
Console.WriteLine();

// CourseEnrollmentDesk Testing


var courseDesk = new CourseEnrollmentDesk("CS101", capacity: 2, tuition: 1500m);
var status1 = courseDesk.Register("asmaa@example.com");
var status2 = courseDesk.Register("ahmed@example.com");
var status3 = courseDesk.Register("mona@example.com"); 
Console.WriteLine($"Asmaa Status: {status1}"); 
Console.WriteLine($"Ahmed Status: {status2}"); 
Console.WriteLine($"Mona Status: {status3}"); 

Console.WriteLine();
var welcomeFormatter = new CourseWelcomePacketFormatter();
Console.WriteLine("Welcome Packet for Asmaa:");
Console.WriteLine(welcomeFormatter.DraftWelcomePacket(courseDesk, "asmaa@example.com", "Asmaa Diab"));
Console.WriteLine("Welcome Packet for Mona:");
Console.WriteLine(welcomeFormatter.DraftWelcomePacket(courseDesk, "mona@example.com", "Mona"));
var invoiceCalculator = new CourseInvoiceCalculator();
Console.WriteLine("Invoice Line for Asmaa (Seated):");
Console.WriteLine(invoiceCalculator.GenerateTuitionInvoiceLine(courseDesk, "asmaa@example.com"));
Console.WriteLine("Invoice Line for Mona (Waitlisted):");
Console.WriteLine(invoiceCalculator.GenerateTuitionInvoiceLine(courseDesk, "mona@example.com"));


// KitchenTicket Testing

var kitchenTicket = new KitchenTicket();
kitchenTicket.AddItem("Cheese Pizza", new[] { "Flour", "Cheese", "Tomato Sauce" }, prepMinutes: 12);
kitchenTicket.AddItem("Shrimp Salad", new[] { "Shrimp", "Lettuce", "Olive Oil" }, prepMinutes: 8);
var allergenDetector = new AllergenDetector();
var timingCalculator = new KitchenTimingCalculator();
var printer = new ThermalTicketPrinter(allergenDetector, timingCalculator);
var laneRouting = new ExpoLaneRouting(allergenDetector, timingCalculator);
var allergens = allergenDetector.DetectAllergens(kitchenTicket.Items.Select(i => i.Ingredients));
Console.WriteLine($"Detected Allergens: {string.Join(", ", allergens)}");
var eta = timingCalculator.EstimatedReadyMinutes(kitchenTicket, allergenDetector, openStations: 2);
Console.WriteLine($"Estimated Ready Time (2 stations): {eta} minutes");
var laneHint = laneRouting.ExpoLaneHint(kitchenTicket);
Console.WriteLine($"Expo Lane Hint: {laneHint}");
Console.WriteLine();
Console.WriteLine("Thermal Ticket Output:");
Console.WriteLine(printer.RenderThermalTicket(kitchenTicket, orderNumber: 101));
Console.WriteLine();


// LoanDesk Testing

var loan = new LoanDesk(requestedAmount: 60000m, creditScore: 720, employmentMonths: 18, hasCollateral: true);
var complianceChecker = new LoanComplianceChecker();
var letterFormatter = new LoanDecisionLetterFormatter(complianceChecker);
var analyticsExporter = new LoanAnalyticsExporter();
Console.WriteLine($"Risk Score: {loan.RiskScore():0.00}");
Console.WriteLine($"Is Eligible: {loan.IsEligible()}");
Console.WriteLine();
var requiredDocs = complianceChecker.RequiredDocuments(loan);
Console.WriteLine("Required Documents:");
foreach (var doc in requiredDocs)
{
    Console.WriteLine($" - {doc}");
}
Console.WriteLine();
Console.WriteLine("Decision Letter:");
Console.WriteLine(letterFormatter.FormatDecisionLetter(loan, "Asmaa Diab"));
Console.WriteLine("Underwriter CSV Export:");
Console.WriteLine(analyticsExporter.ExportUnderwriterCsvRow(loan, applicationId: "APP-2026-001"));

