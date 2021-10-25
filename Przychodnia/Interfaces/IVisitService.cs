using Przychodnia.Models;
using Przychodnia.Transfer.PagedList;
using Przychodnia.Transfer.User;
using Przychodnia.Transfer.Visit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenis.Core;

namespace Przychodnia.Interfaces
{
    public interface IVisitService
    {
        Task<Result<DateTime>> CreateVisit(CreateVisitListDTO visit, User doctor);
        Task<Result<List<string>>> DoctorGetTypes(User doctor);
        Task<Result<List<VisitsListItemDTO>>> DoctorGetVisitsInWeek(GetVisitsDTO getvisit);
        Task<Result<List<VisitsListItemDTO>>> GetVisitsInWeek(GetVisitsDTO getvisit);
        Task<Result<List<VisitsListItemDTO>>> GetVisitsInWeek(GetAllDoctorsVisitsDTO getvisit);
        Result<DateTime> RegisterToVisit(RegisterToVisitDTO visit, User patient);
        Result<DateTime> CancelVisitReservation(RegisterToVisitDTO visit, User patient);
        Result<DateTime> DeleteVisit(DeleteVisitDTO visit, User doctor);
        Task<Result<List<string>>> GetDoctorsTypes();
        Result<List<DoctorDataDTO>> GetDoctorsInType(GetDoctorsDTO getDoctor);
        Result<PagedListDTO<VisitsListItemDTO>> PatientGetReservedVisits(ListQuery query, User patient);
        Result<PagedListDTO<VisitsListItemDTO>> PatientGetDoneVisits(ListQuery query, User patient);
        Task<Result<VisitDetailsDTO>> PatientGetVisitDetails(GetVisitDetailsDTO getvisit);
        Task<Result<VisitDetailsDTO>> DoctorGetVisitDetails(DoctorGetVisitDetailsDTO getvisit, User doctor);
        Result<DateTime> DoctorFinishVisit(DoctorGetVisitDetailsDTO finishVisit, User doctor);
        Result<DateTime> DoctorCancelVisit(DoctorGetVisitDetailsDTO finishVisit, User doctor);
        Result<List<string>> DoctorSendMessage(SendMessageDTO message, User doctor);
        Result<List<string>> PatientSendMessage(SendMessageDTO message, User patient);
        Result<List<string>> DoctorGetMessages(GetVisitDetailsDTO getMessage);
        Result<List<string>> PatientGetMessages(GetVisitDetailsDTO getMessage);
        Result<List<string>> DoctorSendPrescription(SendPrescriptionDTO prescription, User doctor);
        Result<List<string>> DoctorGetPrescriptions(GetVisitDetailsDTO getPrescriptions);
        Result<List<string>> PatientGetPrescriptions(GetVisitDetailsDTO getPrescriptions);
        Result<List<string>> DoctorDeletePrescription(DeletePrescriptionDTO prescription, User doctor);
        Result<List<string>> DoctorSendFinding(NewFindingDTO finding, User doctor);
        Result<List<string>> DoctorGetFindings(GetVisitDetailsDTO getPrescriptions);
        Result<List<string>> PatientGetFindings(GetVisitDetailsDTO getFinding);
        Result<List<string>> DoctorDeleteFinding(DeleteFindingDTO finding, User doctor);


    }
}
