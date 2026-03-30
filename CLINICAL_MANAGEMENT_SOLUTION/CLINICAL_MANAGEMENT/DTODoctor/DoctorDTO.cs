namespace CLINICAL_MANAGEMENT.DTODoctor
{
   
    // ─────────────────────────────────────────────
     //  APPOINTMENT DTOs
     // ─────────────────────────────────────────────

        /// <summary>Response DTO for appointments shown on Doctor Dashboard.</summary>
        public class AppointmentDto
        {
            public int AppointmentId { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public int Age { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string ContactNumber { get; set; } = string.Empty;
            public DateTime AppointmentDate { get; set; }
            public string TimeSlot { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;   // Scheduled / InProgress / Completed
            public string ReasonForVisit { get; set; } = string.Empty;
        }

        // ─────────────────────────────────────────────
        //  CONSULTATION DTOs
        // ─────────────────────────────────────────────

        /// <summary>Full patient details shown when doctor opens a consultation.</summary>
        public class ConsultationDetailDto
        {
            public int AppointmentId { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public int Age { get; set; }
            public string Gender { get; set; } = string.Empty;
            public string BloodGroup { get; set; } = string.Empty;
            public string ContactNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string ReasonForVisit { get; set; } = string.Empty;
            public DateTime AppointmentDate { get; set; }
            public string AppointmentStatus { get; set; } = string.Empty;

            // Populated only when re-opening a saved consultation
            public string? Symptoms { get; set; }
            public string? Diagnosis { get; set; }
            public string? DoctorNotes { get; set; }
            public List<PrescriptionItemDto> Prescriptions { get; set; } = new();
            public List<LabRequestItemDto> LabRequests { get; set; } = new();
        }

        /// <summary>Request DTO sent by the doctor to save a full consultation.</summary>
        public class SaveConsultationRequestDto
        {
            public int AppointmentId { get; set; }   // Links back to appointment
            public int PatientId { get; set; }
            public int DoctorId { get; set; }

            // Consultation Form Fields
            public string Symptoms { get; set; } = string.Empty;
            public string Diagnosis { get; set; } = string.Empty;
            public string? DoctorNotes { get; set; }

            // Nested collections — saved in one atomic operation
            public List<MedicineOrderDto> Medicines { get; set; } = new();
            public List<LabTestOrderDto> LabTests { get; set; } = new();
        }

        /// <summary>Response after saving a consultation.</summary>
        public class SaveConsultationResponseDto
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public int ConsultationId { get; set; }   // Newly created ConsultationId
            public List<string> StockErrors { get; set; } = new();   // Per-medicine stock failures
        }

        // ─────────────────────────────────────────────
        //  MEDICINE / PRESCRIPTION DTOs
        // ─────────────────────────────────────────────

        /// <summary>Dropdown item — loaded from Pharmacist module.</summary>
        public class MedicineDropdownDto
        {
            public int MedicineId { get; set; }
            public string MedicineName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string DosageForm { get; set; } = string.Empty;   // Tablet / Syrup / Injection
            public int AvailableStock { get; set; }
        }

        /// <summary>Doctor's medicine order inside the consultation form.</summary>
        public class MedicineOrderDto
        {
            public int MedicineId { get; set; }
            public string MedicineName { get; set; } = string.Empty;   // For display / error messages
            public int Frequency { get; set; }   // Times per day (e.g. 3)
            public int Duration { get; set; }   // Days (e.g. 7)
                                                // Quantity is server-calculated: Frequency × Duration
        }

        /// <summary>Prescription line item returned in consultation detail / history.</summary>
        public class PrescriptionItemDto
        {
            public int PrescriptionId { get; set; }
            public int MedicineId { get; set; }
            public string MedicineName { get; set; } = string.Empty;
            public int Frequency { get; set; }
            public int Duration { get; set; }
            public int Quantity { get; set; }   // Frequency × Duration
            public string DosageForm { get; set; } = string.Empty;
        }

        // ─────────────────────────────────────────────
        //  LAB TEST DTOs
        // ─────────────────────────────────────────────

        /// <summary>Dropdown item — loaded from Lab Technician module.</summary>
        public class LabTestDropdownDto
        {
            public int LabTestId { get; set; }
            public string TestName { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;   // Haematology / Biochemistry / etc.
            public decimal Price { get; set; }
        }

        /// <summary>Lab test the doctor adds during consultation.</summary>
        public class LabTestOrderDto
        {
            public int LabTestId { get; set; }
            public string? SpecialInstructions { get; set; }
        }

        /// <summary>Lab request item returned in consultation / history.</summary>
        public class LabRequestItemDto
        {
            public int LabRequestId { get; set; }
            public int LabTestId { get; set; }
            public string TestName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;   // Pending / InProgress / Completed
            public string? SpecialInstructions { get; set; }
            public DateTime RequestedOn { get; set; }
        }

        // ─────────────────────────────────────────────
        //  LAB RESULT DTOs
        // ─────────────────────────────────────────────

        /// <summary>Lab result returned from Lab Technician module.</summary>
        public class LabResultDto
        {
            public int LabRequestId { get; set; }
            public int PatientId { get; set; }
            public string PatientName { get; set; } = string.Empty;
            public int LabTestId { get; set; }
            public string TestName { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;   // Pending / Completed
            public string? ResultValue { get; set; }
            public string? ReferenceRange { get; set; }
            public string? Remarks { get; set; }
            public DateTime? CompletedOn { get; set; }
            public DateTime RequestedOn { get; set; }
        }

        // ─────────────────────────────────────────────
        //  PATIENT HISTORY DTOs
        // ─────────────────────────────────────────────

        /// <summary>One past consultation record shown in Patient History.</summary>
        public class PatientHistoryDto
        {
            public int ConsultationId { get; set; }
            public int AppointmentId { get; set; }
            public DateTime ConsultationDate { get; set; }
            public string DoctorName { get; set; } = string.Empty;
            public string Symptoms { get; set; } = string.Empty;
            public string Diagnosis { get; set; } = string.Empty;
            public string? DoctorNotes { get; set; }
            public List<PrescriptionItemDto> Prescriptions { get; set; } = new();
            public List<LabRequestItemDto> LabRequests { get; set; } = new();
        }

        // ─────────────────────────────────────────────
        //  SHARED / UTILITY DTOs
        // ─────────────────────────────────────────────

        /// <summary>Generic API wrapper used for all responses.</summary>
        public class ApiResponseDto<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
            public T? Data { get; set; }
            public List<string> Errors { get; set; } = new();

            public static ApiResponseDto<T> Ok(T data, string message = "Success") =>
                new() { Success = true, Message = message, Data = data };

            public static ApiResponseDto<T> Fail(string message, List<string>? errors = null) =>
                new() { Success = false, Message = message, Errors = errors ?? new() };
        }

        /// <summary>Stock validation result from Pharmacist module.</summary>
        public class StockValidationResult
        {
            public bool IsValid { get; set; }
            public int MedicineId { get; set; }
            public string MedicineName { get; set; } = string.Empty;
            public int RequestedQty { get; set; }
            public int AvailableStock { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }
