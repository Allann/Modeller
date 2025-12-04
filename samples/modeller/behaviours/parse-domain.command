# ParseDomain Command
# Parses definition files into a domain model

command ParseDomain
  "Parses DSL files and builds the in-memory domain model"
  
  input
    SourcePath: text(500) "Path to definition files"
    Recursive: boolean, default(true) "Include subdirectories"
  end
  
  output
    Domain "The parsed domain model"
  end
  
  errors
    FileNotFound "Source path does not exist"
    ParseError "Syntax error in definition file"
    ValidationError "Domain validation failed"
  end
end

