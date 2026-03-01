using System;
using System.ComponentModel.DataAnnotations;

namespace Munientry.Client.Shared.Models
{
    public class NotGuiltyAppearBondSpecialDto
    {
        [Required]
        public string? DefendantFirstName { get; set; }
        [Required]
        public string? DefendantLastName { get; set; }
        [Required]
        public string? CaseNumber { get; set; }
        public string? BondType { get; set; }
        public string? BondAmount { get; set; }
        public string? BondModificationDecision { get; set; }
        public string? NoContactName { get; set; }
        public string? CustodialSupervisionSupervisor { get; set; }
        public string? AdminLicenseSuspensionObjection { get; set; }
        public string? AdminLicenseSuspensionDisposition { get; set; }
        public string? AdminLicenseSuspensionExplanation { get; set; }
        public string? VehicleMakeModel { get; set; }
        public string? VehicleLicensePlate { get; set; }
        public bool TowToResidence { get; set; }
        public bool MotionToReturnVehicle { get; set; }
        public string? StateOpposes { get; set; }
        public string? DispositionMotionToReturn { get; set; }
        public bool VacateResidence { get; set; }
        public string? ResidenceAddress { get; set; }
        public string? ExclusivePossessionTo { get; set; }
        public bool SurrenderWeapons { get; set; }
        public string? SurrenderWeaponsDate { get; set; }
    }
}
