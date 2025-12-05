using Modeller.Domain;
using Modeller.Generator;

namespace Modeller.Generator.Tests;

public class ScribanDomainGeneratorTests
{
    private static readonly string TemplatesPath = GetTemplatesPath();
    private readonly ScribanDomainGenerator _generator = new("TestDomain", TemplatesPath);

    private static string GetTemplatesPath()
    {
        // Navigate from test output to templates folder
        var current = Directory.GetCurrentDirectory();
        var root = current;
        while (!Directory.Exists(Path.Combine(root, "templates")) && Directory.GetParent(root) != null)
        {
            root = Directory.GetParent(root)!.FullName;
        }
        return Path.Combine(root, "templates");
    }

    private static Modeller.Domain.Domain CreateTestDomain(
        IReadOnlyList<Entity>? entities = null,
        IReadOnlyList<Enumeration>? enums = null,
        IReadOnlyList<Command>? commands = null,
        IReadOnlyList<Query>? queries = null)
    {
        return Modeller.Domain.Domain.New(
            "TestDomain",
            "A test domain for generator verification",
            "1.0.0",
            entities: entities,
            enums: enums,
            commands: commands,
            queries: queries)!;
    }

    [Fact]
    public Task GeneratesSimpleEntity()
    {
        var entity = Entity.New(
            "Customer",
            "Represents a customer in the system")!;

        var domain = CreateTestDomain(entities: [entity]);
        var files = _generator.Generate(domain).ToList();

        var entityFile = files.First(f => f.Path == "Customer.cs");
        return Verifier.Verify(entityFile.Content);
    }

    [Fact]
    public Task GeneratesEntityWithAttributes()
    {
        var entity = Entity.New(
            "Product",
            "Represents a product",
            attributes:
            [
                Modeller.Domain.Attribute.New("Code", DataType.New("name")!)!,
                Modeller.Domain.Attribute.New("Price", DataType.New("decimal")!, isRequired: true)!,
                Modeller.Domain.Attribute.New("StockLevel", DataType.New("integer")!, isRequired: true)!,
                Modeller.Domain.Attribute.New("Notes", DataType.New("text")!, isRequired: false)!,
                Modeller.Domain.Attribute.New("IsActive", DataType.New("boolean")!, isRequired: true, defaultValue: "true")!
            ])!;

        var domain = CreateTestDomain(entities: [entity]);
        var files = _generator.Generate(domain).ToList();

        var entityFile = files.First(f => f.Path == "Product.cs");
        return Verifier.Verify(entityFile.Content);
    }

    [Fact]
    public Task GeneratesEntityWithRelationships()
    {
        var entity = Entity.New(
            "Order",
            "Represents a customer order",
            relationships:
            [
                Relationship.New(RelationshipType.BelongsTo, "Customer")!,
                Relationship.New(RelationshipType.HasMany, "OrderLine", "Lines")!,
                Relationship.New(RelationshipType.HasOne, "ShippingAddress", "Address")!
            ])!;

        var domain = CreateTestDomain(entities: [entity]);
        var files = _generator.Generate(domain).ToList();

        var entityFile = files.First(f => f.Path == "Order.cs");
        return Verifier.Verify(entityFile.Content);
    }

    [Fact]
    public Task GeneratesEnumeration()
    {
        var enumeration = Enumeration.New(
            "OrderStatus",
            "Status of an order",
            values:
            [
                EnumValue.New("Pending", "Order is pending", 0)!,
                EnumValue.New("Confirmed", "Order is confirmed", 1)!,
                EnumValue.New("Shipped", "Order has shipped", 2)!,
                EnumValue.New("Delivered", "Order delivered", 3)!,
                EnumValue.New("Cancelled", "Order was cancelled", 99)!
            ])!;

        var domain = CreateTestDomain(enums: [enumeration]);
        var files = _generator.Generate(domain).ToList();

        var enumFile = files.First(f => f.Path == "OrderStatus.cs");
        return Verifier.Verify(enumFile.Content);
    }

    [Fact]
    public Task GeneratesCommand()
    {
        var command = Command.New(
            "CreateOrder",
            "Creates a new order in the system")!;

        var domain = CreateTestDomain(commands: [command]);
        var files = _generator.Generate(domain).ToList();

        var commandFile = files.First(f => f.Path == "CreateOrder.cs");
        return Verifier.Verify(commandFile.Content);
    }

    [Fact]
    public Task GeneratesQuery()
    {
        var query = Query.New(
            "GetOrdersByCustomer",
            "Retrieves all orders for a customer")!;

        var domain = CreateTestDomain(queries: [query]);
        var files = _generator.Generate(domain).ToList();

        var queryFile = files.First(f => f.Path == "GetOrdersByCustomer.cs");
        return Verifier.Verify(queryFile.Content);
    }
}

