using CLINICAL_MANAGEMENT.DTODoctor;
using Microsoft.Data.SqlClient;
using System.Data;
using Dapper;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public class DoctorRepoImpl : IDoctorRepository
    {
        private readonly string _connectionString;

    public DoctorRepoImpl(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    // ── Helper: open a fresh connection ───────────────────────
    private IDbConnection CreateConnection() =>
        new SqlConnection(_connectionString);

    // ══════════════════════════════════════════════════════════
    //  APPOINTMENTS
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetAppointmentsByDoctorAndDate
    /// Params: @DoctorId INT, @AppointmentDate DATE
    /// Returns appointments for today/tomorrow filtered by DoctorId.
    /// </summary>
    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAndDateAsync(
        int doctorId, DateTime date)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<AppointmentDto>(
            "usp_GetAppointmentsByDoctorAndDate",
            new { DoctorId = doctorId, AppointmentDate = date.Date },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_UpdateAppointmentStatus
    /// Params: @AppointmentId INT, @Status NVARCHAR(50)
    /// Updates status to 'InProgress' when consultation starts, 'Completed' when saved.
    /// </summary>
    public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "usp_UpdateAppointmentStatus",
            new { AppointmentId = appointmentId, Status = status },
            commandType: CommandType.StoredProcedure);
        return rows > 0;
    }

    // ══════════════════════════════════════════════════════════
    //  CONSULTATION
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetConsultationDetailByAppointment
    /// Params: @AppointmentId INT
    /// Returns patient + appointment info + existing consultation if already saved.
    /// </summary>
    public async Task<ConsultationDetailDto?> GetConsultationDetailByAppointmentAsync(int appointmentId)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<ConsultationDetailDto>(
            "usp_GetConsultationDetailByAppointment",
            new { AppointmentId = appointmentId },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_InsertConsultation
    /// Params: @AppointmentId, @PatientId, @DoctorId, @Symptoms, @Diagnosis, @DoctorNotes
    /// Returns: @ConsultationId (OUTPUT)
    /// </summary>
    public async Task<int> InsertConsultationAsync(int appointmentId, int patientId, int doctorId,
                                                    string symptoms, string diagnosis, string? doctorNotes)
    {
        using var conn = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@AppointmentId", appointmentId);
        parameters.Add("@PatientId", patientId);
        parameters.Add("@DoctorId", doctorId);
        parameters.Add("@Symptoms", symptoms);
        parameters.Add("@Diagnosis", diagnosis);
        parameters.Add("@DoctorNotes", doctorNotes);
        parameters.Add("@ConsultationId", dbType: DbType.Int32,
                       direction: ParameterDirection.Output);

        await conn.ExecuteAsync(
            "usp_InsertConsultation", parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@ConsultationId");
    }

    // ══════════════════════════════════════════════════════════
    //  PRESCRIPTION
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_InsertPrescription
    /// Params: @ConsultationId, @MedicineId, @Frequency, @Duration, @Quantity
    /// Returns: @PrescriptionId (OUTPUT)
    /// </summary>
    public async Task<int> InsertPrescriptionAsync(int consultationId, int medicineId,
                                                    int frequency, int duration, int quantity)
    {
        using var conn = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@ConsultationId", consultationId);
        parameters.Add("@MedicineId", medicineId);
        parameters.Add("@Frequency", frequency);
        parameters.Add("@Duration", duration);
        parameters.Add("@Quantity", quantity);
        parameters.Add("@PrescriptionId", dbType: DbType.Int32,
                       direction: ParameterDirection.Output);

        await conn.ExecuteAsync(
            "usp_InsertPrescription", parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@PrescriptionId");
    }

    // ══════════════════════════════════════════════════════════
    //  STOCK — PHARMACIST MODULE INTEGRATION
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetMedicineStock
    /// Params: @MedicineId INT
    /// Returns: CurrentStock INT — reads from Pharmacist's Inventory table.
    /// </summary>
    public async Task<int> GetMedicineStockAsync(int medicineId)
    {
        using var conn = CreateConnection();
        return await conn.ExecuteScalarAsync<int>(
            "usp_GetMedicineStock",
            new { MedicineId = medicineId },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_DeductMedicineStock
    /// Params: @MedicineId INT, @Quantity INT
    /// Deducts stock in Pharmacist's Inventory. Returns rows affected.
    /// </summary>
    public async Task<bool> DeductMedicineStockAsync(int medicineId, int quantity)
    {
        using var conn = CreateConnection();
        var rows = await conn.ExecuteAsync(
            "usp_DeductMedicineStock",
            new { MedicineId = medicineId, Quantity = quantity },
            commandType: CommandType.StoredProcedure);
        return rows > 0;
    }

    /// <summary>
    /// SP: usp_GetMedicineDropdown
    /// No params — returns all active medicines from Pharmacist module with stock info.
    /// </summary>
    public async Task<IEnumerable<MedicineDropdownDto>> GetMedicineDropdownAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<MedicineDropdownDto>(
            "usp_GetMedicineDropdown",
            commandType: CommandType.StoredProcedure);
    }

    // ══════════════════════════════════════════════════════════
    //  LAB TESTS — LAB TECHNICIAN MODULE INTEGRATION
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetLabTestDropdown
    /// No params — returns all available lab tests from Lab Technician module.
    /// </summary>
    public async Task<IEnumerable<LabTestDropdownDto>> GetLabTestDropdownAsync()
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<LabTestDropdownDto>(
            "usp_GetLabTestDropdown",
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_InsertLabRequest
    /// Params: @ConsultationId, @PatientId, @DoctorId, @LabTestId, @SpecialInstructions
    /// Returns: @LabRequestId (OUTPUT)
    /// </summary>
    public async Task<int> InsertLabRequestAsync(int consultationId, int patientId, int doctorId,
                                                  int labTestId, string? specialInstructions)
    {
        using var conn = CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@ConsultationId", consultationId);
        parameters.Add("@PatientId", patientId);
        parameters.Add("@DoctorId", doctorId);
        parameters.Add("@LabTestId", labTestId);
        parameters.Add("@SpecialInstructions", specialInstructions);
        parameters.Add("@LabRequestId", dbType: DbType.Int32,
                       direction: ParameterDirection.Output);

        await conn.ExecuteAsync(
            "usp_InsertLabRequest", parameters,
            commandType: CommandType.StoredProcedure);

        return parameters.Get<int>("@LabRequestId");
    }

    // ══════════════════════════════════════════════════════════
    //  PATIENT HISTORY
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetPatientHistory
    /// Params: @PatientId INT
    /// Returns all past consultations (header only — prescriptions/labs fetched separately).
    /// </summary>
    public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int patientId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<PatientHistoryDto>(
            "usp_GetPatientHistory",
            new { PatientId = patientId },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_GetPrescriptionsByConsultation
    /// Params: @ConsultationId INT
    /// Returns all prescription lines for a consultation.
    /// </summary>
    public async Task<IEnumerable<PrescriptionItemDto>> GetPrescriptionsByConsultationAsync(int consultationId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<PrescriptionItemDto>(
            "usp_GetPrescriptionsByConsultation",
            new { ConsultationId = consultationId },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_GetLabRequestsByConsultation
    /// Params: @ConsultationId INT
    /// Returns all lab requests for a consultation with current status.
    /// </summary>
    public async Task<IEnumerable<LabRequestItemDto>> GetLabRequestsByConsultationAsync(int consultationId)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<LabRequestItemDto>(
            "usp_GetLabRequestsByConsultation",
            new { ConsultationId = consultationId },
            commandType: CommandType.StoredProcedure);
    }

    // ══════════════════════════════════════════════════════════
    //  LAB RESULTS
    // ══════════════════════════════════════════════════════════

    /// <summary>
    /// SP: usp_GetLabResultsByDoctor
    /// Params: @DoctorId INT, @Date DATE
    /// Returns lab results for patients seen by this doctor on a given date.
    /// </summary>
    public async Task<IEnumerable<LabResultDto>> GetLabResultsByDoctorAsync(int doctorId, DateTime date)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<LabResultDto>(
            "usp_GetLabResultsByDoctor",
            new { DoctorId = doctorId, Date = date.Date },
            commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// SP: usp_GetLabResultByRequestId
    /// Params: @LabRequestId INT
    /// Returns single lab result with full details.
    /// </summary>
    public async Task<LabResultDto?> GetLabResultByRequestIdAsync(int labRequestId)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<LabResultDto>(
            "usp_GetLabResultByRequestId",
            new { LabRequestId = labRequestId },
            commandType: CommandType.StoredProcedure);
    }
}
}
 
    
