using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Przychodnia.Interfaces;
using Przychodnia.Models;
using Przychodnia.Npgsql;
using Przychodnia.Transfer.PagedList;
using Przychodnia.Transfer.User;
using Przychodnia.Transfer.Visit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tenis.Core;

namespace Przychodnia.Services
{
    public class VisitService : IVisitService
    {
        private UserManager<User> UserManager { get; set; }
        private RoleManager<Role> RoleManager { get; set; }
        private DatabaseContext DataContext { get; set; }

        public VisitService(DatabaseContext dataContext, UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            DataContext = dataContext;
        }

        private DateTime GetFirstDayOfWeek(DateTime date)
        {
            DateTime newDate = date;
            switch ((int)date.DayOfWeek)
            {
                case 2:
                    newDate = newDate.AddDays(-1);
                    break;
                case 3:
                    newDate = newDate.AddDays(-2);
                    break;
                case 4:
                    newDate = newDate.AddDays(-3);
                    break;
                case 5:
                    newDate = newDate.AddDays(-4);
                    break;
                case 6:
                    newDate = newDate.AddDays(-5);
                    break;
                case 0:
                    newDate = newDate.AddDays(-6);
                    break;
            }
            return new DateTime(newDate.Year, newDate.Month, newDate.Day, 0, 0, 0);
        }

        public async Task<Result<DateTime>> CreateVisit(CreateVisitListDTO visits, User doctor)
        {

            var roleList = await UserManager.GetRolesAsync(doctor);
            if (!roleList.Contains("doctor"))
            {
                return Result.Error<DateTime>("You don't have permission to do that.");
            }

            bool hasDoctortype = true;

            foreach(CreateVisitDTO visit in visits.VisitsList)
            {
                if (!roleList.Contains(visit.VisitType)) hasDoctortype = false;
            }

            if (hasDoctortype)
            {
                var visitList = visits.VisitsList.Select(x => new Visit
                {
                    VisitId = x.VisitDate,
                    VisitStatus = Core.VisitStatus.Free,
                    UserId = doctor.Id,
                    VisitType = x.VisitType
                }
                ).ToList();
                try
                {
                    foreach (Visit visit in visitList)
                    {
                        DataContext.Visits.Add(visit);
                    }
                    DataContext.SaveChanges();
                }
                catch
                {
                    return Result.Error<DateTime>("You already have visits on this time.");
                }

                return Result.Ok(visits.VisitsList[0].VisitDate);
            }
            else return Result.Error<DateTime>("Dont have this doctorType");

        }

        public async Task<Result<List<string>>> DoctorGetTypes(User doctor)
        {

            var roleList = await UserManager.GetRolesAsync(doctor);
            if (!roleList.Contains("doctor"))
            {
                return Result.Error<List<string>> ("You don't have permission to do that.");
            }
            var rolesList = roleList.Where(x => x.ToUpper() != "ADMIN" && x.ToUpper() != "USER" && x.ToUpper() != "DOCTOR").ToList();
            return Result.Ok(rolesList);
        }

        public async Task<Result<List<VisitsListItemDTO>>> DoctorGetVisitsInWeek(GetVisitsDTO getvisit)
        {
            if (getvisit == null || getvisit.DoctorId == null || getvisit.WeekDay == null) return Result.Error<List<VisitsListItemDTO>>("Wrong data");
            DateTime weekStartDate = GetFirstDayOfWeek(getvisit.WeekDay);
            DateTime weekEndDay = weekStartDate.AddDays(6);
            var doctor = await UserManager.FindByIdAsync(getvisit.DoctorId);
            if (doctor == null) return Result.Error<List<VisitsListItemDTO>>("This doctor is not found");

            var visitList = DataContext.Visits.Where(x => x.VisitId >= weekStartDate && x.VisitId <= weekEndDay && x.UserId == Convert.ToInt32(getvisit.DoctorId))
                .Select(x =>
                new VisitsListItemDTO
                {
                    VisitId = x.VisitId,
                    VisitStatus = x.VisitStatus,
                    VisitType = x.VisitType,
                    Doctor = doctor.Name + " " + doctor.Surname,
                    DoctorId = getvisit.DoctorId
                }
                ).ToList();
            return visitList.Count == 0 ? Result.Error<List<VisitsListItemDTO>>("No visits found") : Result.Ok(visitList);
        }

        public async Task<Result<List<VisitsListItemDTO>>> GetVisitsInWeek(GetVisitsDTO getvisit)
        {
            if (getvisit == null || getvisit.DoctorId == null || getvisit.WeekDay == null || getvisit.DoctorType == null) return Result.Error<List<VisitsListItemDTO>>("Wrong data");
            DateTime weekStartDate = GetFirstDayOfWeek(getvisit.WeekDay);
            DateTime weekEndDay = weekStartDate.AddDays(6);
            var doctor = await UserManager.FindByIdAsync(getvisit.DoctorId);
            if (doctor == null) return Result.Error<List<VisitsListItemDTO>>("This doctor is not found");

            var visitList = DataContext.Visits.Where(x => x.VisitId >= weekStartDate && x.VisitId <= weekEndDay && x.UserId == Convert.ToInt32(getvisit.DoctorId) && x.VisitType == getvisit.DoctorType)
                .Select(x =>
                new VisitsListItemDTO
                {
                    VisitId = x.VisitId,
                    VisitStatus = x.VisitStatus,
                    VisitType = x.VisitType,
                    Doctor = doctor.Name + " " + doctor.Surname,
                    DoctorId = getvisit.DoctorId
                }
                ).ToList();
            return visitList.Count == 0 ? Result.Error<List<VisitsListItemDTO>>("No visits found") : Result.Ok(visitList);
        }

        public async Task<Result<List<VisitsListItemDTO>>> GetVisitsInWeek(GetAllDoctorsVisitsDTO getvisit)
        {
            if (getvisit == null || getvisit.DoctorType == null || getvisit.WeekDay == null) return Result.Error<List<VisitsListItemDTO>>("Wrong data");

            var doctors = await UserManager.GetUsersInRoleAsync(getvisit.DoctorType);
            doctors = doctors.ToList();

            DateTime weekStartDate = GetFirstDayOfWeek(getvisit.WeekDay);
            DateTime weekEndDay = weekStartDate.AddDays(6);

            var list = DataContext.Visits.Where(x => x.VisitId >= weekStartDate && x.VisitId <= weekEndDay && x.VisitType == getvisit.DoctorType)
                .Select(x =>
                    new VisitsListItemDTO
                    {
                        VisitId = x.VisitId,
                        VisitStatus = x.VisitStatus,
                        VisitType = x.VisitType,                   
                        DoctorId = x.UserId.ToString()
                    }
                ).ToList();
            foreach(VisitsListItemDTO item in list)
            {
                item.Doctor = doctors.Where(doctor => doctor.Id == Convert.ToInt32(item.DoctorId)).Select(doctor => doctor.Name + " " + doctor.Surname).FirstOrDefault();
            }



            return list.Count == 0 ? Result.Error<List<VisitsListItemDTO>>("No visits found") : Result.Ok(list);

        }

        public Result<DateTime> RegisterToVisit(RegisterToVisitDTO visit, User patient)
        {
            DateTime visitDay = new DateTime(visit.VisitId.Year, visit.VisitId.Month, visit.VisitId.Day, 0, 0, 0);
            Visit registerVisit = DataContext.Visits.SingleOrDefault(x => x.VisitId == visit.VisitId && x.UserId == Convert.ToInt32(visit.DoctorId));
            var checkIfRegistered = DataContext.Visits.Where(x =>
                x.VisitId >= visitDay &&
                x.VisitId < visitDay.AddDays(1) &&
                x.PatientId == patient.Id &&
                x.UserId == Convert.ToInt32(visit.DoctorId) &&
                x.VisitType == registerVisit.VisitType &&
                x.VisitId != registerVisit.VisitId);
            if (checkIfRegistered.Count() > 0) return Result.Error<DateTime>("You have visit on this time or you trying register to same specialist in this day.");

            registerVisit.VisitStatus = Core.VisitStatus.Reserved;
            registerVisit.PatientId = patient.Id;
            DataContext.SaveChanges();
            return Result.Ok(visit.VisitId);
        }

        public Result<DateTime> CancelVisitReservation(RegisterToVisitDTO visit, User patient)
        {
            Visit registerVisit = DataContext.Visits.SingleOrDefault(x => x.VisitId == visit.VisitId && x.UserId == Convert.ToInt32(visit.DoctorId) && x.PatientId == patient.Id);
            if (registerVisit == null) return Result.Error<DateTime>("You are not registered on this visit.");
            registerVisit.VisitStatus = Core.VisitStatus.Free;
            registerVisit.Messages = null;
            registerVisit.PatientId = 0;
            DataContext.SaveChanges();
            return Result.Ok(registerVisit.VisitId);
        }

        public Result<DateTime> DeleteVisit(DeleteVisitDTO visit, User doctor)
        {

            var visitToDelete = DataContext.Visits.SingleOrDefault(x => x.VisitId == visit.VisitId && x.UserId == doctor.Id);
            if (visitToDelete == null) return Result.Error<DateTime>("Visit not found.");
            DataContext.Visits.Remove(visitToDelete);
            DataContext.SaveChanges();
            return Result.Ok(visitToDelete.VisitId);
        }

        public async Task<Result<List<string>>> GetDoctorsTypes()
        {
            var rolesList = await DataContext.Role.Where(x => x.NormalizedName != "ADMIN" && x.NormalizedName != "USER" && x.NormalizedName != "DOCTOR").Select(x =>
                x.Name
            ).ToListAsync();
            rolesList.Sort();

            return Result.Ok(rolesList);
        }

        public Result<List<DoctorDataDTO>> GetDoctorsInType(GetDoctorsDTO getDoctor)
        {
            var doctors = UserManager.GetUsersInRoleAsync(getDoctor.DoctorType).Result.OrderBy(x => x.Surname)
                .Select(x =>
                new DoctorDataDTO
                {
                    DoctorId = x.Id.ToString(),
                    Name = x.Name,
                    Surname = x.Surname
                }
                ).ToList();


            return Result.Ok(doctors);

        }

        public Result<PagedListDTO<VisitsListItemDTO>> PatientGetReservedVisits(ListQuery query, User patient)
        {
            var visits = DataContext.Visits.Where(x => x.PatientId == patient.Id && x.VisitStatus == Core.VisitStatus.Reserved)
                .Select(visit => new VisitsListItemDTO
                {
                    VisitId = visit.VisitId,
                    DoctorId = visit.UserId.ToString(),
                    VisitStatus = visit.VisitStatus,
                    VisitType = visit.VisitType

                }).ToList();
            var viewModel = new PagedListDTO<VisitsListItemDTO>()
            {
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                TotalCount = visits.Count
            };

            viewModel.Item = visits.OrderByDescending(x => x.VisitId)
                .Skip(query.PageSize * query.PageIndex)
                .Take(query.PageSize)
                .ToList();

            if (visits.Count == 0) return Result.Error<PagedListDTO<VisitsListItemDTO>> ("You do not have registered visits.");
            foreach(VisitsListItemDTO item in visits)
            {
                item.Doctor = DataContext.Users.Where(doctor => doctor.Id == Convert.ToInt32(item.DoctorId)).Select(doctor => doctor.Name + " " + doctor.Surname).FirstOrDefault();
            }

            return Result.Ok(viewModel);
        }

        public Result<PagedListDTO<VisitsListItemDTO>> PatientGetDoneVisits(ListQuery query,User patient)
        {
            var visits = DataContext.Visits.Where(x => x.PatientId == patient.Id && (x.VisitStatus == Core.VisitStatus.Done || x.VisitStatus == Core.VisitStatus.Cancelled))
                .Select(visit => new VisitsListItemDTO
                {
                    VisitId = visit.VisitId,
                    DoctorId = visit.UserId.ToString(),
                    VisitStatus = visit.VisitStatus,
                    VisitType = visit.VisitType

                }).ToList();

            var viewModel = new PagedListDTO<VisitsListItemDTO>()
            {
                PageSize = query.PageSize,
                PageIndex = query.PageIndex,
                TotalCount = visits.Count
            };

            viewModel.Item = visits.OrderByDescending(x => x.VisitId)
                .Skip(query.PageSize * query.PageIndex)
                .Take(query.PageSize)
                .ToList();

            if (visits.Count == 0) return Result.Error<PagedListDTO<VisitsListItemDTO>>("You do not have done visits.");
            foreach (VisitsListItemDTO item in visits)
            {
                item.Doctor = DataContext.Users.Where(doctor => doctor.Id == Convert.ToInt32(item.DoctorId)).Select(doctor => doctor.Name + " " + doctor.Surname).FirstOrDefault();
            }

            return Result.Ok(viewModel);
        }

        public async Task<Result<VisitDetailsDTO>> PatientGetVisitDetails(GetVisitDetailsDTO getvisit)
        {
            var visitDetails = DataContext.Visits.Where(x => x.VisitId == getvisit.VisitId && x.UserId == Convert.ToInt32(getvisit.DoctorId) && x.PatientId == Convert.ToInt32(getvisit.PatientId))
                 .Select(x => new VisitDetailsDTO
                 {
                     VisitId = x.VisitId,
                     DoctorId = x.UserId,
                     PatientId = x.PatientId,
                     VisitType = x.VisitType,
                     VisitStatus = x.VisitStatus,
                     Findings = x.Findings,
                     Prescriptions = x.Prescriptions,
                     Messages = x.Messages
                 }
                 ).FirstOrDefault();

            if (visitDetails != null)
            {
                var doctor = DataContext.Users.Where(doctor => doctor.Id == Convert.ToInt32(getvisit.DoctorId)).FirstOrDefault();
                var doctorSpecialisations = await DoctorGetTypes(doctor);
                var patient = DataContext.Users.Where(patient => patient.Id == Convert.ToInt32(getvisit.PatientId)).FirstOrDefault();

                visitDetails.Doctor = doctor.Name + " " + doctor.Surname;
                visitDetails.DoctorEmail = doctor.Email;
                visitDetails.DoctorPhoneNumber = doctor.PhoneNumber;
                visitDetails.DoctorSpecialisations = doctorSpecialisations.Value;
                visitDetails.Patient = patient.Name + " " + patient.Surname;
                visitDetails.PatientAddress = patient.Address;
                visitDetails.PatientCountry = patient.Country;
                visitDetails.PatientEmail = patient.Email;
                visitDetails.PatientPhoneNumber = patient.PhoneNumber;
                visitDetails.PatientGender = patient.Gender;
                return Result.Ok(visitDetails);
            }else
            {
            return Result.Error<VisitDetailsDTO>("Visit not Found.");
            }

        }

        public async Task<Result<VisitDetailsDTO>> DoctorGetVisitDetails(DoctorGetVisitDetailsDTO getvisit, User doctor)
        {
            var visitDetails = DataContext.Visits.Where(x => x.VisitId == getvisit.VisitId && x.UserId == doctor.Id)
                 .Select(x => new VisitDetailsDTO
                 {
                     VisitId = x.VisitId,
                     DoctorId = x.UserId,
                     PatientId = x.PatientId,
                     VisitType = x.VisitType,
                     Findings = x.Findings,
                     VisitStatus = x.VisitStatus,
                     Prescriptions = x.Prescriptions,
                     Messages = x.Messages
                 }
                 ).FirstOrDefault();

            if (visitDetails != null)
            {
                var doctorSpecialisations = await DoctorGetTypes(doctor);
                var patient = DataContext.Users.Where(patient => patient.Id == Convert.ToInt32(visitDetails.PatientId)).FirstOrDefault();

                visitDetails.Doctor = doctor.Name + " " + doctor.Surname;
                visitDetails.DoctorEmail = doctor.Email;
                visitDetails.DoctorPhoneNumber = doctor.PhoneNumber;
                visitDetails.DoctorSpecialisations = doctorSpecialisations.Value;
                visitDetails.Patient = patient.Name + " " + patient.Surname;
                visitDetails.PatientAddress = patient.Address;
                visitDetails.PatientCountry = patient.Country;
                visitDetails.PatientEmail = patient.Email;
                visitDetails.PatientPhoneNumber = patient.PhoneNumber;
                visitDetails.PatientGender = patient.Gender;
                return Result.Ok(visitDetails);
            }
            else
            {
                return Result.Error<VisitDetailsDTO>("Visit not Found.");
            }

        }

        public Result<DateTime> DoctorFinishVisit(DoctorGetVisitDetailsDTO finishVisit, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == finishVisit.VisitId && x.UserId == doctor.Id).FirstOrDefault();
            if (visit == null) return Result.Error<DateTime>("Visit not Found"); 
            if(visit.VisitStatus != Core.VisitStatus.Reserved) return Result.Error<DateTime>("You can finish only reserved visit");
            visit.VisitStatus = Core.VisitStatus.Done;
            DataContext.Visits.Update(visit);
            DataContext.SaveChanges();

            return Result.Ok(visit.VisitId);

        }

        public Result<DateTime> DoctorCancelVisit(DoctorGetVisitDetailsDTO finishVisit, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == finishVisit.VisitId && x.UserId == doctor.Id).FirstOrDefault();
            if (visit == null) return Result.Error<DateTime>("Visit not Found");
            if (visit.VisitStatus != Core.VisitStatus.Reserved) return Result.Error<DateTime>("You can cancel only reserved visit");
            visit.VisitStatus = Core.VisitStatus.Cancelled;
            DataContext.Visits.Update(visit);
            DataContext.SaveChanges();

            return Result.Ok(visit.VisitId);

        }

        public Result<List<string>> DoctorSendMessage(SendMessageDTO message, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == message.VisitId && x.UserId == doctor.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var messagesList = visit.Messages;

            if (messagesList == null) messagesList = new List<string>();



            messagesList.Add("{\"date\":\""+ DateTime.Now.ToString() + "\",\"type\":\"doctor\",\"message\":\""+ message.Message + "\"}");

            visit.Messages = messagesList;

            DataContext.SaveChanges();

            return Result.Ok(messagesList);
        }

        public Result<List<string>> PatientSendMessage(SendMessageDTO message, User patient)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == message.VisitId && x.UserId == Convert.ToInt32(message.DoctorId) && x.PatientId == patient.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var messagesList = visit.Messages;

            if (messagesList == null) messagesList = new List<string>();



            messagesList.Add("{\"date\":\"" + DateTime.Now.ToString() + "\",\"type\":\"patient\",\"message\":\"" + message.Message + "\"}");

            visit.Messages = messagesList;

            DataContext.SaveChanges();

            return Result.Ok(messagesList);
        }

        public Result<List<string>> DoctorGetMessages(GetVisitDetailsDTO getMessage)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getMessage.VisitId && x.UserId == Convert.ToInt32(getMessage.DoctorId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Messages != null ? Result.Ok(visit.Messages) : Result.Error<List<string>>("No Messages");
        }

        public Result<List<string>> PatientGetMessages(GetVisitDetailsDTO getMessage)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getMessage.VisitId && x.UserId == Convert.ToInt32(getMessage.DoctorId) && x.PatientId == Convert.ToInt32(getMessage.PatientId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Messages != null ? Result.Ok(visit.Messages) : Result.Error<List<string>>("No Messages");
        }

        public Result<List<string>> DoctorSendPrescription(SendPrescriptionDTO prescription, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == prescription.VisitId && x.UserId == doctor.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var prescriptionList = visit.Prescriptions;

            if (prescriptionList == null) prescriptionList = new List<string>();



            prescriptionList.Add("{\"number\":"+ prescriptionList.Count+","+prescription.Prescription.Substring(1, prescription.Prescription.Length-2)+"}");

            visit.Prescriptions = prescriptionList;

            DataContext.SaveChanges();

            return Result.Ok(prescriptionList);
        }

        public Result<List<string>> DoctorGetPrescriptions(GetVisitDetailsDTO getPrescriptions)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getPrescriptions.VisitId && x.UserId == Convert.ToInt32(getPrescriptions.DoctorId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Prescriptions != null ? Result.Ok(visit.Prescriptions) : Result.Error<List<string>>("No Prescriptions");
        }

        public Result<List<string>> PatientGetPrescriptions(GetVisitDetailsDTO getPrescriptions)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getPrescriptions.VisitId && x.UserId == Convert.ToInt32(getPrescriptions.DoctorId) && x.PatientId== Convert.ToInt32(getPrescriptions.PatientId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Prescriptions != null ? Result.Ok(visit.Prescriptions) : Result.Error<List<string>>("No Prescriptions");
        }

        public Result<List<string>> DoctorDeletePrescription(DeletePrescriptionDTO prescription, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == prescription.VisitId && x.UserId == doctor.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var prescriptionList = visit.Prescriptions;

            if (prescriptionList == null) { return Result.Error<List<string>>("Visit not found"); } else
            {
                prescriptionList.Remove(prescriptionList[prescription.PrescriptionNumber]);

                visit.Prescriptions = prescriptionList;

                DataContext.SaveChanges();

                return Result.Ok(prescriptionList);
            }
        }


        public Result<List<string>> DoctorSendFinding(NewFindingDTO finding, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == finding.VisitId && x.UserId == doctor.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var findingList = visit.Findings;

            if (findingList == null) findingList = new List<string>();



            findingList.Add("{\"number\":" + findingList.Count + "," + finding.Finding.Substring(1, finding.Finding.Length - 2) + "}");

            visit.Findings = findingList;

            DataContext.SaveChanges();

            return Result.Ok(findingList);
        }

        public Result<List<string>> DoctorGetFindings(GetVisitDetailsDTO getFinding)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getFinding.VisitId && x.UserId == Convert.ToInt32(getFinding.DoctorId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Findings != null ? Result.Ok(visit.Findings) : Result.Error<List<string>>("No Prescriptions");
        }

        public Result<List<string>> PatientGetFindings(GetVisitDetailsDTO getFinding)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == getFinding.VisitId && x.UserId == Convert.ToInt32(getFinding.DoctorId) && x.PatientId == Convert.ToInt32(getFinding.PatientId)).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            return visit.Findings != null ? Result.Ok(visit.Findings) : Result.Error<List<string>>("No Prescriptions");
        }

        public Result<List<string>> DoctorDeleteFinding(DeleteFindingDTO finding, User doctor)
        {
            var visit = DataContext.Visits.Where(x => x.VisitId == finding.VisitId && x.UserId == doctor.Id).FirstOrDefault();

            if (visit == null) return Result.Error<List<string>>("Visit not found");

            var findingList = visit.Findings;

            if (findingList == null) { return Result.Error<List<string>>("Visit not found"); }
            else
            {
                findingList.Remove(findingList[finding.FindingNumber]);

                visit.Findings = findingList;

                DataContext.SaveChanges();

                return Result.Ok(findingList);
            }
        }

    }
}
