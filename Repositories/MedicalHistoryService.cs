using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class MedicalHistoryService : IMedicalHistoryService
    {
        private ApplicationDbContext _context;

        public MedicalHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool AddPatientMedicalHistory(PatientPastMedicalHistory model)
        {
            try
            {
                _context.PatientPastMedicalHistory.Add(model);
                _context.SaveChanges();
                return true;


            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> DeleteMedicalHistoryById(int MedicalHistoryId , string UserId)
        {
            var historyId = await _context.PatientPastMedicalHistory.Where(x => x.Id == MedicalHistoryId && x.UserId == UserId).SingleOrDefaultAsync();
            if(historyId != null)
            {
                _context.PatientPastMedicalHistory.Remove(historyId);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List <PatientPastMedicalHistoryViewModel> GetPatientPastMedicalHistory(string UserId)
        {
            var medicalHistoryList = _context.PatientPastMedicalHistory.Where(x => x.UserId == UserId).Select(
                                                                                 x => new PatientPastMedicalHistoryViewModel
                                                                                 {
                                                                                     Id = x.Id,
                                                                                     TreatmentName = x.TreatmentName,
                                                                                     DoctorName = x.DoctorName,
                                                                                     Date = x.Date.Value.ToString(GlobalVariables.DefaultDateFormat),
                                                                                     Description = x.Description

                                                                                 }).OrderByDescending(x => x.Id).ToList();
            return medicalHistoryList;


        }

        public bool AddPatientSurgicalHistory(PatientSurgicalHistory model)
        {
            try
            {
                _context.PatientSurgicalHistory.Add(model);
                _context.SaveChanges();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSurgicalHistoryById(int surgicalHistoryId , string UserId)
        {
            var FindhistoryId = await _context.PatientSurgicalHistory.Where(x => x.Id == surgicalHistoryId && x.UserId == UserId).SingleOrDefaultAsync();
            if (FindhistoryId != null)
            {
                _context.PatientSurgicalHistory.Remove(FindhistoryId);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<GetSurgicalHistoryViewModel> GetPatientSurgicalHistory(string UserId)
        {
            var SurgicalHistoryList = _context.PatientSurgicalHistory.Where(x => x.UserId == UserId).Select(
                                                                                 x => new GetSurgicalHistoryViewModel
                                                                                 {
                                                                                     Id = x.Id,
                                                                                     TreatmentName = x.TreatmentName,
                                                                                     DoctorName = x.DoctorName,
                                                                                     Date = x.Date.Value.ToString(GlobalVariables.DefaultDateFormat),
                                                                                     Description = x.Description

                                                                                 }).OrderByDescending(x => x.Id).ToList();
            return SurgicalHistoryList;
        }

        public bool AddPatientFamilyHistory(PatientFamilyHistory model)
        {
            try
            {
                _context.PatientFamilyHistory.Add(model);
                _context.SaveChanges();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<GetFamilyHistoryViewModel> GetPatientFamilyHistory(string UserId)
        {
            var familyHistoryList = _context.PatientFamilyHistory.Where(x => x.UserId == UserId).Select(
                                                                                 x => new GetFamilyHistoryViewModel
                                                                                 {
                                                                                     Id = x.Id,
                                                                                     DiseaseName = x.DiseaseName,
                                                                                     MemberName = x.MemberName,
                                                                                     Age = x.Age,
                                                                                     Relation = x.Relation,
                                                                                     Description = x.Description

                                                                                 }).OrderByDescending(x => x.Id).ToList();
            return familyHistoryList;
        }

        public async Task<bool> DeleteFamilyHistoryById(int familyHistoryId , string UserId)
        {
            var FindhistoryId = await _context.PatientFamilyHistory.Where(x => x.Id == familyHistoryId && x.UserId == UserId).SingleOrDefaultAsync();
            if (FindhistoryId != null)
            {
                _context.PatientFamilyHistory.Remove(FindhistoryId);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool AddPatientAllergyHistory(PatientPastAllergyHistory model)
        {
            try
            {
                _context.PatientPastAllergyHistory.Add(model);
                _context.SaveChanges();
                return true;


            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<GetAllergyHistoryViewModel> GetPatientAllergyHistory(string UserId)
        {
            var allergyHistoryList = _context.PatientPastAllergyHistory.Where(x => x.UserId == UserId).Select(
                                                                                   x => new GetAllergyHistoryViewModel()
                                                                                   {
                                                                                       Id = x.Id,
                                                                                       AllergyName = x.AllergyName,
                                                                                       Description = x.Description
                                                                                   }).OrderByDescending(x => x.Id).ToList();
            return allergyHistoryList;


        }

        public async Task<bool> DeleteAllergyHistoryById(int allergyHistoryId , string UserId)
        {
            var findId = await _context.PatientPastAllergyHistory.Where(x => x.Id == allergyHistoryId && x.UserId == UserId).SingleOrDefaultAsync();
            if(findId != null)
            {
                _context.PatientPastAllergyHistory.Remove(findId);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddPatientMedicalReport(PatientMedicalReport model)
        {
            try
            {
                _context.PatientMedicalReport.Add(model);
                _context.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletePatientMedicalReport(int medicalReportId, string UserId)
        {
            var findId = await _context.PatientMedicalReport.Where(x => x.Id == medicalReportId && x.UserId == UserId).SingleOrDefaultAsync();
            if (findId != null)
            {
                _context.PatientMedicalReport.Remove(findId);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<GetMedicalReportViewModel> GetPatientMedicalReport(string UserId)
        {
            var medicalReportList = _context.PatientMedicalReport.Where(x => x.UserId == UserId).Select(
                                                                                   x => new GetMedicalReportViewModel()
                                                                                   {
                                                                                       Id = x.Id,
                                                                                       ReportName = x.ReportName,
                                                                                       Date = x.Date.Value.ToString(GlobalVariables.DefaultDateFormat),
                                                                                       Description = x.Description,
                                                                                       MedicalDocumentPath = x.MedicalDocumentPath
                                                                                       
                                                                                   }).OrderByDescending(x => x.Id).ToList();
            return medicalReportList;


        }


    }
}
