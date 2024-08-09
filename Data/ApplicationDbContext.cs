using AnfasAPI.Models;
using AnfasAPI.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Data
{
#pragma warning disable CS1591
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookHomeService>(entity =>
            {
                entity.Property(e => e.AddressCountry).HasMaxLength(500);

                entity.Property(e => e.AddressLong).HasMaxLength(100);

                entity.Property(e => e.AdminResponse).HasMaxLength(500);

                entity.Property(e => e.AddressLat).HasMaxLength(100);

                entity.Property(e => e.AddressStreet).HasMaxLength(100);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FromDate).HasMaxLength(100);

                entity.Property(e => e.FromTime).HasMaxLength(100);

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.ToDate).HasMaxLength(100);

                entity.Property(e => e.ToTime).HasMaxLength(100);

                entity.HasOne(d => d.HomeService)
                    .WithMany(p => p.BookHomeService)
                    .HasForeignKey(d => d.HomeServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookHomeService_HomeServices");

                entity.HasOne(d => d.HomeServicesPlan)
                    .WithMany(p => p.BookHomeService)
                    .HasForeignKey(d => d.HomeServicesPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BookHomeService_HomeServicesPlans");
            });

            modelBuilder.Entity<HomeServices>(entity =>
                       {
                           entity.Property(e => e.ContactName).HasMaxLength(500);

                           entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                           entity.Property(e => e.HomeServiceAddedDate).HasColumnType("datetime");
                       });

            modelBuilder.Entity<HomeServicesFeature>(entity =>
            {
                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.HomeServices)
                    .WithMany(p => p.HomeServicesFeature)
                    .HasForeignKey(d => d.HomeServicesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_HomeServicesFeature_HomeServices");
            });

            modelBuilder.Entity<HomeServicesPlans>(entity =>
            {
                entity.HasKey(e => e.HomeServicesPlanId);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PlanAddedDate).HasColumnType("datetime");

                entity.Property(e => e.PlanName).HasMaxLength(500);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.PaymentStatus).HasMaxLength(50);

                entity.Property(e => e.UserId).HasMaxLength(450);
            });

            modelBuilder.Entity<BookLabService>(entity =>
            {
                entity.Property(e => e.AddressCountry).HasMaxLength(500);

                entity.Property(e => e.AddressLong).HasMaxLength(100);

                entity.Property(e => e.AdminResponse).HasMaxLength(500);

                entity.Property(e => e.AddressLat).HasMaxLength(100);

                entity.Property(e => e.AddressStreet).HasMaxLength(100);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FromDate).HasMaxLength(100);

                entity.Property(e => e.FromTime).HasMaxLength(100);

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.ToDate).HasMaxLength(100);

                entity.Property(e => e.ToTime).HasMaxLength(100);

                entity.HasOne(d => d.LabService)
                    .WithMany(p => p.BookLabService)
                    .HasForeignKey(d => d.LabServiceId)
                    .HasConstraintName("FK_BookLabService_LabServices");

                entity.HasOne(d => d.LabServicesPlan)
                    .WithMany(p => p.BookLabService)
                    .HasForeignKey(d => d.LabServicesPlanId)
                    .HasConstraintName("FK_BookLabService_LabServicesPlan");
            });

            modelBuilder.Entity<LabServices>(entity =>
            {
                entity.Property(e => e.ContactName).HasMaxLength(500);

                entity.Property(e => e.LabServiceAddedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LabServicesFeature>(entity =>
            {
                entity.HasOne(d => d.LabServices)
                    .WithMany(p => p.LabServicesFeature)
                    .HasForeignKey(d => d.LabServicesId)
                    .HasConstraintName("FK_LabServicesFeature_LabServices");
            });

            modelBuilder.Entity<LabServicesPlan>(entity =>
            {
                entity.Property(e => e.PlanAddedDate).HasColumnType("datetime");

                entity.Property(e => e.PlanName).HasMaxLength(500);
            });

            modelBuilder.Entity<BlogDetails>(entity =>
            {
                entity.Property(e => e.BlogImagePath).IsRequired();

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<TermsAndConditions>(entity =>
           {
               entity.Property(e => e.CreateDate)
                   .HasColumnType("datetime")
                   .HasDefaultValueSql("(getdate())");

               entity.Property(e => e.ModifyDate).HasColumnType("datetime");
           });

            modelBuilder.Entity<PrivacyPolicy>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");
            });
            modelBuilder.Entity<RadiologyServices>(entity =>
            {
                entity.Property(e => e.ContactName).HasMaxLength(500);

                entity.Property(e => e.RadiologyServiceAddedDate).HasColumnType("datetime");

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);
            });

            modelBuilder.Entity<RadiologyServicesFeature>(entity =>
            {
                entity.HasOne(d => d.RadiologyServices)
                    .WithMany(p => p.RadiologyServicesFeature)
                    .HasForeignKey(d => d.RadiologyServicesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RadiologyServicesFeature_RadiologyServices");
            });

            modelBuilder.Entity<BookRehabilationService>(entity =>
            {
                entity.Property(e => e.BookRehabilationServiceId);

                entity.Property(e => e.AdminResponse).HasMaxLength(500);

                entity.Property(e => e.BookingDate).HasMaxLength(100);

                entity.Property(e => e.BookingTime).HasMaxLength(100);

                entity.Property(e => e.PatientId).HasMaxLength(450);

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.TransactionId).HasMaxLength(500);
            });

            modelBuilder.Entity<BookDialysisService>(entity =>
            {
                entity.Property(e => e.BookDialysisServiceId).ValueGeneratedNever();

                entity.Property(e => e.AdminResponse).HasMaxLength(500);

                entity.Property(e => e.BookingDate).HasMaxLength(100);

                entity.Property(e => e.BookingTime).HasMaxLength(100);

                entity.Property(e => e.PatientId).HasMaxLength(450);

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.TransactionId).HasMaxLength(500);
            });
            // modelBuilder.Entity<RadiologyServicesPlans>(entity =>
            // {
            //     entity.HasKey(e => e.RadiologyServicesPlanId)
            //         .HasName("PK_RadiologyServicesPlan");

            //     entity.Property(e => e.ServiceCode).HasMaxLength(50);

            //     entity.Property(e => e.ServiceName).HasMaxLength(500);
            // });

            modelBuilder.Entity<RadiologyServicesPlans>(entity =>
            {
                entity.HasKey(e => e.RadiologyServicesPlanId)
                    .HasName("PK_RadiologyServicesPlan");

                entity.Property(e => e.ServiceCode).HasMaxLength(50);

                entity.Property(e => e.ServiceName).HasMaxLength(500);
            });

            modelBuilder.Entity<BookRadiologyService>(entity =>
            {
                entity.Property(e => e.AdminResponse).HasMaxLength(500);

                entity.Property(e => e.BookingDate).HasMaxLength(100);

                entity.Property(e => e.BookingTime).HasMaxLength(100);

                entity.Property(e => e.CreateDate).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ServiceType).HasMaxLength(100);

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.TransactionId).HasMaxLength(500);

                entity.HasOne(d => d.RadiologyServicePlan)
                    .WithMany(p => p.BookRadiologyService)
                    .HasForeignKey(d => d.RadiologyServicePlanId)
                    .HasConstraintName("FK_BookRadiologyService_RadiologyServicesPlans1");
            });

            // modelBuilder.Entity<BookRadiologyService>(entity =>
            // {
            //     entity.Property(e => e.AdminResponse).HasMaxLength(500);

            //     entity.Property(e => e.BookingDate).HasMaxLength(100);

            //     entity.Property(e => e.BookingTime).HasMaxLength(100);

            //     entity.Property(e => e.PatientId)
            //         .IsRequired()
            //         .HasMaxLength(450);

            //     entity.Property(e => e.ServiceType).HasMaxLength(100);

            //     entity.Property(e => e.Status).HasMaxLength(100);

            //     entity.Property(e => e.TransactionId).HasMaxLength(500);

            //     entity.HasOne(d => d.RadiologyServicePlan)
            //         .WithMany(p => p.BookRadiologyService)
            //         .HasForeignKey(d => d.RadiologyServicePlanId)
            //         .HasConstraintName("FK_BookRadiologyService_RadiologyServicesPlans1");
            // });



            modelBuilder.Entity<CountryMaster>(entity =>
            {
                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<DialysisServices>(entity =>
{
    entity.Property(e => e.ContactName).HasMaxLength(500);

    entity.Property(e => e.DialysisServiceAddedDate).HasColumnType("datetime");

    entity.Property(e => e.ServiceType).HasMaxLength(100);

    entity.Property(e => e.Status).HasMaxLength(100);
});

            modelBuilder.Entity<DialysisServicesFeature>(entity =>
            {
                entity.HasOne(d => d.DialysisServices)
                    .WithMany(p => p.DialysisServicesFeature)
                    .HasForeignKey(d => d.DialysisServicesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DialysisServicesFeature_DialysisServices");
            });

            modelBuilder.Entity<DialysisServicesPlans>(entity =>
            {
                entity.HasKey(e => e.DialysisServicesPlanId)
                    .HasName("PK_DialysisServicesPlan");

                entity.Property(e => e.ServiceCode).HasMaxLength(50);

                entity.Property(e => e.ServiceName).HasMaxLength(500);
            });

            modelBuilder.Entity<RehabilationServices>(entity =>
{
    entity.Property(e => e.ContactName).HasMaxLength(500);

    entity.Property(e => e.RehabilationServiceAddedDate).HasColumnType("datetime");

    entity.Property(e => e.ServiceType).HasMaxLength(100);

    entity.Property(e => e.Status).HasMaxLength(100);
});

            modelBuilder.Entity<RehabilationServicesFeature>(entity =>
            {
                entity.HasOne(d => d.RehabilationServices)
                    .WithMany(p => p.RehabilationServicesFeature)
                    .HasForeignKey(d => d.RehabilationServicesId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RehabilationServicesFeature_RehabilationServices");
            });

            modelBuilder.Entity<RehabilationServicesPlans>(entity =>
            {
                entity.HasKey(e => e.RehabilationServicesPlanId)
                    .HasName("PK_RadiologyServicesPlan");

                entity.Property(e => e.ServiceCode).HasMaxLength(50);

                entity.Property(e => e.ServiceName).HasMaxLength(500);
            });

            modelBuilder.Entity<ReportDetail>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(e => e.ReportName)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.RequestReport)
                    .WithMany(p => p.ReportDetail)
                    .HasForeignKey(d => d.RequestReportId)
                    .HasConstraintName("FK_ReportDetail_RequestReport");
            });

            modelBuilder.Entity<RequestReport>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ModifyDate).HasColumnType("datetime");

                entity.Property(e => e.PatientEmail)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.PatientId)
                    .IsRequired()
                    .HasMaxLength(450);
            });
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<CountryMaster> CountryMaster { get; set; }
        public DbSet<StateMaster> StateMaster { get; set; }
        public DbSet<LanguageMaster> LanguageMaster { get; set; }
        public DbSet<SpecialityMaster> SpecialityMaster { get; set; }
        public DbSet<DoctorLanguageInfo> DoctorLanguageInfo { get; set; }
        public DbSet<DoctorSpecialityInfo> DoctorSpecialityInfo { get; set; }
        public DbSet<DoctorRequestLeave> DoctorRequestLeave { get; set; }
        public DbSet<DegreeMaster> DegreeMaster { get; set; }
        public DbSet<GenderMaster> GenderMaster { get; set; }
        public DbSet<DoctorEducationInfo> DoctorEducationInfo { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<UserConfigurations> UserConfigurations { get; set; }
        public DbSet<PatientHealthInfo> PatientHealthInfo { get; set; }
        public DbSet<PatientPastMedicalHistory> PatientPastMedicalHistory { get; set; }
        public DbSet<PatientSurgicalHistory> PatientSurgicalHistory { get; set; }
        public DbSet<PatientFamilyHistory> PatientFamilyHistory { get; set; }
        public DbSet<PatientPastAllergyHistory> PatientPastAllergyHistory { get; set; }
        public DbSet<PatientMedicalReport> PatientMedicalReport { get; set; }
        public DbSet<BlogDetails> BlogDetails { get; set; }
        public DbSet<NationalityMaster> NationalityMaster { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<DoctorBankInfo> DoctorBankInfo { get; set; }
        public DbSet<Prescriptions> Prescriptions { get; set; }
        public DbSet<MedicineMaster> MedicineMaster { get; set; }
        public DbSet<PatientLabRecommendations> PatientLabRecommendations { get; set; }
        public DbSet<Medications> Medications { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedule { get; set; }
        public DbSet<DoctorTimeSlots> DoctorTimeSlots { get; set; }
        public DbSet<Appointments> Appointments { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<DoctorToDoNotes> DoctorToDoNotes { get; set; }
        public DbSet<WalletBillingInfo> WalletBillingInfo { get; set; }
        public DbSet<TransactionMaster> TransactionMaster { get; set; }
        public DbSet<AppointmentMediaSession> AppointmentMediaSession { get; set; }
        public DbSet<UserRating> UserRating { get; set; }
        public DbSet<UserPaymentInfo> UserPaymentInfo { get; set; }
        public DbSet<ChatHistory> ChatHistory { get; set; }
        public DbSet<Banner> Banner { get; set; }
        public DbSet<BookHomeService> BookHomeService { get; set; }
        public DbSet<HomeServices> HomeServices { get; set; }
        public DbSet<HomeServicesFeature> HomeServicesFeature { get; set; }
        public DbSet<HomeServicesPlans> HomeServicesPlans { get; set; }
        public DbSet<FounderAchievement> FounderAchievement { get; set; }
        public DbSet<FounderAdditionalInformation> FounderAdditionalInformation { get; set; }
        public DbSet<FounderEducation> FounderEducation { get; set; }
        public DbSet<FounderInfo> FounderInfo { get; set; }
        public DbSet<FounderPersonalAchievement> FounderPersonalAchievement { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public virtual DbSet<BookLabService> BookLabService { get; set; }
        public DbSet<LabServices> LabServices { get; set; }
        public DbSet<LabServicesFeature> LabServicesFeature { get; set; }
        public DbSet<LabServicesPlan> LabServicesPlan { get; set; }
        public DbSet<PrivacyPolicy> PrivacyPolicy { get; set; }
        public DbSet<TermsAndConditions> TermsAndConditions { get; set; }
        public virtual DbSet<RadiologyServices> RadiologyServices { get; set; }
        public virtual DbSet<RadiologyServicesFeature> RadiologyServicesFeature { get; set; }
        public virtual DbSet<RadiologyServicesPlans> RadiologyServicesPlans { get; set; }
        public virtual DbSet<BookRadiologyService> BookRadiologyService { get; set; }
        public virtual DbSet<BookDialysisService> BookDialysisService { get; set; }
        public virtual DbSet<DialysisServices> DialysisServices { get; set; }
        public virtual DbSet<DialysisServicesFeature> DialysisServicesFeature { get; set; }
        public virtual DbSet<DialysisServicesPlans> DialysisServicesPlans { get; set; }
        public virtual DbSet<BookRehabilationService> BookRehabilationService { get; set; }

        public virtual DbSet<RehabilationServices> RehabilationServices { get; set; }
        public virtual DbSet<RehabilationServicesFeature> RehabilationServicesFeature { get; set; }
        public virtual DbSet<RehabilationServicesPlans> RehabilationServicesPlans { get; set; }
        public virtual DbSet<ReportDetail> ReportDetail { get; set; }
        public virtual DbSet<RequestReport> RequestReport { get; set; }

    }

#pragma warning restore CS1591
}
