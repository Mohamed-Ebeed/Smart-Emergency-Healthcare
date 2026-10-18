# Non-Functional Requirements: Smart Emergency Response & Healthcare Coordination Platform

Oct 18, 2026 · @doha

## 1. Introduction and Scope

This document defines the quality attributes and constraints that the Smart Emergency Response & Healthcare Coordination Platform must satisfy, so the team can use it during design, implementation, and testing.

**System context.** The platform, as set out in the approved Project Proposal, is made of a Patient Web App, a Paramedic Web App, and web dashboards for the Operator, Hospital Staff, Doctor, and Admin, all on one backend and database. Its core flow is: emergency request with GPS location, review by the Operator, suggested hospital confirmed by the Operator and sent together with the patient's Emergency Medical Profile, acceptance by Hospital Staff, suggested ambulance confirmed by Hospital Staff, who send it with a Paramedic, live status tracking, patient reception at the hospital, and case closure. AI-assisted triage and smart matching only make suggestions; the Operator and Hospital Staff make the final decision.

The Project Plan fixes the technology: a React frontend, an ASP.NET Core Web API with Entity Framework Core and SQL Server, ASP.NET Core Identity with JWT, SignalR for live updates, Google Login, and map services. The system has six roles: Patient, Operator, Hospital Staff, Paramedic, Doctor, and Admin. The platform does not diagnose patients, and a blood bank is out of scope.

**How requirements are written.** Each requirement has an ID in the form NFR-\<area\>-\<number\> and a priority.

| Priority | Meaning |
| --- | --- |
| Must | Required for the system to be accepted; applies especially to the emergency flow |
| Should | Expected in the delivered system; may be reduced if time runs short |
| Could | Desirable; implemented only if capacity allows |

**Assumptions.** Where the approved Project KPIs define a target (page load time, server response time, error rate, emergency request delivery, family notification delivery, hospital pre-arrival information, mobile support), this document uses the same value. Other numeric targets are proposals for a graduation-project deployment with a small number of concurrent users and test data. They are measurable and can be raised later. Section 10 lists the points to confirm with the team and supervisor.

**Reference documents.** Project Proposal (October 12, 2026), Project Plan, and Project KPIs.

## 2. Performance and Response Time

The emergency flow must feel immediate. Response times below are measured at the server on a normal broadband or 4G connection, at the 95th percentile (95% of requests) unless stated otherwise.

| ID | Operation | Requirement | Priority |
| --- | --- | --- | --- |
| NFR-PERF-01 | Create an emergency request (press emergency button to request stored) | Confirmation shown to the patient within 3 seconds | Must |
| NFR-PERF-02 | Request appears on the Operator dashboard | Within 3 seconds of the button press, without a manual refresh (Project KPI 'Emergency Request Delivery', measured with server-side timestamps) | Must |
| NFR-PERF-03 | Hospital suggestion (shown to the Operator) and ambulance suggestion (shown to Hospital Staff) | Each suggestion returned within 5 seconds | Must |
| NFR-PERF-04 | AI-assisted triage suggestion (type, priority, department, confidence) | Returned within 5 seconds; if it fails or times out, the request continues without it | Must |
| NFR-PERF-05 | Ambulance assignment reaches the Paramedic | Within 3 seconds of Hospital Staff confirmation | Must |
| NFR-PERF-06 | Live ambulance location and status updates | Shown to the patient and the Operator within 5 seconds of each update; location sent at least every 10 seconds while en route | Must |
| NFR-PERF-07 | Notifications to emergency contacts and hospital | Sent within 10 seconds of the triggering event | Should |
| NFR-PERF-08 | Opening the Medical Profile or case details | Within 2 seconds | Should |
| NFR-PERF-09 | Dashboard pages and lists (requests, cases, reports) | Load within 3 seconds with up to 1,000 records; paginated beyond that | Should |
| NFR-PERF-10 | Admin reports and statistics | Generated within 10 seconds | Could |

**Resource limits.**

- **NFR-PERF-11 (Must):** main pages load in under 3 seconds on average (Project KPI 'Page Load Time'), measured with browser DevTools at a fixed network speed; the Patient Web App's initial download stays under 2 MB.
- **NFR-PERF-12 (Must):** non-emergency work, such as reports, must not slow the emergency flow; heavy queries run separately or are limited.
- **NFR-PERF-13 (Must):** ordinary API requests (not AI analysis, smart matching, or report generation, which have their own targets above) respond in under 500 ms for 95% of requests (Project KPI 'Server Response Time').
- **NFR-PERF-14 (Must):** fewer than 1% of requests fail with a server error (HTTP 5xx) per week, measured from server logs (Project KPI 'Error Rate').

## 3. Security and Privacy

The platform handles identity, location, and medical information, so security applies to every role and every screen.

### 3.1 Authentication

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-SEC-01 | Users sign in with email and password or with Google Login. Staff accounts (Operator, Hospital Staff, Paramedic, Doctor, Admin) are created or approved by an Admin, not through open sign-up. | Must |
| NFR-SEC-02 | Passwords are at least 8 characters, stored only as salted hashes (the ASP.NET Core Identity password hasher or stronger), and never logged or returned by any API. | Must |
| NFR-SEC-03 | Sessions use short-lived JWT access tokens (60 minutes or less) and expire after 30 minutes of inactivity for staff dashboards. | Must |
| NFR-SEC-04 | After 5 failed sign-in attempts, the account is locked or throttled for 15 minutes. | Should |
| NFR-SEC-05 | Password reset uses a single-use link that expires within 1 hour. | Should |

### 3.2 Authorization and role-based access control

Every API endpoint enforces role-based authorization with JWT on the server; hiding a button in the interface is never the only control.

| Role | Allowed access | Not allowed |
| --- | --- | --- |
| Patient | Own profile, own requests, tracking of own ambulance | Any other patient's data; any dashboard |
| Operator | All active requests, patient information and location for those requests; reviewing requests and sending them to hospitals | Admin settings; unrelated historical records; assigning ambulances or Paramedics |
| Hospital Staff | Incoming requests for their own hospital, with the medical profile; accepting or rejecting requests; assigning an ambulance and a Paramedic from their own hospital; updating free beds | Other hospitals' patients |
| Paramedic | Assigned jobs only, with the patient's location, emergency type, allergies, and chronic diseases | Other jobs; full medical history |
| Doctor | Cases of their own hospital and the medical profile for those cases; adding notes | Cases outside their hospital |
| Admin | User, ambulance, hospital, and report management | Editing medical notes |

- **NFR-SEC-06 (Must):** access is denied by default; a role gets only the permissions listed above.
- **NFR-SEC-07 (Must):** medical data is shared with a party only while it has an active case with that patient, and access ends when the case is closed.
- **NFR-SEC-08 (Should):** an emergency contact receives status and location updates only, never medical information.

### 3.3 Protection of sensitive medical information

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-SEC-09 | All traffic uses HTTPS (TLS 1.2 or higher); plain HTTP is redirected or refused. | Must |
| NFR-SEC-10 | Medical profile fields (allergies, chronic diseases, medications, previous cases, notes) are encrypted at rest (for example with SQL Server transparent data encryption or column encryption). | Should |
| NFR-SEC-11 | Every read or change of a medical profile or case is written to an audit log with user, role, time, and action; logs cannot be edited by normal users. | Must |
| NFR-SEC-12 | The system collects only data that the emergency flow needs. Blood type is optional, and no data is shared with third parties other than the map and sign-in providers. | Must |
| NFR-SEC-13 | The patient can view and update their own profile and see who accessed it. | Should |
| NFR-SEC-14 | Notifications and logs contain no more medical detail than needed; for example, family messages show location and status only. | Must |
| NFR-SEC-15 | Location is collected only when the patient sends an emergency request or an active case is running, and the patient is told this before first use. | Must |

### 3.4 Application security

- **NFR-SEC-16 (Must):** the system validates and sanitizes all inputs and uses parameterized queries (Entity Framework Core) to prevent injection and cross-site scripting.
- **NFR-SEC-17 (Should):** APIs use rate limiting, and secrets such as JWT signing keys, API keys, and database passwords are kept outside the source code; JWT keys can be rotated without a code change.
- **NFR-SEC-18 (Should):** the implementation is checked against the OWASP Top 10 before final delivery.
- **NFR-SEC-19 (Must):** permissions are tested for every role (allowed and denied endpoints) before each milestone, and pull requests that change authentication, emergency requests, matching, or notifications are checked for data exposure.
- **NFR-SEC-20 (Should):** the Admin reviews user accounts and permissions regularly, at least once per milestone.

## 4. Reliability and Availability

An emergency request must not be lost, and the emergency flow must keep working when secondary features fail.

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-REL-01 | The emergency flow (request, review, hospital selection, hospital acceptance, ambulance assignment, tracking, hospital handover) is available at least 99.5% of the time, measured monthly, excluding announced maintenance. | Must |
| NFR-REL-02 | Planned maintenance is announced in advance and done outside peak hours, taking less than 30 minutes each time. | Should |
| NFR-REL-03 | An accepted emergency request is stored before the patient sees a confirmation, so a server restart cannot lose it. | Must |
| NFR-REL-04 | If the patient's connection drops while sending, the web app keeps the request and retries automatically until it succeeds, and shows its status. | Must |
| NFR-REL-05 | If AI triage, smart matching, map services, or Google Login fail, the request still reaches the Operator and staff continue manually: AI suggestions can be turned off with fixed lists for emergency type and priority; the Operator can pick a hospital from a list filtered by distance, department, and free beds; Hospital Staff can pick an ambulance and a Paramedic from a list filtered by availability and distance; the patient can type an address; and email and password login stays available. | Must |
| NFR-REL-06 | If live GPS tracking is interrupted, the last known location and its time are shown instead of an empty map. | Should |
| NFR-REL-07 | Each request, assignment, and status change is processed once; retries do not create duplicate requests or assignments. | Must |
| NFR-REL-08 | An emergency request that no Operator accepts within 2 minutes is flagged as unattended and highlighted on the dashboard. | Should |
| NFR-REL-09 | The system recovers automatically from a service crash within 5 minutes and shows users a clear error message in the meantime. | Should |
| NFR-REL-10 | Health checks and monitoring alert the team when the service, database, or notification channel is down. | Should |
| NFR-REL-11 | If the SignalR connection fails, clients reconnect automatically and fall back to long polling; if live updates are still unavailable, screens refresh from the API every 5 seconds, and the latest status is reloaded from the database. | Must |
| NFR-REL-12 | Every notification to a patient's emergency contacts has a stored delivery status and delivered-at time; failed notifications are retried, and 100% of notifications are delivered in testing (Project KPI 'Family Notification Delivery'). | Must |
| NFR-REL-13 | In 100% of the defined emergency test scenarios, the hospital receives the patient's emergency information before the ambulance arrives (Project KPI 'Hospital Pre-Arrival Information'). | Must |
| NFR-REL-14 | A request sent to a hospital that does not answer within 2 minutes is flagged as unanswered and highlighted for the Operator, who can send it to the next closest hospital. | Should |

## 5. Usability and Accessibility

Patients are often stressed, hurt, or in a hurry, so the Patient Web App must be usable with no training. Staff dashboards must be fast to scan during busy periods.

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-USE-01 | A patient can send an emergency request in at most 3 steps after signing in: press the emergency button, choose the emergency type, confirm. | Must |
| NFR-USE-02 | The emergency button is visible on the home screen without scrolling and is at least 48 x 48 px (large on mobile). | Must |
| NFR-USE-03 | A first-time patient completes an emergency request in under 60 seconds in usability testing with at least 5 test users. | Should |
| NFR-USE-04 | Emergency types are shown as large labeled buttons with icons, so the patient does not type; free text is optional. | Must |
| NFR-USE-05 | Status messages and errors use plain language and say what to do next (for example, "No connection. We will keep trying."). | Must |
| NFR-USE-06 | A request can be sent even if the medical profile is empty; the profile can be completed later. | Must |
| NFR-USE-07 | The Operator dashboard shows the most urgent requests first, with the type, location, priority, and status visible in the list without opening each case. | Must |
| NFR-USE-08 | The Operator and Hospital Staff can complete frequent actions (send to hospital, accept request, confirm ambulance) in at most 2 clicks from the list, and the Paramedic can update the status in at most 2 clicks. | Should |
| NFR-USE-09 | Priority and status use text labels as well as color, so they are readable without relying on color. | Must |
| NFR-USE-10 | The Operator and Hospital Staff are told clearly that AI triage and matching are only suggestions, and can override them with one action. | Must |
| NFR-USE-11 | Layouts adapt to phone, tablet, and desktop screens (see Section 8). | Must |

**Accessibility.**

- **NFR-USE-12 (Should):** the web apps follow WCAG 2.1 level AA basics: text contrast of at least 4.5:1, keyboard navigation for all actions, visible focus, and text labels for icons and form fields.
- **NFR-USE-13 (Should):** text is at least 16 px on mobile and remains usable when the browser zoom is set to 200%.
- **NFR-USE-14 (Could):** the interface supports Arabic with right-to-left layout in addition to English (not in the approved Proposal, so optional; see Section 10).

## 6. Scalability

The first version is sized for demonstration and testing, but the design must not block growth to a larger city-level deployment.

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-SCA-01 | The system supports at least 200 concurrent users, including 50 active emergency requests, while still meeting the Section 2 response times. | Must |
| NFR-SCA-02 | The system supports at least 1,000 registered patients, 50 ambulances, and 20 hospitals in the test dataset. | Should |
| NFR-SCA-03 | The ASP.NET Core backend is stateless (identity kept in JWT tokens, and SignalR uses a shared backplane if more than one instance runs), so more server instances can be added behind a load balancer without code changes. | Should |
| NFR-SCA-04 | The SQL Server database uses indexes on request status, location, and dates, and lists are paginated, so response times do not grow noticeably as case history grows. | Must |
| NFR-SCA-05 | Live updates use SignalR and support at least 100 connected dashboards and Paramedic Web App clients. | Should |
| NFR-SCA-06 | Load testing shows that, at 2 times the target load in NFR-SCA-01, the emergency flow still works, even if response times are slower. | Could |

## 7. Maintainability

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-MNT-01 | The code is split into modules by area (patients, emergency requests, ambulances, hospitals, notifications, admin), with the frontend, backend, and database layers separated. | Must |
| NFR-MNT-02 | The backend exposes REST APIs documented with Swagger/OpenAPI, and every endpoint has a documented request, response, and error format; API contracts are agreed early so the work of different members fits together. | Must |
| NFR-MNT-03 | Code follows one agreed style guide and is checked by a linter; every task has its own branch and pull request, and the Team Lead reviews every pull request before it is merged into develop. | Must |
| NFR-MNT-04 | AI triage and smart matching are separate components behind a defined interface, so they can be changed or replaced without changing the rest of the system. | Should |
| NFR-MNT-05 | Configuration (keys, URLs, thresholds) is stored in environment settings, not hard-coded. | Must |
| NFR-MNT-06 | Integration tests and API tests cover the critical emergency flow (request, review, hospital selection, hospital acceptance, ambulance assignment, tracking, hospital handover) and are run before each milestone. | Must |
| NFR-MNT-07 | The system writes structured logs with timestamps and request IDs, with levels (info, warning, error), so problems can be traced. | Should |
| NFR-MNT-08 | The repository contains a README with setup steps, an architecture overview, and a database schema description; a new team member can run the system locally in under 1 hour. | Should |
| NFR-MNT-09 | Database changes are made through versioned Entity Framework Core migrations. | Should |

## 8. Compatibility

All user interfaces are web applications, so compatibility is defined by browsers, devices, and external services. No native mobile app is required.

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-CMP-01 | Supported desktop browsers: the latest 2 versions of Google Chrome, Microsoft Edge, Mozilla Firefox, and Safari. | Must |
| NFR-CMP-02 | Supported mobile browsers: Chrome on Android 10 or later, and Safari on iOS 15 or later. | Must |
| NFR-CMP-03 | All pages, including the Operator, Hospital Staff, Doctor, and Admin dashboards, work properly on mobile screens (Project KPI 'Mobile Support': 100% of pages), verified by manual testing on Android and iOS devices and common screen sizes. The layout is responsive from 360 px (phone) upward; the dashboards are optimized for 1280 px and wider and remain usable on a 768 px tablet. | Must |
| NFR-CMP-04 | Features that need the device (GPS location, notifications) ask for browser permission, and show clear instructions if the user refuses. | Must |
| NFR-CMP-05 | If location permission is denied or GPS is inaccurate, the patient can confirm or type an address manually. | Must |
| NFR-CMP-06 | The web apps need no plug-in or installation; the Patient Web App could additionally be installable as a Progressive Web App. | Could |
| NFR-CMP-07 | The ASP.NET Core Web API runs on Windows or Linux (or in a container) and uses SQL Server; the React frontend is served as static files. | Should |
| NFR-CMP-08 | Maps, GPS location, and Google Login are used through their public APIs behind simple interfaces, and API quota is monitored; the system keeps working on its basic functions if one is unavailable (see NFR-REL-05). No government API is required. | Must |
| NFR-CMP-09 | Data exchange between components uses JSON over HTTPS, with dates in ISO 8601 format and UTC time stored in the database. | Should |

## 9. Data Integrity, Backup and Recovery

### 9.1 Data integrity

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-DAT-01 | Related changes (for example, assigning an ambulance and updating the request status) are done in one database transaction, so data is never left half-updated. | Must |
| NFR-DAT-02 | The database enforces integrity with primary keys, foreign keys, and required fields; an ambulance cannot be assigned to two active cases at once. | Must |
| NFR-DAT-03 | Every emergency case keeps a full status history with user and timestamp (for example, created, sent to hospital, accepted by hospital, assigned, en route, picked up, arrived at hospital, closed), and these records cannot be deleted by normal users. | Must |
| NFR-DAT-04 | Input data is validated on both client and server (for example, phone number format, valid coordinates, allowed emergency types). | Must |
| NFR-DAT-05 | Medical profile changes keep the previous value and who changed it, so that a case can show what was known at that time. | Should |
| NFR-DAT-06 | Closed cases are retained for at least 1 year in the test environment; the real retention period depends on the applicable regulations. | Should |

### 9.2 Backup and recovery

| ID | Requirement | Priority |
| --- | --- | --- |
| NFR-DAT-07 | The SQL Server database is backed up automatically every day, with a full backup at least weekly. | Must |
| NFR-DAT-08 | The last 7 daily backups are kept, and backups are stored in a different location from the main database. | Should |
| NFR-DAT-09 | Backups are encrypted, and access to them is limited to Admins. | Should |
| NFR-DAT-10 | Recovery point objective (RPO): at most 24 hours of data loss in a full failure, and at most 1 hour if point-in-time recovery or frequent incremental backups are enabled. | Should |
| NFR-DAT-11 | Recovery time objective (RTO): service restored within 4 hours of a failure, using a documented restore procedure. | Should |
| NFR-DAT-12 | A restore from backup is tested at least once before final delivery, and the result is recorded. | Must |

## 10. Traceability and Open Items

### 10.1 Link to the Proposal and Project KPIs

| Source | Item | Related requirements |
| --- | --- | --- |
| Proposal | One-tap emergency request with GPS location | NFR-PERF-01, NFR-USE-01 to 04, NFR-REL-03, NFR-REL-04, NFR-CMP-04, NFR-CMP-05 |
| Proposal | Operator dashboard and request review | NFR-PERF-02, NFR-USE-07, NFR-USE-08, NFR-REL-08 |
| Proposal | AI-assisted triage (suggestion only) | NFR-PERF-04, NFR-REL-05, NFR-USE-10, NFR-MNT-04 |
| Proposal | Hospital recommendation (confirmed by the Operator) and ambulance recommendation (confirmed by Hospital Staff) | NFR-PERF-03, NFR-REL-05, NFR-REL-14, NFR-USE-10, NFR-MNT-04 |
| Proposal | Paramedic Web App and live tracking with SignalR | NFR-PERF-05, NFR-PERF-06, NFR-REL-06, NFR-REL-11, NFR-CMP-03 |
| Proposal | Hospital Staff and Doctor dashboards | NFR-SEC-06, NFR-SEC-07, NFR-PERF-08, NFR-REL-13 |
| Proposal | Emergency Medical Profile | NFR-SEC-10 to 15, NFR-DAT-05 |
| Proposal | Emergency contact notifications | NFR-PERF-07, NFR-SEC-08, NFR-SEC-14, NFR-REL-12 |
| Proposal | Authentication, Google Login, and role-based access control | NFR-SEC-01 to 07, NFR-SEC-19, NFR-SEC-20 |
| Proposal | Admin dashboard, reports, and status history | NFR-PERF-10, NFR-DAT-03, NFR-DAT-06, NFR-SCA-04 |
| Proposal | No diagnosis, no blood bank | NFR-USE-10, NFR-SEC-12 |
| KPI | Page Load Time | NFR-PERF-11 |
| KPI | Server Response Time | NFR-PERF-13 |
| KPI | Error Rate | NFR-PERF-14 |
| KPI | Emergency Request Delivery | NFR-PERF-02 |
| KPI | Family Notification Delivery | NFR-REL-12 |
| KPI | Hospital Pre-Arrival Information | NFR-REL-13 |
| KPI | Mobile Support | NFR-CMP-02, NFR-CMP-03 |

### 10.2 Open items to confirm

- **Alignment with the Plan:** the technology (ASP.NET Core, SQL Server, React, SignalR, JWT), the six roles, the workflow, and the Project KPI values are used as given. The Plan does not say where the system will be hosted.
- **Hosting and deployment:** the availability and backup requirements (NFR-REL-01, NFR-DAT-07 to NFR-DAT-11) assume a hosted server with automatic backups. Confirm the environment before the Deployment phase (1 to 4 December 2026).
- **Proposed targets:** values that the Project KPIs do not set (99.5% availability, RPO of 24 hours, RTO of 4 hours, 200 concurrent users) are proposals; confirm them with the Team Lead and the supervisor.
- **Retention and regulations:** which health-data rules apply decides the final retention period (NFR-DAT-06) and encryption needs (NFR-SEC-10).
- **Arabic support:** the approved Proposal does not mention Arabic or other languages, so NFR-USE-14 is Could until the team decides.
- **Test data versus real data:** the requirements assume test data for ambulances, hospitals, and GPS during the demo. The Proposal states that the platform does not depend on government APIs.
