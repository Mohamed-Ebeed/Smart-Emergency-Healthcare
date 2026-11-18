
using SmartEmergency.Domain.Enums;

namespace SmartEmergency.Domain.Rules
{
    public static class RequestStatusTransitions
    {
        public static bool CanTransition(
            RequestStatus currentStatus,
            RequestStatus newStatus)
        {
            return currentStatus switch
            {
                RequestStatus.Submitted =>
                    newStatus == RequestStatus.Confirmed ||
                    newStatus == RequestStatus.Cancelled ||
                    newStatus == RequestStatus.Invalid,

                RequestStatus.Confirmed =>
                    newStatus == RequestStatus.SentToHospital ||
                    newStatus == RequestStatus.Cancelled,

                RequestStatus.SentToHospital =>
                    newStatus == RequestStatus.AmbulanceAssigned ||
                    newStatus == RequestStatus.Cancelled,

                RequestStatus.AmbulanceAssigned =>
                    newStatus == RequestStatus.OnTheWay ||
                    newStatus == RequestStatus.Cancelled,

                RequestStatus.OnTheWay =>
                    newStatus == RequestStatus.Arrived ||
                    newStatus == RequestStatus.Cancelled,

                RequestStatus.Arrived =>
                    newStatus == RequestStatus.PickedUp ||
                    newStatus == RequestStatus.Cancelled,

                RequestStatus.PickedUp =>
                    newStatus == RequestStatus.GoingToHospital,

                RequestStatus.GoingToHospital =>
                    newStatus == RequestStatus.AtHospital,

                RequestStatus.AtHospital =>
                    newStatus == RequestStatus.Completed,

                _ => false
            };
        }

        public static bool IsFinalStatus(RequestStatus status)
        {
            return status == RequestStatus.Completed ||
                   status == RequestStatus.Cancelled ||
                   status == RequestStatus.Invalid;
        }
    }
}
