/*
{
    "TaskCategory": "Plugins",
    "TaskName": "GraphTool Demo",
    "TaskDetail": "A test script for SQLite in-memory database with Mermaid diagram generation",
    "RequiredPlugins": ["GraphTool"]
}
*/

// Get the SqLiteDatabase service from the DI container
var db = new SqliteDatabase();

// Set up the connection string for the in-memory database
db.Setup("Data Source=:memory:");

// Open the database connection
db.OpenConnection();

// Create multiple entities with relationships
string createTablesQuery = @"
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Address TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId INTEGER NOT NULL,
    OrderDate TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE IF NOT EXISTS Products (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Price REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS OrderItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderId INTEGER NOT NULL,
    ProductId INTEGER NOT NULL,
    Quantity INTEGER NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);";
db.ExecuteNonQuery(createTablesQuery);

// Insert some records into the tables
string insertDataQuery = @"
INSERT INTO Users (Name, Address) VALUES 
('John Doe', '123 Elm Street'),
('Jane Smith', '456 Oak Avenue');

INSERT INTO Orders (UserId, OrderDate) VALUES
(1, '2024-10-01'),
(2, '2024-10-02');

INSERT INTO Products (Name, Price) VALUES
('Widget', 19.99),
('Gadget', 29.99);

INSERT INTO OrderItems (OrderId, ProductId, Quantity) VALUES
(1, 1, 2),
(1, 2, 1),
(2, 1, 1);
";
db.ExecuteNonQuery(insertDataQuery);

// Query and dump all users
var usersTable = db.ExecuteQuery("SELECT * FROM Users");
DumpTable("Users:", usersTable);

// Load entities and relationships from the database
var entities = db.LoadEntities();
var relationships = db.LoadRelationships();

// Specify the entity list to filter (e.g., filter by the 'Orders' entity)
string[] entityFilter = new string[] { 
    "Orders", "OrderItems", "Products"
};

// Generate graph data
var logger = GetLogger("GraphPluginManager");
var pluginManager = new GraphPluginManager(logger: logger, erdPlugin: new ErdPlugin());
var graphTool = new GraphTool(pluginManager);
var graphData = graphTool.CreateGraph(PluginType.Erd, entities, relationships, entityFilter);

// Render Mermaid diagram
var graphVisualisation = new GraphVisualization();
var mermaidDiagram = graphVisualisation.Render(graphData, TemplateType.MermaidErd);

// Dump the Mermaid diagram
Dump(mermaidDiagram);

// Close the database connection
db.CloseConnection();

return $"Task completed successfully with Mermaid diagram generated.";