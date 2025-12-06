# StartSyncJob Command
# Starts a background sync job for units

command StartSyncJob
  "Starts a background sync job to synchronize units from external source"

  input
    JobName: text(100), optional "Optional name for the job"
    SyncUnits: boolean, optional "Whether to sync units"
    CreateNew: boolean, optional "Whether to create new records that don't exist"
    UpdateExisting: boolean, optional "Whether to update existing records"
  end

  output
    SyncJob "The created sync job"
  end

  errors
    SyncAlreadyRunning "A sync job is already running"
    ConfigurationError "Invalid sync configuration"
  end
end

