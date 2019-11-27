using DFEngine.Core.Models;
using System.Collections.Generic;

namespace DFEngine.Core.UnitTests
{
    class MockProvider
    {
        internal Node Table_01 { get; }

        internal Node Table_02 { get; }

        internal Node Table_03 { get; }

        internal Node Table_04 { get; }

        internal Node Table_05 { get; }
        internal Node Table_06 { get; }

        internal Node File_01 { get; }

        internal Node File_02 { get; }

        internal Node Report_01 { get; }

        internal Node Report_02 { get; }

        internal Node Database_01 { get; }

        internal Node Database_02 { get; }

        internal Node Database_03 { get; }
        internal Node Database_04 { get; }

        internal Node Server_01 { get; }

        internal Node Server_02 { get; }

        internal Node ReportService_01 { get; }

        internal Graph Graph_01 { get; }

        internal Graph Graph_02 { get; }

        internal MockProvider()
        {
            Graph_01 = new Graph();
            Graph_02 = new Graph();

            Table_01 = new Node("table_01", "table");
            Table_02 = new Node("table_02", "table");
            Table_03 = new Node("table_03", "table");
            Table_04 = new Node("table_01", "table");
            Table_05 = new Node("table_04", "table");
            Table_06 = new Node("table_05", "table");

            Database_01 = new Node("db_01", "database");
            Database_01.AddChild(Table_01);
            Database_01.AddChild(Table_02);

            Database_02 = new Node("db_02", "database");
            Database_02.AddChild(Table_03);

            Database_03 = new Node("db_01", "database");
            Database_03.AddChild(Table_04);
            Database_03.AddChild(Table_05);

            Database_04 = new Node("db_03", "database");
            Database_04.AddChild(Table_06);

            Server_01 = new Node("server_01", "sqlserver");
            Server_01.AddChild(Database_01);
            Server_01.AddChild(Database_02);

            Server_02 = new Node("server_01", "sqlserver");
            Server_02.AddChild(Database_03);
            Server_02.AddChild(Database_04);


            File_01 = new Node("file_01", "file");
            File_02 = new Node("file_01", "file");
            
            Report_01 = new Node("report_01", "report");
            Report_02 = new Node("report_02", "report");

            ReportService_01 = new Node("reportService_01", "ssrs");
            ReportService_01.AddChild(Report_01);
            ReportService_01.AddChild(Report_02);

            Graph_01.AddNode(Server_01);
            Graph_01.AddNode(File_01);

            Graph_02.AddNode(Server_02);
            Graph_02.AddNode(File_02);
            Graph_02.AddNode(ReportService_01);

            Graph_02.AddEdge(new Edge(Table_04, Report_01, "testEnvironment"));
            Graph_02.AddEdge(new Edge(Table_04, Report_02, "testEnvironment"));
            Graph_02.AddEdge(new Edge(Table_05, Report_02, "testEnvironment"));
            Graph_02.AddEdge(new Edge(File_02, Report_01, "testEnvironment"));
            Graph_02.AddEdge(new Edge(Table_04, Table_06, "testEnvironment"));
            Graph_02.AddEdge(new Edge(Table_06, Report_01, "testEnvironment"));
        }
    }
}
