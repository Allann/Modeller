# GenerateOutput Command
# Runs a generator against a domain

command GenerateOutput
  "Executes a generator to produce output from a domain"
  
  input
    Domain: Domain "The domain to generate from"
    Generator: Generator "The generator to run"
    OutputPath: text(500), optional "Override output path"
    DryRun: boolean, default(false) "Preview without writing"
  end
  
  output
    GeneratorRun "The execution record"
  end
  
  errors
    TemplateError "Error executing template"
    OutputError "Error writing output"
  end
  
  publishes
    GenerationStarted
    GenerationCompleted
    GenerationFailed
  end
end

