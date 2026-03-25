using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface IPharmacistRepository
    {

        //dfihbgvodygfbvpbhsdbfpppppppibgsdphibgvshdbvdsbhvhvbgv
        //fnrydfvos8ybhpvfgrvbgyvbfgvbfgvbfgvbgfypgvb
        //hurgfvdhpgerfbvugdbrgehfhbvuregbfregybuufergy9frgedbfrehgudyerfhudrefhud  




        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetMedicineByIdAsync(int id);
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine> UpdateMedicineAsync(Medicine medicine);
    }
}
