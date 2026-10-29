## AppointmentDesk

### Identified Responsibilities:
1. **Appointment Desk & Slot Booking (`AppointmentDesk`)**: Managing booked slots, evaluating availability, and searching for the next open appointment timeslot.
2. **Business Hours Policy (`BusinessHoursPolicy`)**: Determining operating days and valid daily working hours for clinic appointments.
3. **ICS Calendar Export (`IcsCalendarExporter`)**: Generating iCalendar (.ics) format files for calendar integration.
4. **SMS Reminder Formatting (`SmsReminderFormatter`)**: Formatting text message appointment reminders for patients.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Clinic operations changes working days, Marketing/Patient Experience changes SMS wording, and IT changes calendar ICS spec formatting.
* **Risk of Unintended Side Effects**: Changing SMS message content or ICS export layout should not impact clinic slot availability calculations.


## CheckoutBasket

### Identified Responsibilities:
1. **Cart State & Subtotal Math (`CheckoutBasket`)**: Storing line items, quantity validations, and calculating base cart subtotals.
2. **Coupon Parser & Discount Rules (`DiscountCalculator`)**: Parsing promotional text/coupon codes and applying percentage or flat discounts.
3. **Gift Packaging Policy (`GiftWrapFeeCalculator`)**: Managing gift wrapping options and associated service fees.
4. **Gift Card Message Formatting (`GiftCardFormatter`)**: Formatting customer-facing gift message card copy.
5. **Payment Gateway Authorization (`PaymentGatewayStub`)**: Simulating payment authorization payload generation and security hash minting.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Marketing changes coupon codes and promo rules, Logistics updates gift-wrapping fees, Customer Experience modifies card formatting, and Payments/Security alters gateway auth logic.
* **Complex Coupling**: Parsing strings for coupons or generating payment auth stubs inside the shopping cart makes unit testing cart totals fragile.


## CourseEnrollmentDesk

### Identified Responsibilities:
1. **Enrollment & Waitlist Core State (`CourseEnrollmentDesk`)**: Managing student registration, capacity checks, and waitlist tracking.
2. **Student Onboarding Document Generator (`CourseWelcomePacketFormatter`)**: Drafting student welcome letters and instructions formatted in Markdown for marketing/onboarding.
3. **Financial Billing & VAT Exporter (`CourseInvoiceCalculator`)**: Calculating tuition fees, 14% VAT taxes, and generating line-item invoice records.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Marketing changes onboarding instructions, Finance changes tax rates/invoice layouts, and Academic Ops changes capacity rules. Separating them prevents financial or display updates from breaking enrollment logic.


## GradeBook

### Identified Responsibilities:
1. **Grade Storage & Aggregation (`GradeBook`)**: Recording individual scores per student and calculating score averages.
2. **Academic Grading Policy (`GradingPolicy`)**: Translating numerical averages into letter grades and determining honor roll eligibility.
3. **Student Transcript Formatter (`TranscriptFormatter`)**: Drafting official student transcript documents in standard text layout.
4. **Grade Book CSV Exporter (`GradeBookCsvExporter`)**: Exporting class grade rosters and academic metrics into CSV format.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: The Faculty Senate changes letter grade boundaries and honor roll rules, the Registrar updates transcript presentation layouts, and IT/Analytics modifies CSV export formats.
* **Separation of Concerns**: Calculating statistical averages should be completely independent of document rendering and export schemes.

## KitchenTicket

### Identified Responsibilities:
1. **Kitchen Order State (`KitchenTicket`)**: Storing order line items, ingredients, and base preparation times.
2. **Allergen Detection Rules (`AllergenDetector`)**: Identifying regulatory dietary allergens (dairy, gluten, nuts, shellfish) from item ingredients.
3. **Prep Time & ETA Estimation (`KitchenTimingCalculator`)**: Calculating ready times based on kitchen stations and allergy protocol delays.
4. **Thermal Printer Formatting & Expo Lane Routing (`ThermalTicketPrinter` / `ExpoLaneRouting`)**: Formatting 32-column printer layouts and determining kitchen line routing tags.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Health inspectors/compliance update allergen definitions, Kitchen Management changes cooking ETA heuristics, POS vendors change printer line widths, and Expo Leads change lane routing rules.
* **Separation of Concerns**: Printing formatted output should not be tightly coupled with allergy inspection or timing logic.


## LoanDesk

### Identified Responsibilities:
1. **Loan Application State & Risk Assessment (`LoanDesk` / `LoanRiskEvaluator`)**: Holding loan request attributes, calculating underwriting risk scores, and evaluating baseline loan eligibility.
2. **Regulatory Compliance & Document Checklist (`LoanComplianceChecker`)**: Determining mandatory legal and regulatory verification documents based on loan terms.
3. **Legal Decision Letter Generation (`LoanDecisionLetterFormatter`)**: Formatting decision/pre-approval and rejection letters for applicants.
4. **Underwriter Analytics CSV Export (`LoanAnalyticsExporter`)**: Formatting loan risk and eligibility data into CSV records for risk analysis tools.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Risk Committees alter risk scoring formulas, Compliance Officers update mandatory document requirements, Legal/Communications edit letter templates, and Analytics teams change CSV export formats.
* **Tight Coupling**: Modifying decision letter prose or regulatory documents should never impact underwriting risk calculations.


## SubscriptionBilling

### Identified Responsibilities:
1. **Subscription State & Proration Math (`SubscriptionBilling` / `ProrationCalculator`)**: Managing subscription period dates, tracking payment failure counts, and calculating prorated billing amounts.
2. **Invoice Sequence Generator (`InvoiceNumberGenerator`)**: Minting sequential fiscal invoice numbers with year/month prefixes.
3. **Dunning & Collections Email Formatter (`DunningEmailFormatter`)**: Formatting collection reminder email copy, severity levels, and notices.
4. **Accounting Ledger Exporter (`LedgerJournalExporter`)**: Generating general ledger journal lines for accounting exports.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Finance alters proration and fiscal numbering, Collections/Legal updates dunning email wording and escalation severity, and Accounting modifies ledger export formats.
* **Side-Effect Danger**: Generating invoice numbers as a side effect while constructing an email body (`NextInvoiceNumber()` inside `DunningEmail`) creates subtle bugs during email preview or retry operations.


## SupportTicket

### Identified Responsibilities:
1. **Ticket Core State (`SupportTicket`)**: Managing customer ticket metadata, message history, and opened timestamp.
2. **Text Priority Classifier (`TicketPriorityClassifier`)**: Analyzing ticket subject and body text using keyword heuristics to categorize urgency levels (P1/P2/P3).
3. **SLA Deadlines & Breach Evaluation (`SlaCalculator`)**: Calculating operational SLA deadlines and detecting time breaches.
4. **Customer Reply Formatting (`PublicReplyFormatter`)**: Drafting public-facing response messages tailored to priority levels.
5. **Internal Escalation Blurbs (`InternalEscalationFormatter`)**: Generating technical escalation summaries for internal teams.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Customer Experience (CX) controls reply text, Support Operations handles keyword priority rules, IT/Engineering configures SLA target hours, and Ops Leads define escalation blurb formats.
* **Separation of Concerns**: Recalculating priority from raw text should not be coupled with SLA clock checks or customer message templates.


## WardBoard

### Identified Responsibilities:
1. **Ward Bed State Management (`WardBoard`)**: Tracking bed assignments, patient identifiers, and vitals severity scores.
2. **Clinical Acuity Scoring (`AcuityScorer`)**: Evaluating clinical vital signs (heart rate and SpO2) to compute patient acuity scores based on medical triage rules.
3. **Pager Notification Dispatch (`WardPagerService`)**: Triggering and logging emergency code alert pings for medical staff when patient acuity crosses thresholds.
4. **Nurse Handoff Note Formatter (`NurseHandoffFormatter`)**: Generating narrative shift handoff notes for nursing personnel with acuity status labels.
5. **Ward Census CSV Exporter (`CensusCsvExporter`)**: Formatting ward occupancy and acuity metrics into CSV records for hospital administration reports.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Medical Directors update clinical acuity scoring formulas, Nursing Management modifies handoff note formats, Hospital IT/Ops changes pager dispatch logic, and Admin Analysts update census export layouts.
* **Separation of Concerns**: Calculating clinical vital scores or generating handoff text should not trigger side effects in pager logs or couple with file export formats.


## WarehousePickList

### Identified Responsibilities:
1. **Pick List State & Allocation Policy (`WarehousePickList` / `StockAllocator`)**: Maintaining requested items and evaluating inventory stock allocation vs required quantities.
2. **Warehouse Walking Path Optimization (`PickingPathOptimizer`)**: Determining the optimal physical sequence (aisles and bins) for pickers navigating the floor.
3. **Picker Handheld UX Scripting (`PickerScriptFormatter`)**: Formatting step-by-step instructions and shortage warnings for handheld devices.
4. **WMS XML Integration Export (`WmsXmlExporter`)**: Formatting allocated items into XML batch structures for integration contracts with Warehouse Management Systems.

### Why Having Them Together Is a Problem:
* **Multiple Stakeholders**: Operations Managers change allocation and backorder policies, Logistics Leads update walking path heuristics for new layouts, UI/UX teams change handheld picker scripts, and IT Integration teams update WMS XML contracts.
* **Separation of Concerns**: Calculating physical walking paths or generating XML export data should not be coupled with handheld UI text or stock allocation math.