using CLINICAL_MANAGEMENT.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CLINICAL_MANAGEMENT.Repository
{
    public class ReceptionRepositoryImpl : IReceptionRepository
    {
        private readonly CmsContext _context;

        public ReceptionRepositoryImpl(CmsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _context.Patients
                .Where(p => p.Status != "Inactive")
                .ToListAsync();
        }

        public async Task<Patient?> GetPatientById(int id)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id && p.Status != "Inactive");
        }

        public async Task<IEnumerable<Patient>> SearchPatients(string term)
        {
            term = term.Trim().ToLower();

            return await _context.Patients
                .Where(p =>
                    p.Status != "Inactive" &&
                    (
                        (p.Name != null && p.Name.ToLower().Contains(term)) ||
                        (p.Phone != null && p.Phone.Contains(term)) ||
                        (p.Mmrno != null && p.Mmrno.ToLower().Contains(term))
                    ))
                .ToListAsync();
        }

        public async Task<string> GenerateNextMmrNo()
        {
            var lastPatient = await _context.Patients
                .OrderByDescending(p => p.PatientId)
                .FirstOrDefaultAsync();

            if (lastPatient == null || string.IsNullOrWhiteSpace(lastPatient.Mmrno))
                return "MMR001";

            var lastNumberPart = lastPatient.Mmrno.Replace("MMR", "");

            if (!int.TryParse(lastNumberPart, out int lastNumber))
                return "MMR001";

            return $"MMR{(lastNumber + 1):D3}";
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            patient.Mmrno = await GenerateNextMmrNo();

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patient.PatientId && p.Status != "Inactive");

            if (existingPatient == null)
                return false;

            // Do not update MMRNo
            existingPatient.Name = patient.Name;
            existingPatient.Address = patient.Address;
            existingPatient.Gender = patient.Gender;
            existingPatient.Dob = patient.Dob;
            existingPatient.BloodGroup = patient.BloodGroup;
            existingPatient.Phone = patient.Phone;
            existingPatient.Status = patient.Status;
            existingPatient.Email = patient.Email;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePatient(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return false;

            patient.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DoctorSlot>> GetAvailableSlotsByDate(DateOnly date)
        {
            return await _context.DoctorSlots
                .Include(s => s.Doctor)
                .Where(s => s.SlotDate == date && s.IsBooked != true)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<Appointment?> BookAppointment(int patientId, int slotId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patientId && p.Status != "Inactive");

            if (patient == null)
                return null;

            var slot = await _context.DoctorSlots
                .FirstOrDefaultAsync(s => s.SlotId == slotId && s.IsBooked != true);

            if (slot == null)
                return null;

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == slot.DoctorId);

            if (doctor == null)
                return null;

            int nextToken = 1;

            var lastToken = await _context.Appointments
                .Where(a => a.DoctorId == slot.DoctorId && a.AppointmentDate == slot.SlotDate)
                .OrderByDescending(a => a.TokenNumber)
                .Select(a => a.TokenNumber)
                .FirstOrDefaultAsync();

            if (lastToken.HasValue)
                nextToken = lastToken.Value + 1;

            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = slot.DoctorId,
                SlotId = slot.SlotId,
                AppointmentDate = slot.SlotDate,
                TokenNumber = nextToken,
                Status = "Scheduled",
                ConsultationBill = doctor.ConsultationFees
            };

            slot.IsBooked = true;
            slot.PatientId = patientId;

            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatient(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Slot)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetAllAppointments()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.TokenNumber)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.PatientId,
                    PatientName = a.Patient != null ? a.Patient.Name : "",
                    MmrNo = a.Patient != null ? a.Patient.Mmrno : "",
                    a.DoctorId,
                    a.TokenNumber,
                    a.AppointmentDate,
                    a.Status,
                    a.ConsultationBill,
                    a.SlotId
                })
                .ToListAsync<object>();
        }

        public async Task<IEnumerable<object>> SearchAppointments(string? term, DateOnly? date)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .AsQueryable();

            if (date.HasValue)
                query = query.Where(a => a.AppointmentDate == date.Value);

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim().ToLower();

                query = query.Where(a =>
                    a.Patient != null &&
                    (
                        (a.Patient.Name != null && a.Patient.Name.ToLower().Contains(term)) ||
                        (a.Patient.Mmrno != null && a.Patient.Mmrno.ToLower().Contains(term))
                    ));
            }

            return await query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.TokenNumber)
                .Select(a => new
                {
                    a.AppointmentId,
                    a.PatientId,
                    PatientName = a.Patient != null ? a.Patient.Name : "",
                    MmrNo = a.Patient != null ? a.Patient.Mmrno : "",
                    a.DoctorId,
                    a.TokenNumber,
                    a.AppointmentDate,
                    a.Status,
                    a.ConsultationBill,
                    a.SlotId
                })
                .ToListAsync<object>();
        }

        public async Task<object?> GenerateConsultationBill(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return null;

            return new
            {
                appointment.AppointmentId,
                appointment.PatientId,
                PatientName = appointment.Patient?.Name,
                PatientEmail = appointment.Patient?.Email,
                MmrNo = appointment.Patient?.Mmrno,
                appointment.DoctorId,
                appointment.TokenNumber,
                appointment.AppointmentDate,
                appointment.Status,
                DoctorFee = appointment.ConsultationBill,
                appointment.SlotId
            };
        }

        public async Task<byte[]?> GenerateConsultationBillPdf(int appointmentId)
        {
            var bill = await GenerateConsultationBill(appointmentId);

            if (bill == null)
                return null;

            dynamic b = bill;

            QuestPDF.Settings.License = LicenseType.Community;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    page.Content().Column(column =>
                    {
                        column.Item().Text("Clinic Management System")
                            .FontSize(22).Bold().AlignCenter();

                        column.Item().Text("Consultation Bill")
                            .FontSize(16).SemiBold().AlignCenter();

                        column.Item().PaddingVertical(10);
                        column.Item().LineHorizontal(1);

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                            });

                            void Cell(string label, string value)
                            {
                                table.Cell().Text(label).Bold();
                                table.Cell().Text(value);
                            }

                            Cell("Appointment ID", b.AppointmentId.ToString());
                            Cell("Patient ID", b.PatientId.ToString());
                            Cell("Patient Name", b.PatientName ?? "");
                            Cell("MMR No", b.MmrNo ?? "");
                            Cell("Email", b.PatientEmail ?? "");
                            Cell("Doctor ID", b.DoctorId.ToString());
                            Cell("Token No", b.TokenNumber.ToString());
                            Cell("Date", b.AppointmentDate.ToString("dd-MM-yyyy"));
                            Cell("Status", b.Status ?? "");
                        });

                        column.Item().PaddingVertical(10);
                        column.Item().LineHorizontal(1);

                        column.Item().Text("Billing Details")
                            .FontSize(14).Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(1);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Description").Bold();
                                header.Cell().AlignRight().Text("Amount").Bold();
                            });

                            table.Cell().Text("Consultation Fee");
                            table.Cell().AlignRight().Text($"₹ {b.DoctorFee}");
                        });

                        column.Item().AlignRight().Text($"Total: ₹ {b.DoctorFee}")
                            .FontSize(14).Bold();

                        column.Item().PaddingTop(20);
                        column.Item().AlignCenter().Text("Thank you for visiting!")
                            .Italic().FontSize(10);
                    });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        public async Task<bool> EmailConsultationBill(int appointmentId, string? email)
        {
            var bill = await GenerateConsultationBill(appointmentId);
            if (bill == null)
                return false;

            dynamic b = bill;

            string targetEmail = !string.IsNullOrWhiteSpace(email)
                ? email
                : (string?)b.PatientEmail ?? string.Empty;

            if (string.IsNullOrWhiteSpace(targetEmail))
                return false;

            var pdfBytes = await GenerateConsultationBillPdf(appointmentId);
            if (pdfBytes == null)
                return false;

            var message = new MailMessage
            {
                From = new MailAddress("yourgmail@gmail.com"),
                Subject = $"Consultation Bill - Appointment {b.AppointmentId}",
                Body = $"Dear {b.PatientName},\n\nPlease find your consultation bill attached.\n\nRegards,\nClinic Management System"
            };

            message.To.Add(targetEmail);

            using var stream = new MemoryStream(pdfBytes);
            message.Attachments.Add(
                new Attachment(stream, $"ConsultationBill_{b.AppointmentId}.pdf", "application/pdf")
            );

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("yourgmail@gmail.com", "your-app-password"),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
            return true;
        }
    }
}