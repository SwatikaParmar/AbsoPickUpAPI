using AnfasAPI.Common;
using AnfasAPI.Data;
using AnfasAPI.IServices;
using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Repositories
{
    public class PrescriptionService : IPrescriptionService
    {
        private ApplicationDbContext _context;

        public PrescriptionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public ResponseStatusIdViewModel AddPrescriptions(Prescriptions _Prescriptions)
        {
            ResponseStatusIdViewModel objModel = new ResponseStatusIdViewModel(); 
            try
            {
                var _appointmentDetails = _context.Appointments.Where(x => x.Id == _Prescriptions.AppointmentId
                                           && x.Status == (int)GlobalVariables.AppointmentStatus.Confirmed
                                           && x.IsDeleted == false).FirstOrDefault();
                if (_appointmentDetails != null)
                {
                    
                   _context.Prescriptions.Add(_Prescriptions);
                   _context.SaveChanges();
                    objModel.ReturnId = _Prescriptions.Id;
                    objModel.Status = true;
                   
                }
                else
                {
                    objModel.Status = false;
                }
                return objModel;

            }
            catch(Exception)
            {
                objModel.Status = false;
                return objModel;
            }
        }

        public bool AddPatientMedications(Medications model)
        {
            try
            {
                _context.Medications.Add(model);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<GetPrescriptionListViewModel> GetPrescriptionList(string userId, PrescriptionListViewModel model)
        {
            try
            {
                var _prescriptionList = _context.Prescriptions.Where(x => x.PatientId == model.PatientId 
                                                                    && x.AppointmentId == model.AppointmentId 
                                                                    && x.DoctorId == userId
                                                                    && x.IsDeleted == false)
                                                              .Select(x => new GetPrescriptionListViewModel
                                                              {
                                                                  PrescriptionId = x.Id,
                                                                  PatientId = x.PatientId,
                                                                  DoctorId = x.DoctorId,
                                                                  AppointmentId = x.AppointmentId,
                                                                  MediCareNumber = x.MedicareNumber,
                                                                  ReferenceNumber = x.ReferenceNumber,
                                                                  EntitlementNumber = x.EntitlementNumber,
                                                                  IsBrandNotPermitted = x.IsBrandNotPermitted,
                                                                  IsPBSPrescriptionFromStateManager = x.IsPBSPrescriptionFromStateManager,
                                                                  IsPBSSafetyConcessionCardHolder = x.IsPBSSafetyConcessionCardHolder,
                                                                  IsPBSSafetyEntitlementCardHolder = x.ISPBSSafetyEntitlementCardHolder,
                                                                  IsRBPSPrescription = x.IsRBPSPrescription,
                                                                  Medicines = (from ps in _context.Prescriptions
                                                                               join ms in _context.Medications
                                                                                on ps.Id equals ms.PrescriptionId
                                                                               where ps.AppointmentId == model.AppointmentId
                                                                               select new GetPatientMedicines
                                                                               {
                                                                                   MedicineId = ms.Id,
                                                                                   MedicineName = ms.MedicineName,
                                                                                   NumberOfRepeats = ms.NumberOfRepeats,
                                                                                   DosageDirections = ms.DosageDirections,
                                                                                   Quantity = ms.Quantity,
                                                                                   Date = ms.Date.ToString(GlobalVariables.DefaultDateFormat)
                                                                               }).ToList()
                               
                                                              }).ToList();


                return _prescriptionList;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public bool UpdatePrescriptionDetails(string userId, UpdatePrescriptionViewModel model)
        {
            try
            {
                var _prescriptionDetails = _context.Prescriptions.Where(x => x.Id == model.PrescriptionId).FirstOrDefault();
                if(_prescriptionDetails != null)
                {
                    _prescriptionDetails.MedicareNumber = model.MedicareNumber;
                    _prescriptionDetails.ReferenceNumber = model.ReferenceNumber;
                    _prescriptionDetails.EntitlementNumber = model.EntitlementNumber;
                    _prescriptionDetails.IsBrandNotPermitted = model.IsBrandNotPermitted;
                    _prescriptionDetails.IsPBSPrescriptionFromStateManager = model.IsPBSPrescriptionFromStateManager;
                    _prescriptionDetails.IsPBSSafetyConcessionCardHolder = model.IsPBSSafetyConcessionCardHolder;
                    _prescriptionDetails.ISPBSSafetyEntitlementCardHolder = model.ISPBSSafetyEntitlementCardHolder;
                    _prescriptionDetails.IsRBPSPrescription = model.IsRBPSPrescription;
                    _context.Prescriptions.Update(_prescriptionDetails);
                    
                    foreach(var medDetails in model.PatientMedicines)
                    {
                        Medications medi = new Medications();
                        medi.MedicineName = medDetails.MedicineName;
                        medi.DosageDirections = medDetails.DosageDirections;
                        medi.NumberOfRepeats = medDetails.NumberOfRepeats;
                        medi.Quantity = medDetails.Quantity;
                        medi.PrescriptionId = model.PrescriptionId;
                        medi.Date = DateTime.ParseExact(medDetails.Date, GlobalVariables.DefaultDateFormat, null);
                        _context.Medications.Add(medi);
                    }
                   
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeleteMedicationById(string medicineId)
        {
            try
            {
                var medicineDetails = await _context.Medications.Where(x => x.Id == medicineId).SingleOrDefaultAsync();
                if (medicineDetails != null)
                {
                    _context.Medications.Remove(medicineDetails);
                    await _context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }


    }
}
