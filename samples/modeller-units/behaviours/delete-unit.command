# DeleteUnit Command
# Deletes a unit by its ID

command DeleteUnit
  "Deletes a unit from the system"
  
  input
    UnitId: guid, required "The unique identifier for the unit to delete"
  end
  
  output
    Success: boolean "Whether the deletion was successful"
  end
  
  errors
    NotFound "Unit not found"
  end
end

