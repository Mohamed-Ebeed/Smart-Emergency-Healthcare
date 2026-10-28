# Sequence and State Diagrams

Smart Emergency Response & Healthcare Coordination Platform

## 1. Overview

This document illustrates the runtime interactions between system components using **Sequence Diagrams** and defines the complete lifecycle of an emergency request using a **State Diagram**.

All diagrams are strictly aligned with the approved **Use Cases (`UC-01` to `UC-18`)**, **User Stories (`US-PAT-02`, `US-OPR`, `US-HOS`, `US-AMB`, `US-DOC`)**, **Functional Requirements (`FR-10` to `FR-40`)**, and **Non-Functional Requirements (`NFR-PERF`, `NFR-REL`, `NFR-DAT`)**.

### Main System Participants & Components
- **Actors:** Patient, Operator, Hospital Staff, Paramedic, Doctor, Emergency Contacts.
- **Frontend Clients:** React Web Apps (Patient App, Paramedic App, Operator/Hospital/Doctor Dashboards).
- **Backend Core:** ASP.NET Core Web API (`EmergencyController`, `HospitalController`, `ParamedicController`, `DoctorController`).
- **Real-Time Engine:** ASP.NET Core SignalR Hub (`EmergencyHub`).
- **Decision Support Services:** AI Triage Service & Smart Matching Engine (Hospital & Ambulance Recommendation).
- **External & Infrastructure:** SQL Server Database (EF Core), Maps/GPS Service, Notification Service.

---

## 2. Sequence Diagrams

### 2.1. Scenario 1: Emergency Request Submission (`UC-04`, `FR-10` to `FR-14`, `US-PAT-01`)

This diagram covers the flow when a patient presses the emergency button, captures GPS location (or enters a manual address fallback), stores the request in SQL Server before confirming (`NFR-REL-03`), triggers AI triage preparation, notifies emergency contacts (`NFR-REL-12`), and pushes the request live to the Operator dashboard within 3 seconds (`NFR-PERF-01`, `NFR-PERF-02`).

```mermaid
sequenceDiagram
    autonumber
    actor Patient
    participant PatApp as Patient Web App (React)
    participant Maps as Maps / GPS API
    participant API as ASP.NET Core Web API
    participant DB as SQL Server (EF Core)
    participant AI as AI Triage Service
    participant Notif as Notification Service
    participant Hub as SignalR Hub
    actor Contacts as Emergency Contacts
    actor Operator as Operator Dashboard

    Patient->>PatApp: Press Emergency Button & select Emergency Type
    PatApp->>Maps: Request current GPS coordinates
    alt GPS Permission Granted & Available
        Maps-->>PatApp: Return (Latitude, Longitude)
    else GPS Off or Denied (Fallback - FR-12 / NFR-CMP-05)
        Patient->>PatApp: Enter address manually
    end

    PatApp->>API: POST /api/emergency-requests (Type, Description, Location)
    activate API
    
    API->>DB: Begin Transaction: Insert Request (Status = "Submitted") & CaseHistory
    DB-->>API: Request Saved (RequestId)
    
    par Async AI Triage Pre-calculation (NFR-PERF-04)
        API->>AI: Analyze request description & type (Timeout = 5s)
        alt AI Responds in <= 5s
            AI-->>API: Return Suggestion (Type, Priority, Dept, Confidence)
            API->>DB: Save AI Triage Suggestion
        else AI Times Out or Fails (NFR-REL-05)
            AI-->>API: Timeout / Fallback to manual classification
        end
    and Notify Emergency Contacts (FR-13 / US-PAT-06)
        API->>Notif: Trigger alert (Patient Name, Location, Status="Submitted")
        Notif-->>Contacts: Send SMS/Notification (No medical details - NFR-SEC-08)
        Notif->>DB: Log Notification Delivery Status (NFR-REL-12)
    and Real-Time Broadcast to Operator (FR-14 / NFR-PERF-02)
        API->>Hub: Broadcast NewRequestReceived(RequestId, Priority, Location, AISuggestion)
        Hub-->>Operator: Push new request to Operator Queue (Displays as "Submitted" / Waiting)
    end

    API-->>PatApp: 201 Created (RequestId, Status = "Submitted")
    deactivate API
    PatApp-->>Patient: Display Request Number & Live Status Screen (< 3s)
```

---

### 2.2. Scenario 2: Request Review, Hospital Selection & Ambulance Assignment (`UC-06`, `UC-07`, `UC-09`, `FR-17` to `FR-27`)

This diagram illustrates the human-in-the-loop review by the Operator (`US-OPR-02`, `US-OPR-03`), smart hospital recommendation and rejection loop (`US-OPR-05`, `US-OPR-06`), Hospital Staff acceptance (`US-HOS-03`), and closest free ambulance + paramedic assignment (`US-HOS-04`, `US-AMB-02`).

```mermaid
sequenceDiagram
    autonumber
    actor Operator
    participant OprApp as Operator Dashboard
    participant API as ASP.NET Core Web API
    participant Match as Smart Matching Engine
    participant DB as SQL Server (EF Core)
    participant Hub as SignalR Hub
    actor HosStaff as Hospital Staff Dashboard
    actor Paramedic as Paramedic Web App
    actor Patient as Patient Web App

    Operator->>OprApp: Open Submitted Request
    OprApp->>API: GET /api/emergency-requests/{id} (Includes Medical Profile & AI Suggestion)
    API->>DB: Fetch Request + Medical Profile + Write Audit Log (NFR-SEC-11)
    DB-->>API: Return Case Details
    API-->>OprApp: Display Case, Location, Medical Profile & AI Suggestion

    Operator->>OprApp: Confirm or Override Type & Priority (US-OPR-03)
    OprApp->>API: PUT /api/emergency-requests/{id}/confirm (ConfirmedType, ConfirmedPriority)
    API->>DB: Update Status = "Confirmed", Store Operator Decision & History
    API->>Match: GetRecommendedHospitals(Location, RequiredDept, ExcludedHospitals=[])
    Match->>DB: Query Hospitals with RequiredDept, FreeBeds > 0, Order by GPS Distance
    DB-->>Match: Ranked Hospitals List
    Match-->>API: Return Suggested Hospitals (< 5s - NFR-PERF-03)
    API-->>OprApp: Show Confirmed Status & Ranked Hospital Suggestions
    API->>Hub: Notify Patient Status = "Confirmed"
    Hub-->>Patient: Update UI Status to "Confirmed"

    loop Until a Hospital Accepts (US-OPR-05, US-OPR-06)
        Operator->>OprApp: Select & Confirm Hospital
        OprApp->>API: POST /api/emergency-requests/{id}/send-to-hospital (HospitalId)
        API->>DB: Record Hospital Dispatch Attempt
        API->>Hub: Send IncomingEmergencyRequest to Hospital Group
        Hub-->>HosStaff: Alert Incoming Request (Priority, Dept, Location, Medical Profile)

        alt Hospital Rejects or Unanswered > 2 mins (FR-21 / NFR-REL-14)
            HosStaff->>API: POST /api/hospital/requests/{id}/reject (Reason)
            API->>DB: Log Rejection & Add HospitalId to Excluded List
            API->>Match: GetNextClosestHospital(Location, Dept, ExcludedHospitals)
            Match-->>API: Return Next Closest Hospital
            API->>Hub: Notify Operator of Rejection + New Suggestion
            Hub-->>OprApp: Alert Rejection & Suggest Next Closest Hospital
        else Hospital Accepts Request (UC-09 / US-HOS-03)
            HosStaff->>API: POST /api/hospital/requests/{id}/accept
            API->>DB: Link Case to Hospital & Reserve Bed
            API->>Match: GetClosestFreeAmbulance(HospitalId, PatientLocation)
            Match->>DB: Query Hospital's Ambulances where Status = "Available"
            DB-->>Match: Closest Free Ambulance & Paramedic
            Match-->>API: Return Suggested Ambulance
            API-->>HosStaff: Display Suggested Ambulance for Confirmation
        end
    end

    HosStaff->>API: POST /api/hospital/requests/{id}/assign-ambulance (AmbulanceId, ParamedicId)
    activate API
    API->>DB: Transaction: Set Request Status = "Ambulance Assigned", Ambulance = "Busy", Log History (NFR-DAT-01)
    DB-->>API: Committed
    API->>Hub: Broadcast JobAssigned to Paramedic (< 3s - NFR-PERF-05)
    Hub-->>Paramedic: Push Job Details (Location, Emergency Type, Allergies/Conditions)
    API->>Hub: Broadcast StatusChanged("Ambulance Assigned") to Patient & Operator
    Hub-->>Patient: Update Status to "Ambulance Assigned"
    Hub-->>OprApp: Update Live Dashboard
    deactivate API
```

---

### 2.3. Scenario 3: Live Location Tracking & Status Updates (`UC-05`, `UC-12`, `UC-13`, `FR-15`, `FR-16`, `FR-31` to `FR-37`)

This diagram shows the Paramedic executing the job (`US-AMB-03`, `US-AMB-04`), streaming live GPS coordinates via SignalR (`NFR-PERF-06`), handling weak connections (`FR-36`, `NFR-REL-11`), and sending non-medical status updates to Emergency Contacts (`FR-35`).

```mermaid
sequenceDiagram
    autonumber
    actor Paramedic
    participant AmbApp as Paramedic Web App
    participant Hub as SignalR Hub
    participant API as ASP.NET Core Web API
    participant DB as SQL Server (EF Core)
    participant Notif as Notification Service
    actor Patient as Patient Web App
    actor Operator as Operator Dashboard
    actor HosStaff as Hospital Staff Dashboard
    actor Contacts as Emergency Contacts

    Note over Paramedic, AmbApp: Job received directly from Hospital (US-AMB-02)

    loop For Each Field Status: "On the Way" -> "Arrived" -> "Picked Up" -> "Going to Hospital" -> "At Hospital"
        Paramedic->>AmbApp: Tap Status Update Button (One-Tap - US-AMB-03)
        alt Online Connection Available
            AmbApp->>API: PUT /api/paramedic/cases/{id}/status (NewStatus, CurrentGPS)
        else Weak Signal / Offline (FR-36 / NFR-REL-04)
            AmbApp->>AmbApp: Queue Status Update Locally with Timestamp
            AmbApp->>API: Auto-Retry PUT /api/paramedic/cases/{id}/status when back online
        end

        activate API
        API->>DB: Save New Status & Append to CaseStatusHistory (FR-37 / NFR-DAT-03)
        DB-->>API: Saved
        
        par Broadcast Real-Time Status (FR-34 / NFR-PERF-06)
            API->>Hub: Broadcast CaseStatusUpdated(CaseId, NewStatus, ETA, Location)
            Hub-->>Patient: Update Status Timeline & Map (< 5s)
            Hub-->>Operator: Update Active Cases Dashboard
            Hub-->>HosStaff: Update Incoming Patient Pre-Arrival Board (NFR-REL-13)
        and Notify Emergency Contacts (FR-35 / US-PAT-06)
            API->>Notif: Send Short Status & Location Update
            Notif-->>Contacts: Deliver Update (No medical info - NFR-SEC-14)
            Notif->>DB: Update Delivery Log (NFR-REL-12)
        end
        API-->>AmbApp: 200 OK
        deactivate API

        opt While Status is "On the Way" or "Going to Hospital" (US-AMB-04 / NFR-PERF-06)
            loop Every 10 Seconds
                AmbApp->>Hub: SendAmbulanceLocation(CaseId, Latitude, Longitude)
                Hub-->>Patient: Update Ambulance Marker & ETA on Map
                Hub-->>Operator: Update Map Marker
            end
        end
    end
```

---

### 2.4. Scenario 4: Hospital Reception, Doctor Assessment & Case Closure (`UC-10`, `UC-14`, `UC-15`, `FR-28` to `FR-30`, `FR-38` to `FR-40`)

This diagram covers hospital handover once the ambulance reaches the hospital (`At Hospital`), assigning the case to a Doctor (`US-HOS-06`), viewing the medical profile and recording treatment notes (`US-DOC-01` to `04`), closing the case (`US-HOS-05`), releasing the ambulance, and revoking temporary medical data access (`NFR-SEC-07`).

```mermaid
sequenceDiagram
    autonumber
    actor HosStaff as Hospital Staff
    participant HosApp as Hospital Dashboard
    participant API as ASP.NET Core Web API
    participant DB as SQL Server (EF Core)
    participant Hub as SignalR Hub
    actor Doctor as Doctor Dashboard
    actor Patient as Patient Web App
    actor Operator as Operator Dashboard

    Note over HosStaff, HosApp: Ambulance Status is "At Hospital" (Pre-arrival info already visible - NFR-REL-13)

    HosStaff->>HosApp: Mark Patient as Received / Admitted & Assign to Doctor (UC-10, US-HOS-05, US-HOS-06)
    HosApp->>API: POST /api/hospital/cases/{id}/receive (AssignedDoctorId, DepartmentId)
    activate API
    API->>DB: Record Patient Arrival, Assign Doctor, Update Free Beds Count (UC-11), Log History
    API->>Hub: NotifyAssignedDoctor(DoctorId, CaseId)
    Hub-->>Doctor: Show Case in Doctor's Incoming List (US-DOC-01)
    API-->>HosApp: 200 OK (Patient Received)
    deactivate API

    Doctor->>API: GET /api/doctor/cases/{id} (UC-14 / US-DOC-02)
    activate API
    API->>DB: Fetch Case Info + Medical Profile (if exists - FR-39) + Write Audit Log (NFR-SEC-11)
    DB-->>API: Return Full Case & Medical Profile
    API-->>Doctor: Display Unified Case View (Description, Type, Priority, Medical Profile, Timeline)
    deactivate API

    Doctor->>API: POST /api/doctor/cases/{id}/notes (Assessment, TreatmentNotes, MarkDoctorWorkFinished) (UC-15 / US-DOC-03, 04)
    activate API
    API->>DB: Save Clinical Notes (DoctorName, Timestamp) & Mark Doctor Assessment Complete
    API->>Hub: NotifyHospitalStaff(CaseId, DoctorWorkComplete)
    Hub-->>HosApp: Show Doctor Assessment Complete
    API-->>Doctor: 200 OK (Notes Saved)
    deactivate API

    HosStaff->>HosApp: Complete & Close Emergency Case (UC-10 / US-HOS-05)
    HosApp->>API: PUT /api/hospital/cases/{id}/close
    activate API
    API->>DB: Transaction: Set Case Status = "Completed", Set Ambulance Status = "Available", Log Final Timestamp
    Note right of DB: Active case medical profile access ends upon closure (NFR-SEC-07)
    DB-->>API: Committed
    API->>Hub: Broadcast CaseClosed(CaseId, Status="Completed")
    Hub-->>Patient: Update Status to "Completed" & Stop GPS Tracking
    Hub-->>Operator: Remove from Active Queue / Move to Completed History
    API-->>HosApp: 200 OK (Case Closed)
    deactivate API
```

---

## 3. Emergency Request State Diagram

This State Diagram models the complete lifecycle of an **Emergency Request** entity in the database, strictly adhering to the status names defined in **`US-PAT-02`**, **`US-PAT-08`**, **`US-OPR-06`**, **`US-OPR-08`**, and **`US-OPR-10`**.

### 3.1. State Diagram (`Mermaid stateDiagram-v2`)

```mermaid
stateDiagram-v2
    direction TB

    [*] --> Submitted : Patient presses Emergency Button\n(UC-04 / US-PAT-01)

    state "Submitted\n(Displayed as 'Waiting' in Operator Queue - UC-04)" as Submitted
    state "Confirmed\n(Type & Priority Verified by Operator)" as Confirmed
    state "Hospital Dispatch Pending\n(Sent to Selected Hospital - UC-07)" as PendingHospital
    state "Ambulance Assigned\n(Hospital Accepted & Confirmed Ambulance - UC-09)" as AmbulanceAssigned
    state "On the Way\n(Paramedic En Route to Patient - UC-12)" as OnTheWay
    state "Arrived\n(Paramedic Reached Patient Location - UC-13)" as Arrived
    state "Picked Up\n(Patient Onboard Ambulance - UC-13)" as PickedUp
    state "Going to Hospital\n(En Route to Accepting Hospital - UC-13)" as GoingToHospital
    state "At Hospital\n(Arrived at Hospital Handover - UC-13)" as AtHospital
    state "Completed\n(Patient Received, Treated & Case Closed - UC-10)" as Completed
    state "Cancelled\n(Cancelled Before Ambulance Assignment - US-PAT-08)" as Cancelled
    state "Invalid\n(Closed by Operator as False/Duplicate - US-OPR-10)" as Invalid

    Submitted --> Confirmed : Operator confirms/overrides AI triage\n(UC-06 / US-OPR-03)
    Submitted --> Cancelled : Patient cancels request\n(US-PAT-08)
    Submitted --> Invalid : Operator marks as invalid with reason\n(US-OPR-10)

    Confirmed --> PendingHospital : Operator sends request to suggested hospital\n(UC-07 / US-OPR-05)
    Confirmed --> Cancelled : Patient cancels before ambulance assignment\n(US-PAT-08)
    Confirmed --> Invalid : Operator closes invalid request\n(US-OPR-10)

    PendingHospital --> PendingHospital : Hospital rejects OR timeout (>2 min)\nOperator sends to next closest hospital (UC-07 / US-OPR-06)
    PendingHospital --> AmbulanceAssigned : Hospital accepts request & confirms closest free ambulance\n(UC-09 / US-HOS-03, US-HOS-04)
    PendingHospital --> Cancelled : Patient cancels before ambulance assignment\n(US-PAT-08 / US-OPR-08)

    AmbulanceAssigned --> OnTheWay : Paramedic receives job & starts moving\n(UC-12, UC-13 / US-AMB-03)
    AmbulanceAssigned --> PendingHospital : Hospital becomes unavailable\nOperator re-routes to another hospital (US-OPR-09)

    OnTheWay --> Arrived : Paramedic taps "Arrived" at patient location\n(UC-13 / US-AMB-03)
    Arrived --> PickedUp : Paramedic taps "Picked Up" after stabilizing patient\n(UC-13 / US-AMB-03)
    PickedUp --> GoingToHospital : Paramedic taps "Going to Hospital"\n(UC-13 / US-AMB-03)
    GoingToHospital --> AtHospital : Paramedic taps "At Hospital" upon arrival\n(UC-13 / US-AMB-03)

    AtHospital --> Completed : Hospital Staff marks patient received,\nDoctor saves notes, Staff closes case (UC-10, UC-15 / US-HOS-05)

    Completed --> [*]
    Cancelled --> [*]
    Invalid --> [*]
```

---

### 3.2. State Transitions & Consistency Reference Table

To ensure 100% traceability across the ERD, APIs, Use Cases, and User Stories, the table below maps every state transition, triggering actor, and business rule:

| # | Current State | Event / Trigger | Next State | Triggered By | Guard Condition / Business Rule | Related Requirement |
|---|---|---|---|---|---|---|
| 1 | *(Initial)* | Submit Emergency Request | `Submitted` *(Waiting)* | Patient | GPS captured or manual address entered; saved in DB first. | `UC-04`, `US-PAT-01`, `FR-14` |
| 2 | `Submitted` | Review & Confirm Priority | `Confirmed` | Operator | Operator confirms or overrides AI triage suggestion. | `UC-06`, `US-OPR-03`, `FR-18` |
| 3 | `Submitted` / `Confirmed` / `PendingHospital` | Cancel Request | `Cancelled` | Patient / Operator | Allowed **only** before status reaches `Ambulance Assigned`. | `US-PAT-08`, `US-OPR-08` |
| 4 | `Submitted` / `Confirmed` | Mark as Invalid | `Invalid` | Operator | Reason must be recorded (false or duplicate request). | `US-OPR-10` |
| 5 | `Confirmed` | Send Request to Hospital | `PendingHospital` | Operator | Hospital has required department and free beds. | `UC-07`, `US-OPR-05`, `FR-20` |
| 6 | `PendingHospital` | Reject Request / Timeout | `PendingHospital` | Hospital Staff / System | Rejecting hospital is excluded; next closest GPS hospital suggested. | `UC-07`, `US-OPR-06`, `FR-21`, `NFR-REL-14` |
| 7 | `PendingHospital` | Accept & Confirm Ambulance | `Ambulance Assigned` | Hospital Staff | Hospital has free bed + free paramedic; ambulance marked `Busy`. | `UC-09`, `US-HOS-03`, `US-HOS-04`, `FR-27` |
| 8 | `Ambulance Assigned` | Update Status to En Route | `On the Way` | Paramedic | Paramedic taps status button; live GPS sharing starts. | `UC-12`, `UC-13`, `US-AMB-03`, `FR-33` |
| 9 | `On the Way` | Update Status at Scene | `Arrived` | Paramedic | Paramedic reaches patient coordinates; contacts notified. | `UC-13`, `US-AMB-03`, `FR-33`, `FR-35` |
| 10 | `Arrived` | Update Status on Boarding | `Picked Up` | Paramedic | Patient is onboard the ambulance. | `UC-13`, `US-AMB-03`, `FR-33` |
| 11 | `Picked Up` | Start Transit to Hospital | `Going to Hospital` | Paramedic | Ambulance navigates back to its home hospital. | `UC-13`, `US-AMB-01`, `US-AMB-03` |
| 12 | `Going to Hospital` | Arrive at Hospital | `At Hospital` | Paramedic | Ambulance arrives at hospital entrance. | `UC-13`, `US-AMB-03`, `FR-33` |
| 13 | `At Hospital` | Receive Patient & Close Case | `Completed` | Hospital Staff *(with Doctor notes)* | Patient admitted, Doctor notes saved, Ambulance freed (`Available`), temporary medical access revoked. | `UC-10`, `UC-15`, `US-HOS-05`, `NFR-SEC-07` |
