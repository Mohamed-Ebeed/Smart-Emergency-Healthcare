# Stakeholder Analysis

Smart Emergency Response & Healthcare Coordination Platform

## 1. Purpose and Approach

This document identifies everyone who uses, operates, depends on or is affected by the Smart Emergency Response & Healthcare Coordination Platform, and records what each of them needs from it. It is the first input to the user stories, use cases and requirements in this phase.

Stakeholders were identified by following one emergency from start to finish: a patient creates a request, the Emergency Center operator reviews it, an ambulance is assigned, a hospital prepares, and a doctor takes over. Everyone touched along that path is listed, together with the people who manage the system and the external services it depends on.

The needs below come from the project proposal and from how emergency coordination normally works. They have not been validated with real emergency staff, so the supervisors' review is the checkpoint for them.

## 2. Stakeholder Register

Eleven stakeholders were identified: six system roles, five supporting parties.

| ID | Stakeholder | Type | Role in the system | Main needs |
|---|---|---|---|---|
| S1 | Patient | Primary user | Submits emergency requests, keeps a medical profile and emergency contacts, tracks the ambulance | Fast request with little typing, location shared automatically, clear status, private medical data |
| S2 | Emergency Center Operator | Primary user | Reviews every request, checks the AI suggestion, confirms type, priority, ambulance and hospital, contacts patients and families when needed | Priority-ordered queue, AI suggestion with confidence, one-screen view of the case, quick confirmation |
| S3 | Ambulance crew / paramedic | Primary user | Accepts assignments, updates case status, shares location | Clear destination, one-tap status updates, patient allergies and conditions on the way |
| S4 | Hospital staff | Primary user | Manages bed and department capacity, prepares for incoming patients, completes the case | Early warning with arrival time, accurate capacity, simple acknowledge action |
| S5 | Doctor | Primary user | Views the patient case, records the initial assessment and treatment notes | Quick access to the case and medical profile, notes saved with the case |
| S6 | System administrator | Primary user | Manages users, roles, hospitals, ambulances and reports | Control over accounts and roles, reliable data, usable reports |
| S7 | Emergency contacts | Secondary | Receive alerts when the patient submits a request and as the case progresses | Short, clear message; no account needed; no medical details exposed |
| S8 | Hospital and ambulance service management | Secondary | Use reports to monitor response times and workload | Trustworthy figures on response time, case volume and utilisation |
| S9 | Project supervisors and evaluators | Secondary | Review and assess each deliverable | Consistent documents, clear scope, a working demo |
| S10 | Project team | Internal | Designs, builds, tests and documents the system | Clear requirements, defined ownership, realistic scope |
| S11 | Google Login and Maps providers | External | Provide sign-in, maps, location and distance data | Use within their terms and API limits |

## 3. Influence and Interest

The operator, supervisors and project team have the most influence on whether the project succeeds, so they get the closest attention. Users with less influence but high interest are kept informed, because the system is built for them.

| Stakeholder | Influence | Interest | Engagement strategy |
|---|---|---|---|
| S2 Emergency Center Operator | High | High | Manage closely: their workflow drives the core design |
| S9 Supervisors and evaluators | High | High | Manage closely: review every deliverable before its deadline |
| S10 Project team | High | High | Manage closely: weekly sync, GitHub issues and pull requests |
| S1 Patient | Medium | High | Keep informed: represented through user stories and the demo scenario |
| S3 Paramedic | Medium | High | Keep informed: status flow and responsive screens checked in UI/UX review |
| S4 Hospital staff | Medium | High | Keep informed: capacity and incoming-patient screens reviewed |
| S6 Administrator | Medium | High | Keep informed: user and role management reviewed |
| S5 Doctor | Medium | Medium | Keep informed: case view and notes screen reviewed |
| S8 Service management | Medium | Medium | Keep satisfied: reports designed around response time and workload |
| S7 Emergency contacts | Low | Medium | Monitor: keep the notification short and free of medical data |
| S11 Google and Maps providers | Low | Low | Monitor: stay within API limits and keep a fallback if a service is down |

## 4. Communication and Feedback Plan

The team has no direct access to real emergency staff, so stakeholder needs are checked through the project's own review steps.

| Stakeholder group | How their input is gathered | When |
|---|---|---|
| Supervisors and evaluators (S9) | Each phase package is uploaded to GitHub and reviewed before its deadline | 16 Oct, 6 Nov, 30 Nov, 4 Dec 2026 |
| Project team (S10) | Weekly sync; tasks tracked as GitHub issues and pull requests | Weekly and per task |
| Operator, paramedic, hospital staff, doctor, admin (S2 to S6) | Needs written as user stories and use cases, then checked again on the wireframes and mockups | Phase 2 and Phase 4 |
| Patient and emergency contacts (S1, S7) | Needs written as user stories; checked by running the full emergency scenario in testing | Phase 2 and Phase 6 |
| Service management (S8) | Report contents agreed from the response-time and workload figures in the requirements | Phase 2 |
| Google and Maps providers (S11) | API limits and terms checked before implementation; fallbacks defined in the risk plan | Phase 3 and Phase 5 |

If the scope changes, this list is revisited and the change is recorded as a GitHub issue.
