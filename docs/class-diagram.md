# Smart Emergency Response & Healthcare Coordination Platform

## Class Diagram Explanation

The class diagram represents the main entities and relationships of the Smart Emergency Response & Healthcare Coordination Platform.

### Main Classes

- **User:** Stores common account information and serves as the base class for different user types.
- **Patient:** Represents a person who may require emergency assistance and has medical information and emergency contacts.
- **Dispatcher:** Reviews emergency requests and coordinates ambulance assignments.
- **Paramedic:** Records patient conditions and emergency care information.
- **Doctor:** Reviews emergency cases and represents medical staff.
- **HospitalAdmin:** Manages hospital departments and resources.
- **Role:** Defines user roles and access permissions.
- **EmergencyRequest:** Stores emergency details, location, priority, status, and assignment information.
- **Ambulance:** Stores vehicle information, availability, and location.
- **Hospital:** Represents a healthcare facility that receives emergency cases.
- **Department:** Represents a hospital unit and its available resources.
- **EmergencyMedicalProfile:** Stores relevant medical information, such as blood type, allergies, and medications.
- **EmergencyContact:** Stores contact details for people to reach during an emergency.
- **Notification:** Represents messages sent to users.
- **EmergencyStatusHistory:** Records emergency status changes for tracking and auditing.

### Relationships

- User is the parent class of Patient, Dispatcher, Paramedic, Doctor, and HospitalAdmin.
- Users can have multiple roles, depending on the system's authorization rules.
- A patient can have one medical profile and multiple emergency contacts.
- A user can submit multiple emergency requests.
- An emergency request may be assigned an ambulance and a destination hospital.
- A hospital contains departments, and departments are associated with doctors.
- Each emergency request maintains a history of status changes.
- Users can receive notifications related to emergency requests.

### Design Note

This diagram is a proposed domain model. Its classes, attributes, and relationships must be validated against the approved ERD and system architecture before final acceptance.
