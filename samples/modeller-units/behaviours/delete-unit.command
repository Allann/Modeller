# DeleteUnit Command
# Deletes a unit by its ID

command DeleteUnit
  "Deletes a unit from the system"

  input
    UnitId: guid "The unique identifier for the unit to delete"
  end

  output
    Unit "The deleted unit"
  end

  errors
    NotFound "Unit not found"
  end
end

