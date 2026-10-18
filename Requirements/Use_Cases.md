# Use Cases

Smart Emergency Response & Healthcare Coordination Platform

## Actors

| Actor | What they do |
|---|---|
| **Patient** | sends the emergency request and follows it |
| **Operator** | checks the request, sends it to a hospital, and calls the patient or family if needed |
| **Hospital Staff** | accepts or rejects requests, sends the ambulance, receives the patient and closes the case |
| **Paramedic** | works with the hospital, gets the job, sees the patient's medical info and updates the status |
| **Doctor** | sees the case and writes notes |
| **Admin** | manages the system and sees reports |

Other actors: they don't log in, but the system works with them.

| Other actor | What it does |
|---|---|
| **Google Login** | lets the patient sign in with his Google account |
| **Maps** | gives the location, the map and the distance to ambulances and hospitals |
| **Emergency Contacts** | the patient's family, they get a message when he sends a request and when the case changes |

## Use case list

| ID | Use case | Actor |
|---|---|---|
| UC-01 | Register / Login | Patient |
| UC-02 | Fill Medical Profile | Patient |
| UC-03 | Add Emergency Contacts | Patient |
| UC-04 | Send Emergency Request | Patient |
| UC-05 | Track the Ambulance | Patient |
| UC-06 | Review Request | Operator |
| UC-07 | Send Request to Hospital | Operator |
| UC-08 | Follow Active Cases | Operator |
| UC-09 | Accept Request and Send Ambulance | Hospital Staff |
| UC-10 | Receive Patient and Close Case | Hospital Staff |
| UC-11 | Update Free Beds | Hospital Staff |
| UC-12 | Get the Job | Paramedic |
| UC-13 | Update Status | Paramedic |
| UC-14 | View Case | Doctor |
| UC-15 | Write Notes | Doctor |
| UC-16 | Manage Users | Admin |
| UC-17 | Manage Ambulances and Hospitals | Admin |
| UC-18 | View Reports | Admin |

---

## Patient

### UC-01: Register / Login

| | |
|---|---|
| **Actor** | Patient |
| **Other actors** | Google Login |
| **Before** | Patient opens the website. |
| **Steps** | 1. Patient registers or logs in with email or Google.<br>2. System opens his home page. |
| **If something goes wrong** | Wrong password shows an error. If Google doesn't work, he uses email. |
| **After** | Patient is logged in. |

### UC-02: Fill Medical Profile

| | |
|---|---|
| **Actor** | Patient |
| **Before** | Patient is logged in. |
| **Steps** | 1. Patient writes his allergies, diseases and medicines.<br>2. He saves it. |
| **If something goes wrong** | He can skip it and still send requests. |
| **After** | Profile is saved and only staff can see it. |

### UC-03: Add Emergency Contacts

| | |
|---|---|
| **Actor** | Patient |
| **Before** | Patient is logged in. |
| **Steps** | 1. Patient adds a name, phone and relation.<br>2. He saves it. |
| **If something goes wrong** | Wrong phone number, the system asks him to fix it. |
| **After** | Contact is saved and will get updates. |

### UC-04: Send Emergency Request

| | |
|---|---|
| **Actor** | Patient |
| **Other actors** | Maps, Emergency Contacts |
| **Before** | Patient is logged in. |
| **Steps** | 1. Patient presses the emergency button.<br>2. He picks the emergency type.<br>3. System takes his GPS location and sends the request.<br>4. System sends a message to his emergency contacts. |
| **If something goes wrong** | GPS is off, he writes his address himself. |
| **After** | Request shows on the operator's dashboard as "Waiting". |

### UC-05: Track the Ambulance

| | |
|---|---|
| **Actor** | Patient |
| **Other actors** | Maps |
| **Before** | An ambulance is on the way. |
| **Steps** | 1. Patient sees the ambulance on the map and the arrival time.<br>2. The page updates by itself. |
| **If something goes wrong** | Live updates stop, the page refreshes every few seconds. |
| **After** | Patient knows where the ambulance is. |

---

## Operator

### UC-06: Review Request

| | |
|---|---|
| **Actor** | Operator |
| **Before** | A new request is waiting. |
| **Steps** | 1. Operator opens the new request.<br>2. AI suggests the type and priority.<br>3. Operator agrees or changes it. |
| **If something goes wrong** | AI is off, he picks from a list. |
| **After** | Request has a confirmed priority. |

### UC-07: Send Request to Hospital

| | |
|---|---|
| **Actor** | Operator |
| **Other actors** | Maps |
| **Before** | The request was reviewed. |
| **Steps** | 1. System suggests the closest hospital with the right department and free beds.<br>2. Operator sends the request to that hospital. |
| **If something goes wrong** | The hospital rejects, the system finds the next closest hospital by GPS and the operator sends the request to it. |
| **After** | A hospital accepted the request. |

### UC-08: Follow Active Cases

| | |
|---|---|
| **Actor** | Operator |
| **Other actors** | Emergency Contacts |
| **Before** | Operator is logged in. |
| **Steps** | 1. Operator sees all open cases and their status.<br>2. He can call the patient or the family if he needs to. |
| **After** | He knows the state of every case. |

---

## Hospital Staff

### UC-09: Accept Request and Send Ambulance

| | |
|---|---|
| **Actor** | Hospital Staff |
| **Other actors** | Maps |
| **Before** | The hospital got a request from the operator. |
| **Steps** | 1. Hospital staff see the patient's info, the emergency type and the location.<br>2. They accept the request if they have a free bed and a free paramedic.<br>3. System suggests the hospital's closest free ambulance.<br>4. They confirm it and send it. |
| **If something goes wrong** | No free beds or no paramedic, so they reject it and the operator sends it to the next closest hospital. |
| **After** | Status is "Assigned" and the paramedic gets the job. |

### UC-10: Receive Patient and Close Case

| | |
|---|---|
| **Actor** | Hospital Staff |
| **Before** | The ambulance is going to the hospital. |
| **Steps** | 1. Staff get ready for the patient.<br>2. They mark the patient as received when he arrives.<br>3. When treatment is done, they close the case. |
| **After** | The case is closed. |

### UC-11: Update Free Beds

| | |
|---|---|
| **Actor** | Hospital Staff |
| **Before** | Staff member is logged in. |
| **Steps** | 1. Staff update the free beds in each department. |
| **After** | Hospital suggestions use the new numbers. |

---

## Paramedic

### UC-12: Get the Job

| | |
|---|---|
| **Actor** | Paramedic |
| **Other actors** | Maps |
| **Before** | The hospital sent him to the case. |
| **Steps** | 1. Paramedic gets the job from the hospital with the location, the emergency type and the patient's allergies and diseases.<br>2. He goes to the patient. |
| **After** | Ambulance is on the way. |

### UC-13: Update Status

| | |
|---|---|
| **Actor** | Paramedic |
| **Other actors** | Maps, Emergency Contacts |
| **Before** | He got the job. |
| **Steps** | 1. Paramedic updates the status: on the way, arrived, picked up, going to hospital, at hospital.<br>2. Everyone sees the new status on the map.<br>3. Emergency contacts get a short update. |
| **If something goes wrong** | Weak signal, the status is sent when he's back online. |
| **After** | Every status is saved in the case history. |

---

## Doctor

### UC-14: View Case

| | |
|---|---|
| **Actor** | Doctor |
| **Before** | Patient was received. |
| **Steps** | 1. Doctor opens the case and the medical profile. |
| **If something goes wrong** | No medical profile, he sees the request info only. |
| **After** | Doctor has the info he needs. |

### UC-15: Write Notes

| | |
|---|---|
| **Actor** | Doctor |
| **Before** | Doctor opened the case. |
| **Steps** | 1. Doctor writes his first check and treatment notes.<br>2. He saves them. |
| **After** | Notes are saved with the case. |

---

## Admin

### UC-16: Manage Users

| | |
|---|---|
| **Actor** | Admin |
| **Before** | Admin is logged in. |
| **Steps** | 1. Admin adds, edits or stops users and gives them roles. |
| **After** | Users can log in with the right role. |

### UC-17: Manage Ambulances and Hospitals

| | |
|---|---|
| **Actor** | Admin |
| **Before** | Admin is logged in. |
| **Steps** | 1. Admin adds, edits or removes hospitals and ambulances, and links each ambulance to its hospital. |
| **After** | They can be used in suggestions. |

### UC-18: View Reports

| | |
|---|---|
| **Actor** | Admin |
| **Before** | Admin is logged in. |
| **Steps** | 1. Admin picks daily, weekly or monthly.<br>2. He sees the number of cases and the average response time. |
| **After** | Admin sees how the system is doing. |
