# StartSyncJob Command
# Starts a background sync job for units

command StartSyncJob
  "Starts a background sync job to synchronize units from external source"
  
  input
    # Job identification
    JobName: text(100), optional "Optional name for the job"
    
    # Sync options
    SyncUnits: boolean, default(true) "Whether to sync units"
    CreateNew: boolean, default(true) "Whether to create new records that don't exist"
    UpdateExisting: boolean, default(true) "Whether to update existing records"
  end
  
  output
    JobId: guid "Unique identifier for the sync job"
    Message: text(200) "Message about the job start"
    StartedAt: datetimeoffset "When the job was started"
  end
  
  errors
    SyncAlreadyRunning "A sync job is already running"
    ConfigurationError "Invalid sync configuration"
  end
end

