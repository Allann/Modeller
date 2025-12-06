# CreateUnit Command
# Creates a new unit (physical truck)

command CreateUnit
  "Creates a new unit (physical truck) in the system"

  input
    TruckNumber: text(50) "The truck number (business key)"
    RegistrationNumber: text(20), optional "Vehicle registration number"
    Description: text(200), optional "Description of the unit"
    Make: text(50), optional "Vehicle make"
    Model: text(100), optional "Vehicle model"
    TruckType: TruckType, optional "Type of truck for operational classification"
    EuroType: text(20), optional "Euro emission standard type"
    EngineNumber: text(50), optional "Engine number"
    ChassisNumber: text(50), optional "Chassis number"
    WarrantyDate: datetime, optional "Warranty expiration date"
    State: text(10), optional "State or territory where unit operates"
    Company: text(20), optional "Company code for organisation linkage"
    Department: text(50), optional "Department within the company"
    Activity: text(50), optional "Activity or operational area"
    CountryCode: text(5), optional "Country code"
    DCN: text(50), optional "DCN identifier"
    Extra: text(500), optional "Additional information"
    Active: boolean, optional "Determines if the unit is active or not"
  end

  output
    Unit "The created unit"
  end

  errors
    DuplicateTruckNumber "A unit with this truck number already exists"
    ValidationError "Input validation failed"
  end
end

