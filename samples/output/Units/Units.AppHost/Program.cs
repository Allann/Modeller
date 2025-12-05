using Units.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

var unitsDb = builder.AddConnectionString("Realm-Unit");

// Migrations now run on API startup via DbMigrator<UnitsManagementDbContext>
var unitsApi = builder.AddProject<Projects.JJs_UnitsManagement_Api>("units-api")
    .WithReference(unitsDb)
    .WithSwagger()
    .WithReDoc()
    .WithScalar();

builder.AddProject<Projects.Shell>("shell-ui")
    .WithReference(unitsApi)
    .WaitFor(unitsApi);

builder.Build().Run();
