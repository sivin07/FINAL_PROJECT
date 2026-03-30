using CLINICAL_MANAGEMENT.DTODoctor;
using CLINICAL_MANAGEMENT.Repositories;

namespace CLINICAL_MANAGEMENT.Services
{
    public class DoctorServiceImpl :IDoctorService
    {
        private readonly IDoctorRepository _repo;
        private readonly ILogger<DoctorServiceImpl> _logger;

        // Appointment status constants — single source of truth
        private const string STATUS_IN_PROGRESS = "InProgress";
        private const string STATUS_COMPLETED = "Completed";

        public DoctorServiceImpl(IDoctorRepository repo, ILogger<DoctorServiceImpl> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // ══════════════════════════════════════════════════════════
        //  DASHBOARD
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Returns today's and tomorrow's appointments for the logged-in doctor.
        /// Two separate SP calls are merged into one list (today first, then tomorrow).
        /// </summary>
        public async Task<IEnumerable<AppointmentDto>> GetDashboardAppointmentsAsync(int doctorId)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Parallel fetch for efficiency
            var todayTask = _repo.GetAppointmentsByDoctorAndDateAsync(doctorId, today);
            var tomorrowTask = _repo.GetAppointmentsByDoctorAndDateAsync(doctorId, tomorrow);

            await Task.WhenAll(todayTask, tomorrowTask);

            var results = todayTask.Result
                .Concat(tomorrowTask.Result)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.TimeSlot)
                .ToList();

            _logger.LogInformation(
                "Dashboard loaded {Count} appointments for DoctorId={DoctorId}",
                results.Count, doctorId);

            return results;
        }

        // ══════════════════════════════════════════════════════════
        //  CONSULTATION DETAIL
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Loads consultation page data.
        /// Also marks the appointment as InProgress so the receptionist knows it's started.
        /// Validates that this appointment belongs to the requesting doctor.
        /// </summary>
        public async Task<ConsultationDetailDto?> GetConsultationDetailAsync(
            int appointmentId, int doctorId)
        {
            var detail = await _repo.GetConsultationDetailByAppointmentAsync(appointmentId);

            if (detail == null)
            {
                _logger.LogWarning(
                    "ConsultationDetail not found for AppointmentId={AppointmentId}", appointmentId);
                return null;
            }

            // If the consultation was already saved, hydrate prescriptions & lab requests
            if (!string.IsNullOrEmpty(detail.Symptoms))
            {
                // ConsultationId comes embedded in the SP result
                // Use AppointmentId to correlate — SP returns ConsultationId as part of detail
                // (See usp_GetConsultationDetailByAppointment)
            }

            // Mark appointment InProgress (idempotent — SP handles already-InProgress case)
            if (detail.AppointmentStatus == "Scheduled")
            {
                await _repo.UpdateAppointmentStatusAsync(appointmentId, STATUS_IN_PROGRESS);
                detail.AppointmentStatus = STATUS_IN_PROGRESS;
            }

            return detail;
        }

        // ══════════════════════════════════════════════════════════
        //  SAVE CONSULTATION — CORE BUSINESS LOGIC
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Full consultation save flow:
        ///   Step 1: Input validation
        ///   Step 2: Calculate quantities (Frequency × Duration) for each medicine
        ///   Step 3: Validate stock for ALL medicines before inserting anything
        ///   Step 4: Insert the consultation record → get ConsultationId
        ///   Step 5: Insert prescriptions + deduct stock for each medicine
        ///   Step 6: Insert lab requests
        ///   Step 7: Mark appointment Completed
        /// If ANY stock check fails → abort, return errors, nothing is persisted.
        /// </summary>
        public async Task<SaveConsultationResponseDto> SaveConsultationAsync(
            SaveConsultationRequestDto request)
        {
            // ── Step 1: Validate required fields ─────────────────
            var validationErrors = ValidateConsultationRequest(request);
            if (validationErrors.Any())
            {
                return new SaveConsultationResponseDto
                {
                    Success = false,
                    Message = "Validation failed.",
                    StockErrors = validationErrors
                };
            }

            // ── Step 2: Calculate medicine quantities ─────────────
            //    Quantity = Frequency × Duration (business rule)
            var medicineOrders = request.Medicines
                .Select(m => new
                {
                    Medicine = m,
                    Quantity = m.Frequency * m.Duration   // ← core calculation
                })
                .ToList();

            // ── Step 3: Validate ALL stock BEFORE any DB write ────
            var stockErrors = new List<string>();

            var stockTasks = medicineOrders.Select(async mo =>
            {
                int availableStock = await _repo.GetMedicineStockAsync(mo.Medicine.MedicineId);
                if (availableStock < mo.Quantity)
                {
                    stockErrors.Add(
                        $"Insufficient stock for '{mo.Medicine.MedicineName}': " +
                        $"Required {mo.Quantity}, Available {availableStock}.");
                }
            });

            await Task.WhenAll(stockTasks);

            if (stockErrors.Any())
            {
                _logger.LogWarning(
                    "Stock validation failed for ConsultationSave. AppointmentId={Id}. Errors: {Errors}",
                    request.AppointmentId, string.Join("; ", stockErrors));

                return new SaveConsultationResponseDto
                {
                    Success = false,
                    Message = "Stock validation failed. Consultation not saved.",
                    StockErrors = stockErrors
                };
            }

            // ── Step 4: Insert Consultation ───────────────────────
            int consultationId;
            try
            {
                consultationId = await _repo.InsertConsultationAsync(
                    request.AppointmentId,
                    request.PatientId,
                    request.DoctorId,
                    request.Symptoms,
                    request.Diagnosis,
                    request.DoctorNotes);

                if (consultationId <= 0)
                    throw new InvalidOperationException("Consultation insert returned invalid ID.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to insert consultation for AppointmentId={Id}", request.AppointmentId);
                return new SaveConsultationResponseDto
                {
                    Success = false,
                    Message = "Failed to save consultation. Please try again."
                };
            }

            // ── Step 5: Insert Prescriptions + Deduct Stock ───────
            foreach (var mo in medicineOrders)
            {
                try
                {
                    await _repo.InsertPrescriptionAsync(
                        consultationId,
                        mo.Medicine.MedicineId,
                        mo.Medicine.Frequency,
                        mo.Medicine.Duration,
                        mo.Quantity);                    // server-calculated quantity

                    await _repo.DeductMedicineStockAsync(mo.Medicine.MedicineId, mo.Quantity);

                    _logger.LogInformation(
                        "Prescription saved & stock deducted: MedicineId={MId}, Qty={Qty}",
                        mo.Medicine.MedicineId, mo.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error saving prescription for MedicineId={MId} in ConsultationId={CId}",
                        mo.Medicine.MedicineId, consultationId);
                    // Non-fatal per medicine — continue, but log clearly
                }
            }

            // ── Step 6: Insert Lab Requests ───────────────────────
            foreach (var lt in request.LabTests)
            {
                try
                {
                    await _repo.InsertLabRequestAsync(
                        consultationId,
                        request.PatientId,
                        request.DoctorId,
                        lt.LabTestId,
                        lt.SpecialInstructions);

                    _logger.LogInformation(
                        "Lab request inserted: LabTestId={LId}, ConsultationId={CId}",
                        lt.LabTestId, consultationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error inserting lab request for LabTestId={LId}", lt.LabTestId);
                }
            }

            // ── Step 7: Mark Appointment Completed ────────────────
            await _repo.UpdateAppointmentStatusAsync(request.AppointmentId, STATUS_COMPLETED);

            _logger.LogInformation(
                "Consultation saved successfully. ConsultationId={CId}, AppointmentId={AId}",
                consultationId, request.AppointmentId);

            return new SaveConsultationResponseDto
            {
                Success = true,
                Message = "Consultation saved successfully.",
                ConsultationId = consultationId
            };
        }

        // ══════════════════════════════════════════════════════════
        //  DROPDOWNS
        // ══════════════════════════════════════════════════════════

        public async Task<IEnumerable<MedicineDropdownDto>> GetMedicineDropdownAsync() =>
            await _repo.GetMedicineDropdownAsync();

        public async Task<IEnumerable<LabTestDropdownDto>> GetLabTestDropdownAsync() =>
            await _repo.GetLabTestDropdownAsync();

        // ══════════════════════════════════════════════════════════
        //  PATIENT HISTORY
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// Fetches all past consultations for a patient, enriching each
        /// with its prescriptions and lab requests.
        /// Note: No DoctorId filter here — doctor sees FULL patient history
        ///       (consultations done by other doctors too, for continuity of care).
        /// </summary>
        public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(
            int patientId, int doctorId)
        {
            var consultations = (await _repo.GetPatientHistoryAsync(patientId)).ToList();

            // Enrich each consultation with prescriptions and lab requests in parallel
            var enrichTasks = consultations.Select(async c =>
            {
                var prescriptions = _repo.GetPrescriptionsByConsultationAsync(c.ConsultationId);
                var labRequests = _repo.GetLabRequestsByConsultationAsync(c.ConsultationId);

                await Task.WhenAll(prescriptions, labRequests);

                c.Prescriptions = prescriptions.Result.ToList();
                c.LabRequests = labRequests.Result.ToList();
            });

            await Task.WhenAll(enrichTasks);

            return consultations.OrderByDescending(c => c.ConsultationDate);
        }

        // ══════════════════════════════════════════════════════════
        //  LAB RESULTS
        // ══════════════════════════════════════════════════════════

        public async Task<IEnumerable<LabResultDto>> GetLabResultsForDoctorAsync(
            int doctorId, DateTime date) =>
            await _repo.GetLabResultsByDoctorAsync(doctorId, date);

        public async Task<LabResultDto?> GetLabResultDetailAsync(int labRequestId, int doctorId)
        {
            var result = await _repo.GetLabResultByRequestIdAsync(labRequestId);
            // Result already scoped by SP — but log if null for debugging
            if (result == null)
                _logger.LogWarning(
                    "Lab result not found: LabRequestId={Id}, DoctorId={DId}",
                    labRequestId, doctorId);
            return result;
        }

        // ══════════════════════════════════════════════════════════
        //  PRIVATE HELPERS
        // ══════════════════════════════════════════════════════════

        private static List<string> ValidateConsultationRequest(SaveConsultationRequestDto req)
        {
            var errors = new List<string>();

            if (req.AppointmentId <= 0)
                errors.Add("AppointmentId is required.");
            if (req.PatientId <= 0)
                errors.Add("PatientId is required.");
            if (req.DoctorId <= 0)
                errors.Add("DoctorId is required.");
            if (string.IsNullOrWhiteSpace(req.Symptoms))
                errors.Add("Symptoms are required.");
            if (string.IsNullOrWhiteSpace(req.Diagnosis))
                errors.Add("Diagnosis is required.");

            // Validate medicine orders
            foreach (var m in req.Medicines)
            {
                if (m.MedicineId <= 0)
                    errors.Add("Each medicine must have a valid MedicineId.");
                if (m.Frequency <= 0)
                    errors.Add($"Frequency for MedicineId {m.MedicineId} must be > 0.");
                if (m.Duration <= 0)
                    errors.Add($"Duration for MedicineId {m.MedicineId} must be > 0.");
            }

            // Validate lab test orders
            foreach (var lt in req.LabTests)
            {
                if (lt.LabTestId <= 0)
                    errors.Add("Each lab test must have a valid LabTestId.");
            }

            return errors;
        }
    }
}