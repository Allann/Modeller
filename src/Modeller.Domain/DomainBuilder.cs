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
        var entities = _entityNodes.Select(BuildEntity).Where(e => e is not null).Cast<Entity>().ToList();
        var enums = _enumNodes.Select(BuildEnum).Where(e => e is not null).Cast<Enumeration>().ToList();
        var commands = _commandNodes.Select(BuildCommand).Where(c => c is not null).Cast<Command>().ToList();
        var queries = _queryNodes.Select(BuildQuery).Where(q => q is not null).Cast<Query>().ToList();
        var services = _serviceNodes.Select(BuildService).Where(s => s is not null).Cast<Service>().ToList();

        var domain = Domain.New(
            name: _domainNode?.Name ?? "Unknown",
            description: _domainNode?.Description,
            version: _domainNode?.Version,
            entities: entities,
            enums: enums,
            commands: commands,
            queries: queries,
            services: services) ?? throw new InvalidOperationException("Failed to create domain");

        // Resolve references and return new immutable domain
        return ResolveReferences(domain);
    }

    private static Entity? BuildEntity(EntityNode node) => Entity.New(
        name: node.Name,
        description: node.Description,
        attributes: node.Attributes?.Select(BuildAttribute).Where(a => a is not null).Cast<Attribute>().ToList(),
        relationships: node.Relationships?.Select(BuildRelationship).Where(r => r is not null).Cast<Relationship>().ToList(),
        key: node.Key is not null ? BuildKey(node.Key) : null);

    private static Attribute? BuildAttribute(AttributeNode node) => Attribute.New(
        name: node.Name,
        dataType: DataType.New(
            typeName: node.DataType,
            length: node.MaxLength,
            precision: node.Precision,
            scale: node.Scale) ?? throw new InvalidOperationException($"Invalid data type: {node.DataType}"),
        description: node.Description,
        isRequired: !node.IsOptional,
        defaultValue: node.DefaultValue);

    private static Relationship? BuildRelationship(RelationshipNode node) => Relationship.New(
        type: node.Type switch
        {
            Parser.Ast.RelationshipType.HasOne => RelationshipType.HasOne,
            Parser.Ast.RelationshipType.HasMany => RelationshipType.HasMany,
            Parser.Ast.RelationshipType.BelongsTo => RelationshipType.BelongsTo,
            Parser.Ast.RelationshipType.ManyToMany => RelationshipType.HasMany,
            _ => RelationshipType.HasOne
        },
        targetEntityName: node.TargetEntity,
        alias: node.Alias);

    private static Key BuildKey(KeyNode node) => Key.New(
        fields: node.Fields?.Select(f => KeyField.New(
            name: f.Name,
            typeName: f.DataType,
            isGenerated: f.IsGenerated)).Where(k => k is not null).Cast<KeyField>().ToList(),
        indexes: node.Indexes?.Select(i => Index.New(
            fields: i.Fields?.ToList(),
            isUnique: i.IsUnique)).ToList());

    private static Enumeration? BuildEnum(EnumNode node) => Enumeration.New(
        name: node.Name,
        description: node.Description,
        values: node.Values?.Select(v => EnumValue.New(
            name: v.Name,
            description: v.Description,
            value: v.Value)).Where(e => e is not null).Cast<EnumValue>().ToList());

    private static Command? BuildCommand(CommandNode node) => Command.New(
        name: node.Name,
        description: node.Description,
        inputs: node.Inputs?.Select(BuildAttribute).Where(a => a is not null).Cast<Attribute>().ToList(),
        outputTypeName: node.Output,
        errors: node.Errors?.Select(e => ErrorType.New(
            name: e.Name,
            description: e.Description)).Where(e => e is not null).Cast<ErrorType>().ToList(),
        events: node.Events?.ToList());

    private static Query? BuildQuery(QueryNode node) => Query.New(
        name: node.Name,
        description: node.Description,
        inputs: node.Inputs?.Select(BuildAttribute).Where(a => a is not null).Cast<Attribute>().ToList(),
        outputTypeName: node.Returns,
        returnsCollection: node.ReturnsMany);

    private static Service? BuildService(ServiceNode node) => Service.New(
        name: node.Name,
        description: node.Description);

    /// <summary>
    /// Resolves all cross-references in the domain, returning a new immutable domain
    /// </summary>
    private static Domain ResolveReferences(Domain domain)
    {
        // Resolve entities with their relationships resolved
        var resolvedEntities = domain.Entities
            .Select(entity => Entity.CreateValid(
                entity.PublicId,
                entity.Name,
                entity.Description,
                entity.IsAggregateRoot,
                entity.Service,
                entity.Attributes,
                entity.Relationships
                    .Select(rel => Relationship.New(
                        rel.Type,
                        rel.TargetEntityName,
                        rel.Alias,
                        domain.GetEntity(rel.TargetEntityName)))
                    .Where(r => r is not null)
                    .Cast<Relationship>()
                    .ToList(),
                entity.Key)!)
            .ToList();

        // Resolve commands with output entities
        var resolvedCommands = domain.Commands
            .Select(cmd => Command.CreateValid(
                cmd.PublicId,
                cmd.Name,
                cmd.Description,
                cmd.Inputs,
                cmd.OutputTypeName,
                cmd.OutputTypeName is not null ? domain.GetEntity(cmd.OutputTypeName) : null,
                cmd.Errors,
                cmd.Events)!)
            .ToList();

        // Resolve queries with output entities
        var resolvedQueries = domain.Queries
            .Select(q => Query.CreateValid(
                q.PublicId,
                q.Name,
                q.Description,
                q.Inputs,
                q.OutputTypeName,
                q.ReturnsCollection,
                q.OutputTypeName is not null ? domain.GetEntity(q.OutputTypeName) : null)!)
            .ToList();

        // Create resolved domain
        var resolvedDomain = Domain.CreateValid(
            domain.PublicId,
            domain.Name,
            domain.Description,
            domain.Version,
            domain.Services,
            resolvedEntities,
            domain.Enums,
            resolvedCommands,
            resolvedQueries) ?? throw new InvalidOperationException("Failed to create resolved domain");

        // Resolve services with parent domain reference
        var resolvedServices = domain.Services
            .Select(svc => Service.CreateValid(
                svc.PublicId,
                svc.Name,
                svc.Description,
                resolvedDomain,
                svc.Entities,
                svc.Commands,
                svc.Queries)!)
            .ToList();

        return Domain.CreateValid(
            resolvedDomain.PublicId,
            resolvedDomain.Name,
            resolvedDomain.Description,
            resolvedDomain.Version,
            resolvedServices,
            resolvedDomain.Entities,
            resolvedDomain.Enums,
            resolvedDomain.Commands,
            resolvedDomain.Queries) ?? throw new InvalidOperationException("Failed to create final domain");
    }
}

