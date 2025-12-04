using Modeller.Parser.Ast;

namespace Modeller.Domain;

/// <summary>
/// Builds a semantic Domain model from parsed AST nodes
/// </summary>
public sealed class DomainBuilder
{
    private readonly List<EntityNode> _entityNodes = [];
    private readonly List<EnumNode> _enumNodes = [];
    private readonly List<ServiceNode> _serviceNodes = [];
    private readonly List<CommandNode> _commandNodes = [];
    private readonly List<QueryNode> _queryNodes = [];
    private DomainNode? _domainNode;

    /// <summary>
    /// Adds a parsed domain definition
    /// </summary>
    public DomainBuilder AddDomain(DomainNode node)
    {
        _domainNode = node;
        return this;
    }

    /// <summary>
    /// Adds a parsed entity definition
    /// </summary>
    public DomainBuilder AddEntity(EntityNode node)
    {
        _entityNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Adds a parsed enum definition
    /// </summary>
    public DomainBuilder AddEnum(EnumNode node)
    {
        _enumNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Adds a parsed service definition
    /// </summary>
    public DomainBuilder AddService(ServiceNode node)
    {
        _serviceNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Adds a parsed command definition
    /// </summary>
    public DomainBuilder AddCommand(CommandNode node)
    {
        _commandNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Adds a parsed query definition
    /// </summary>
    public DomainBuilder AddQuery(QueryNode node)
    {
        _queryNodes.Add(node);
        return this;
    }

    /// <summary>
    /// Builds the semantic domain model, resolving all references
    /// </summary>
    public Domain Build()
    {
        // Build entities first (other things reference them)
        var entities = _entityNodes.Select(BuildEntity).ToList();
        var enums = _enumNodes.Select(BuildEnum).ToList();
        var commands = _commandNodes.Select(BuildCommand).ToList();
        var queries = _queryNodes.Select(BuildQuery).ToList();
        var services = _serviceNodes.Select(BuildService).ToList();

        var domain = new Domain(
            Name: _domainNode?.Name ?? "Unknown",
            Description: _domainNode?.Description,
            Version: _domainNode?.Version,
            Entities: entities,
            Enums: enums,
            Commands: commands,
            Queries: queries,
            Services: services);

        // Resolve references and return new immutable domain
        return ResolveReferences(domain);
    }

    private static Entity BuildEntity(EntityNode node) => new(
        Name: node.Name,
        Description: node.Description,
        Attributes: node.Attributes?.Select(BuildAttribute).ToList(),
        Relationships: node.Relationships?.Select(BuildRelationship).ToList(),
        Key: node.Key is not null ? BuildKey(node.Key) : null);

    private static Attribute BuildAttribute(AttributeNode node) => new(
        Name: node.Name,
        DataType: new DataType(TypeName: node.DataType, Length: node.MaxLength),
        Description: node.Description,
        IsRequired: !node.IsOptional,
        DefaultValue: node.DefaultValue);

    private static Relationship BuildRelationship(RelationshipNode node) => new(
        Type: node.Type switch
        {
            Parser.Ast.RelationshipType.HasOne => RelationshipType.HasOne,
            Parser.Ast.RelationshipType.HasMany => RelationshipType.HasMany,
            Parser.Ast.RelationshipType.BelongsTo => RelationshipType.BelongsTo,
            Parser.Ast.RelationshipType.ManyToMany => RelationshipType.HasMany,
            _ => RelationshipType.HasOne
        },
        TargetEntityName: node.TargetEntity,
        Alias: node.Alias);

    private static Key BuildKey(KeyNode node) => new(
        Fields: node.Fields?.Select(f => new KeyField(
            Name: f.Name,
            TypeName: f.DataType,
            IsGenerated: f.IsGenerated)).ToList(),
        Indexes: node.Indexes?.Select(i => new Index(
            Fields: i.Fields?.ToList(),
            IsUnique: i.IsUnique)).ToList());

    private static Enumeration BuildEnum(EnumNode node) => new(
        Name: node.Name,
        Description: node.Description,
        Values: node.Values?.Select(v => new EnumValue(
            Name: v.Name,
            Description: v.Description,
            Value: v.Value)).ToList());

    private static Command BuildCommand(CommandNode node) => new(
        Name: node.Name,
        Description: node.Description,
        Inputs: node.Inputs?.Select(BuildAttribute).ToList(),
        OutputTypeName: node.Output,
        Errors: node.Errors?.Select(e => new ErrorType(
            Name: e.Name,
            Description: e.Description)).ToList(),
        Events: node.Events?.ToList());

    private static Query BuildQuery(QueryNode node) => new(
        Name: node.Name,
        Description: node.Description,
        Inputs: node.Inputs?.Select(BuildAttribute).ToList(),
        OutputTypeName: node.Returns,
        ReturnsCollection: node.ReturnsMany);

    private static Service BuildService(ServiceNode node) => new(
        Name: node.Name,
        Description: node.Description);

    /// <summary>
    /// Resolves all cross-references in the domain, returning a new immutable domain
    /// </summary>
    private static Domain ResolveReferences(Domain domain)
    {
        // Resolve entities with their relationships resolved
        var resolvedEntities = domain.Entities
            .Select(entity => entity with
            {
                Relationships = entity.Relationships
                    .Select(rel => rel with { TargetEntity = domain.GetEntity(rel.TargetEntityName) })
                    .ToList()
            })
            .ToList();

        // Resolve commands with output entities
        var resolvedCommands = domain.Commands
            .Select(cmd => cmd with
            {
                OutputEntity = cmd.OutputTypeName is not null
                    ? domain.GetEntity(cmd.OutputTypeName)
                    : null
            })
            .ToList();

        // Resolve queries with output entities
        var resolvedQueries = domain.Queries
            .Select(q => q with
            {
                OutputEntity = q.OutputTypeName is not null
                    ? domain.GetEntity(q.OutputTypeName)
                    : null
            })
            .ToList();

        // Create resolved domain
        var resolvedDomain = domain with
        {
            Entities = resolvedEntities,
            Commands = resolvedCommands,
            Queries = resolvedQueries
        };

        // Resolve services with parent domain reference
        var resolvedServices = domain.Services
            .Select(svc => svc with { Domain = resolvedDomain })
            .ToList();

        return resolvedDomain with { Services = resolvedServices };
    }
}

