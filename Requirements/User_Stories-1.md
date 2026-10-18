# User Stories

Smart Emergency Response & Healthcare Coordination Platform

## 1. Overview

This document lists the user stories for the six roles of the platform: patient, emergency center operator, paramedic, hospital staff, doctor and admin. Emergency contacts are not a role; they only receive notifications.

Each story follows the form "As a [role], I want [goal], so that [benefit]" and has short acceptance criteria that the testing phase can check.

- ID: US-<role>-<number>, for example US-PAT-03. Roles: ALL, PAT, OPR, AMB, HOS, DOC, ADM.
- Priority: Must = part of the MVP, Should = planned if time allows, Could = optional improvement.
- Note on AI and matching: the system only suggests triage results and hospital or ambulance matches. A person always confirms them.
- Flow: the emergency center sends the request to a hospital. If the hospital accepts, it sends its own ambulance and paramedic. If it rejects, the operator sends the request to the next closest hospital.

## 2. All Users and Patient

### All users

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-ALL-01 | As a user, I want to register and log in with my email and password, so that I can use the features of my role. | Valid credentials open the dashboard for my role; invalid credentials show a clear error. | Must |
| US-ALL-02 | As a user, I want to sign in with Google, so that I do not need another password. | Google sign-in creates or links my account; if Google is unavailable, email login still works. | Should |
| US-ALL-03 | As a user, I want to see only the screens and actions allowed for my role, so that I am not confused or exposed to data that is not mine. | Menus show role items only; the API rejects calls outside the role with an authorization error. | Must |
| US-ALL-04 | As a user, I want to log out, so that nobody else can use my session. | Logging out ends the session; an expired session asks me to log in again. | Should |

### Patient

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-PAT-01 | As a patient, I want to press an emergency button and choose the emergency type, so that help starts quickly with my location attached. | I choose a type from the list (chest pain, stroke symptoms, accident, breathing difficulty, severe injury, pregnancy emergency, burn, severe bleeding, other) and can add a short description; my GPS location is captured automatically and I can correct it; I get a request number and the status Submitted. | Must |
| US-PAT-02 | As a patient, I want to see the status of my request, so that I know what is happening. | Status moves through Submitted, Confirmed, Ambulance Assigned, On the Way, Arrived, Picked Up, Going to Hospital, At Hospital, Completed. | Must |
| US-PAT-03 | As a patient, I want to follow the ambulance on a map with its estimated arrival time, so that I know when help will arrive. | The ambulance position updates without refreshing the page. | Must |
| US-PAT-04 | As a patient, I want to keep an Emergency Medical Profile (name, age, allergies, chronic diseases, current medications and an optional blood type), so that responders have it when it matters. | I can create and edit the profile; only people involved in my active emergency can see it. | Must |
| US-PAT-05 | As a patient, I want to add emergency contacts, so that my family is informed. | I can add, edit and remove contacts with name, relationship and contact details. | Must |
| US-PAT-06 | As a patient, I want my emergency contacts to be notified when I submit a request and as it progresses, so that someone I trust knows what is happening. | Contacts receive a short notification with my location and the ambulance status at each main step; it contains no medical details. | Must |
| US-PAT-07 | As a patient, I want to be notified when my request status changes, so that I do not have to keep checking. | A notification appears for each status change. | Should |
| US-PAT-08 | As a patient, I want to cancel a request I created by mistake, so that no ambulance is sent without need. | Cancel is allowed until an ambulance is assigned; the operator sees the cancellation. | Should |
| US-PAT-09 | As a patient, I want to see my past requests, so that I can review what happened. | History lists date, type and final status of each request. | Should |

## 3. Emergency Center Operator

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-OPR-01 | As an operator, I want to see incoming requests ordered by priority and time, so that I handle the most urgent first. | New requests appear in the queue without a page reload. | Must |
| US-OPR-02 | As an operator, I want to see the AI triage suggestion (emergency type, priority, suggested department, confidence) next to the patient's description, so that I can classify faster. | The suggestion is labelled as a suggestion and shows its confidence level; it is not a diagnosis. | Must |
| US-OPR-03 | As an operator, I want to confirm or change the type and priority, so that the final classification is a human decision. | The request is not sent to a hospital until I confirm; my choice and the original suggestion are both stored. | Must |
| US-OPR-04 | As an operator, I want to see the patient's location on a map, their Emergency Medical Profile and their contact details, so that I understand the case and can call the patient or their family. | Location, profile, the patient's phone number and emergency contacts appear on the same screen as the request. | Must |
| US-OPR-05 | As an operator, I want to see suggested hospitals ranked by department availability, free beds and distance, and send the request to one, so that the patient goes to a hospital that can treat them. | Nothing is sent until I confirm; the chosen hospital gets the request with the patient's info and location. | Must |
| US-OPR-06 | As an operator, I want to know right away if the hospital rejects the request, so that I can send it to another hospital. | A rejection shows on my dashboard; the system suggests the next closest hospital by GPS, without the one that rejected. | Must |
| US-OPR-07 | As an operator, I want a live dashboard of all active cases, so that I can monitor the whole operation. | The dashboard shows status, hospital and assigned ambulance for every active case. | Must |
| US-OPR-08 | As an operator, I want to close a case that the hospital cannot complete (for example a cancelled request), so that records stay accurate. | Closing sets the final status and stops tracking. | Should |
| US-OPR-09 | As an operator, I want to send the request to another hospital if the first one becomes unavailable, so that the patient is not left waiting. | The system offers a new hospital suggestion that excludes the first one. | Should |
| US-OPR-10 | As an operator, I want to mark a request as invalid with a reason, so that false or duplicate requests do not use resources. | The request is closed with the stored reason. | Should |

## 4. Paramedic

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-AMB-01 | As a paramedic, I want to see the job my hospital sent me with the patient location, distance and emergency type, so that I know where to go. | The job shows the address, the map, the distance and the emergency type; after pickup I can navigate back to my hospital. | Must |
| US-AMB-02 | As a paramedic, I want to get the job straight from my hospital, so that I can move right away. | The job appears without accept or reject; the hospital only sends it to a free paramedic. | Must |
| US-AMB-03 | As a paramedic, I want to update the case status (on the way, arrived, picked up, going to hospital, at hospital) with one tap, so that everyone sees the progress. | The new status is visible to the patient, operator and hospital staff without a refresh. | Must |
| US-AMB-04 | As a paramedic, I want my location to be shared automatically while I am on a case, so that I do not have to report it. | Location updates are sent at regular intervals and stop when the case is completed. | Must |
| US-AMB-05 | As a paramedic, I want to see the patient's allergies, conditions and medications, so that I can act safely on the way. | The Emergency Medical Profile is visible for my active case only. | Must |
| US-AMB-06 | As a paramedic, I want to set my ambulance as available, busy or offline, so that my hospital does not send me when I cannot go. | Only available ambulances appear in my hospital's suggestions. | Should |

## 5. Hospital Staff

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-HOS-01 | As hospital staff, I want to update department availability and bed capacity, so that patients are only sent where there is room. | The values are used in hospital matching straight away. | Must |
| US-HOS-02 | As hospital staff, I want to see incoming requests with priority, needed department and the patient's location, so that we can decide and prepare. | The incoming list updates without a refresh. | Must |
| US-HOS-03 | As hospital staff, I want to accept or reject an incoming request, so that the operator knows whether we can take the patient. | We accept only if we have a free bed and a free paramedic; a rejection goes back to the operator with a new hospital suggestion. | Must |
| US-HOS-04 | As hospital staff, I want to see our closest free ambulance suggested and send it with a paramedic, so that help leaves right away. | Nothing is sent until we confirm; the paramedic gets the job and the status becomes Ambulance Assigned. | Must |
| US-HOS-05 | As hospital staff, I want to record the patient's arrival and admission status and complete the emergency case, so that the case is closed correctly. | The status appears in the case timeline; completing the case sets its status to Completed. | Must |
| US-HOS-06 | As hospital staff, I want to assign an incoming case to a doctor, so that the patient is seen quickly. | The doctor sees the case in their list. | Should |

## 6. Doctor

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-DOC-01 | As a doctor, I want to see the cases assigned to me or my department, so that I know who is coming. | The list shows priority and status and updates without a refresh. | Must |
| US-DOC-02 | As a doctor, I want to view a case with the description, confirmed type and priority, medical history, ambulance information and status timeline, so that I have the full picture before I see the patient. | All of these appear on one screen. | Must |
| US-DOC-03 | As a doctor, I want to add my initial assessment and treatment notes to a case, so that what was done is recorded. | Notes are saved with my name and the time and are visible to authorized staff only. | Must |
| US-DOC-04 | As a doctor, I want to mark my part of the case as finished, so that the hospital can complete the admission. | The case status shows that the doctor's work is complete. | Should |

## 7. Admin

| ID | Story | Acceptance criteria | Priority |
|---|---|---|---|
| US-ADM-01 | As an admin, I want to create, edit and disable user accounts and assign roles, so that only the right people have access. | A disabled user cannot log in; role changes apply at the next login. | Must |
| US-ADM-02 | As an admin, I want to manage hospitals, departments and ambulances, and link each ambulance to its hospital, so that the data used for matching is correct. | Changes are available to matching straight away. | Must |
| US-ADM-03 | As an admin, I want reports on case volume, response times and ambulance and hospital use, so that I can see how the operation performs. | Reports can be filtered by date range. | Must |
| US-ADM-04 | As an admin, I want a dashboard with the overall activity, so that I can see the situation at a glance. | The dashboard shows counts of users, ambulances, hospitals, and active and completed emergencies. | Should |
| US-ADM-05 | As an admin, I want an audit log of important actions (confirmations, role changes, status changes), so that I can trace what happened. | Each entry shows who did what and when. | Should |
