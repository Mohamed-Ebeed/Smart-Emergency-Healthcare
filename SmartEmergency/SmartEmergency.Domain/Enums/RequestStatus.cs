namespace SmartEmergency.Domain.Enums
{
    public enum RequestStatus
    {
        Submitted, 
        Confirmed,
        SentToHospital,
        AmbulanceAssigned, 
        OnTheWay, 
        Arrived, 
        PickedUp, 
        GoingToHospital,
        AtHospital, 
        Completed, 
        Cancelled, 
        Invalid
    }
}
