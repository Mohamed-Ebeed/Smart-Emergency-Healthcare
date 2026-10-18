# Functional Requirements

Smart Emergency Response & Healthcare Coordination Platform

What the system should do. Each requirement comes from a use case in the Use Cases file.

## 1. Accounts and Login

| ID | Requirement | Use case |
|---|---|---|
| FR-01 | Patient can register and log in with email and password. | UC-01 |
| FR-02 | Patient can log in with his Google account. | UC-01 |
| FR-03 | System shows an error when the email or password is wrong. | UC-01 |
| FR-04 | Each user sees only the pages of his role. | UC-16 |

## 2. Medical Profile and Emergency Contacts

| ID | Requirement | Use case |
|---|---|---|
| FR-05 | Patient can add and edit his allergies, diseases and medicines. | UC-02 |
| FR-06 | The medical profile is optional, the patient can send a request without it. | UC-02 |
| FR-07 | Only staff can see the patient's medical profile. | UC-02 |
| FR-08 | Patient can add emergency contacts (name, phone, relation). | UC-03 |
| FR-09 | System checks the phone number and asks the patient to fix it if it's wrong. | UC-03 |

## 3. Emergency Request and Tracking

| ID | Requirement | Use case |
|---|---|---|
| FR-10 | Patient can send an emergency request with one button and pick the emergency type. | UC-04 |
| FR-11 | System takes the patient's GPS location and sends it with the request. | UC-04 |
| FR-12 | If GPS is off, the patient can write his address himself. | UC-04 |
| FR-13 | System sends a message to the emergency contacts when the patient sends a request. | UC-04 |
| FR-14 | New requests show on the operator's dashboard as "Waiting". | UC-04 |
| FR-15 | Patient can see the ambulance on the map and the arrival time. | UC-05 |
| FR-16 | The tracking page updates by itself. If live updates stop, it refreshes every few seconds. | UC-05 |

## 4. Operator

| ID | Requirement | Use case |
|---|---|---|
| FR-17 | AI suggests the emergency type and priority. | UC-06 |
| FR-18 | Operator can agree with the AI suggestion or change it. If AI is off, he picks from a list. | UC-06 |
| FR-19 | System suggests the closest hospital with the right department and free beds. | UC-07 |
| FR-20 | Operator can send the request to the hospital. | UC-07 |
| FR-21 | If the hospital rejects, system finds the next closest hospital by GPS. | UC-07, UC-09 |
| FR-22 | Operator can see all open cases and their status. | UC-08 |
| FR-23 | Operator can see the phone numbers of the patient and his emergency contacts. | UC-08 |

## 5. Hospital Staff

| ID | Requirement | Use case |
|---|---|---|
| FR-24 | Hospital staff can see the patient's info, the emergency type and the location. | UC-09 |
| FR-25 | Hospital staff can accept or reject the request. | UC-09 |
| FR-26 | System suggests the hospital's closest free ambulance. | UC-09 |
| FR-27 | Hospital staff can confirm the ambulance and send it with a paramedic. | UC-09 |
| FR-28 | Hospital staff can mark the patient as received. | UC-10 |
| FR-29 | Hospital staff can close the case when treatment is done. | UC-10 |
| FR-30 | Hospital staff can update the free beds in each department. | UC-11 |

## 6. Paramedic

| ID | Requirement | Use case |
|---|---|---|
| FR-31 | Paramedic gets the job from the hospital with the location, the emergency type and the patient's allergies and diseases. | UC-12 |
| FR-32 | The job goes straight to the paramedic. He doesn't accept or reject it, the hospital already knows he's free. | UC-12 |
| FR-33 | Paramedic can update the status: on the way, arrived, picked up, going to hospital, at hospital. | UC-13 |
| FR-34 | Everyone on the case sees the new status on the map. | UC-13 |
| FR-35 | Emergency contacts get a short update when the status changes. | UC-13 |
| FR-36 | If the signal is weak, the status is sent when the paramedic is back online. | UC-13 |
| FR-37 | Every status is saved in the case history. | UC-13 |

## 7. Doctor

| ID | Requirement | Use case |
|---|---|---|
| FR-38 | Doctor can open the case and the patient's medical profile. | UC-14 |
| FR-39 | If there's no medical profile, the doctor sees the request info only. | UC-14 |
| FR-40 | Doctor can write notes and save them with the case. | UC-15 |

## 8. Admin

| ID | Requirement | Use case |
|---|---|---|
| FR-41 | Admin can add, edit or stop users and give them roles. | UC-16 |
| FR-42 | Admin can add, edit or remove hospitals and ambulances, and link each ambulance to its hospital. | UC-17 |
| FR-43 | Admin can see daily, weekly or monthly reports with the number of cases and the average response time. | UC-18 |
