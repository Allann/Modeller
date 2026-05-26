using Modeller.Parser.Ast;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Modeller.Parser.Parsers;

/// <summary>
/// Parsers for entity definitions
/// </summary>
public static class EntityParsers
{
    /// <summary>
    /// Parses a single modifier like: , optional or , default(value)
    /// </summary>
    private static Parser<char, (bool IsOptional, string? DefaultValue)> SingleModifier { get; } =
        Try(
            TokenParsers.Comma
                .Then(
                    Try(TokenParsers.Keyword("optional").Select(_ => (true, (string?)null)))
                        .Or(TokenParsers.Keyword("default")
                            .Then(TokenParsers.InParens(
                                TokenParsers.Identifier.Or(TokenParsers.QuotedString).Or(TokenParsers.Boolean.Select(b => b.ToString().ToLower()))))
                            .Select(v => (false, (string?)v)))
                )
        );

    /// <summary>
    /// Parses attribute modifiers like: optional, default(value)
    /// </summary>
    private static Parser<char, (bool IsOptional, string? DefaultValue)> AttributeModifiers { get; } =
        SingleModifier.Many()
            .Select(mods =>
            {
                var isOptional = false;
                string? defaultValue = null;
                foreach (var (opt, def) in mods)
                {
                    if (opt) isOptional = true;
                    if (def != null) defaultValue = def;
                }
                return (isOptional, defaultValue);
            });

    /// <summary>
    /// Parses an attribute line like: Name: text(100), optional "Description"
    /// </summary>
    public static Parser<char, AttributeNode> Attribute { get; } =
        Map(
            (name, dataType, mods, _, desc) => new AttributeNode(
                name,
                dataType.TypeName,
                desc.GetValueOrDefault(),
                dataType.MaxLength,
                dataType.Precision,
                dataType.Scale,
                mods.HasValue && mods.Value.IsOptional,
                mods.HasValue ? mods.Value.DefaultValue : null),
            TokenParsers.Identifier.Before(TokenParsers.Colon),
            DataTypeParsers.DataType,
            AttributeModifiers.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional()
        );

    /// <summary>
    /// Parses a relationship type keyword
    /// </summary>
    private static Parser<char, RelationshipType> RelationshipTypeParser { get; } =
        Try(TokenParsers.Keyword("has_one")).Select(_ => RelationshipType.HasOne)
            .Or(Try(TokenParsers.Keyword("has_many")).Select(_ => RelationshipType.HasMany))
            .Or(Try(TokenParsers.Keyword("belongs_to")).Select(_ => RelationshipType.BelongsTo))
            .Or(Try(TokenParsers.Keyword("many_to_many")).Select(_ => RelationshipType.ManyToMany))
            .Or(Try(TokenParsers.Keyword("references")).Select(_ => RelationshipType.References));

    /// <summary>
    /// Parses an alias like: as Parameters
    /// </summary>
    private static Parser<char, string> Alias { get; } =
        Try(
            TokenParsers.SkipWhitespaceAndComments
                .Then(TokenParsers.Keyword("as"))
                .Then(TokenParsers.SkipWhitespaceAndComments)
                .Then(TokenParsers.Identifier)
        );

    /// <summary>
    /// Parses relationship target forms: Target, Target?, or Alias: Target?
    /// </summary>
    private static Parser<char, (string Target, string? Alias)> RelationshipTarget { get; } =
        Try(
            Map(
                (alias, _, target, __) => (Target: target, Alias: (string?)alias),
                TokenParsers.Identifier,
                TokenParsers.Colon,
                TokenParsers.Identifier,
                Char('?').Optional()
            )
        ).Or(
            Map(
                (target, _) => (Target: target, Alias: (string?)null),
                TokenParsers.Identifier,
                Char('?').Optional()
            )
        );

    /// <summary>
    /// Parses a relationship line like: has_many Attribute as Parameters
    /// </summary>
    public static Parser<char, RelationshipNode> Relationship { get; } =
        Try(
            Map(
                (relType, _, relTarget, asAlias, __, desc) =>
                    new RelationshipNode(relTarget.Target, relType, asAlias.GetValueOrDefault() ?? relTarget.Alias, desc.GetValueOrDefault()),
                RelationshipTypeParser,
                TokenParsers.SkipWhitespaceAndComments,
                RelationshipTarget,
                Alias.Optional(),
                TokenParsers.SkipWhitespaceAndComments,
                TokenParsers.QuotedString.Optional()
            )
        );

    /// <summary>
    /// Parses an entity body line (either attribute or relationship)
    /// </summary>
    private static Parser<char, object> EntityBodyLine { get; } =
        Relationship.Select(r => (object)r)
            .Or(Try(Attribute).Select(a => (object)a));

    /// <summary>
    /// Parses a complete entity definition
    /// </summary>
    public static Parser<char, EntityNode> Entity { get; } =
        Map(
            (_, __, name, ___, desc, ____, lines) =>
            {
                var attrs = lines.OfType<AttributeNode>().ToList();
                var rels = lines.OfType<RelationshipNode>().ToList();
                return new EntityNode(
                    name,
                    desc.GetValueOrDefault(),
                    attrs.Count > 0 ? attrs : null,
                    rels.Count > 0 ? rels : null);
            },
            TokenParsers.Keyword("entity"),
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.Identifier,
            TokenParsers.SkipWhitespaceAndComments,
            TokenParsers.QuotedString.Optional(),
            TokenParsers.SkipWhitespaceAndComments,
            EntityBodyLine.Before(TokenParsers.SkipWhitespaceAndComments).Until(TokenParsers.End_)
        );

    /// <summary>
    /// Parses an entity file which may contain both entity and key definitions
    /// </summary>
    public static Parser<char, EntityNode> EntityFile { get; } =
        Map(
            (entity, _, key) => key.HasValue
                ? entity with { Key = key.Value }
                : entity,
            Entity,
            TokenParsers.SkipWhitespaceAndComments,
            KeyParsers.Key.Optional()
        );
}

