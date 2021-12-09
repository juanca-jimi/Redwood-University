using System;
using Microsoft.Data.SqlClient;

namespace embeddedSQL421
{
    class Program
    {
        //----------------------------------------------------------------------
        static void Main(string[] args)
        {
            //---------------------------
            //---------------------------
            //---------------------------
            try
            {    
                SqlConnectionStringBuilder builder =
                        new SqlConnectionStringBuilder();

                builder.DataSource = "421db.database.windows.net";
                builder.UserID = "redwoodAdmin";
                builder.Password = "Adminadmin1";
                builder.InitialCatalog = "RedwoodUniversityDB";

                using (var connection =
                    new SqlConnection(builder.ConnectionString))
                {
                    //-------------------------------------------------
                    //Connecting to AzureDB instance 
                    //-------------------------------------------------
                    connection.Open();

                    Submit_Tsql_NonQuery
                        (connection, "2 - Create-Tables",
                        Build_Tsql_CreateTables());

                    Submit_Tsql_NonQuery
                        (connection, "3 - Inserts", Build_Tsql_Inserts());

                    Tsql_Select(connection);

                    connection.Close();
                    Console.ReadLine();
                    //-------------------------------------------------
                    //Closing Connection
                    //-------------------------------------------------
                }
            }
            //---------------------------
            //---------------------------
            //---------------------------
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        //----------------------------------------------------------------------

        private static void Tsql_Select(SqlConnection connection)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("Now, SelectEmployees (6)...");

            string tsql = Build_Tsql_Select();

            using (var command = new SqlCommand(tsql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0} , {1} , {2} , {3} , {4}",
                            reader.GetGuid(0),
                            reader.GetString(1),
                            reader.GetInt32(2),
                            (reader.IsDBNull(3)) ? "NULL" : reader.GetString(3),
                            (reader.IsDBNull(4)) ? "NULL" : reader.GetString(4));
                    }
                }
            }
        }
        private static void Submit_Tsql_NonQuery(SqlConnection connection,
                                                    string tsqlPurpose,
                                                    string tsqlSourceCode,
                                                    string parameterName = null,
                                                    string parameterValue = null)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("T-SQL to {0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                if (parameterName != null)
                {
                    // Or, use SqlParameter class.
                    command.Parameters.AddWithValue(  
                        parameterName,
                        parameterValue);
                }
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
            }

        }


        //-------------------------------------------------------------
        //
        // SQL Statements
        //
        //-------------------------------------------------------------

        //CREATING ALL TABLES
        private static string Build_Tsql_CreateTables()
        {
            return @"
            DROP TABLE IF EXISTS EventAttendant;
            DROP TABLE IF EXISTS DeclaredMajor;
            DROP TABLE IF EXISTS EventHost;
            DROP TABLE IF EXISTS Student;
            DROP TABLE IF EXISTS Event;
            DROP TABLE IF EXISTS Major;
            DROP TABLE IF EXISTS Department;

            CREATE TABLE Department
            (
                DeptCode VarChar(3) Not Null ,
                DeptName VarChar(256) Not Null,
                DeptChair Varchar(100) Not Null,
				DeptMembers int,

				CONSTRAINT PK_Department_DeptCode Primary Key NONCLUSTERED (DeptCode) ,

                --Completing Deptartment name constraint where name must include 'Department'
                CONSTRAINT DepartmentNameConstraint CHECK(DeptName like '%Department%')
            );

            CREATE TABLE Major
            (
                --(1/2)Completing Major code 3 character constraint
                MajorCode char(3) Not Null,
				DeptCode Varchar(3) Not Null,
				
                CONSTRAINT PK_Major_MajorCode PRIMARY KEY NONCLUSTERED (MajorCode) ,

                CONSTRAINT FK_Major_DeptCode FOREIGN KEY (DeptCode) REFERENCES Department (DeptCode) on UPDATE CASCADE ON DELETE No ACTION
            );

            CREATE TABLE Student
            (
                StudentID varchar(50) Not Null,
                StudentFirstName varchar(25),
                StudentLastName VARCHAR(25),
                StudentInitials VARCHAR (3),

                CONSTRAINT PK_Student_StudentID PRIMARY KEY NONCLUSTERED (StudentID) ,

                --completing student initial (length > 1) constraint
                CONSTRAINT MinStudentInitialsLengthConstraint CHECK (DATALENGTH(StudentInitials) > 1)
            );

            CREATE TABLE Event
            (
                EventID VarChar(5) Not Null,
                EventName VARCHAR(50),
                StartDate DATE,
                EndDate DATE,

                CONSTRAINT PK_Event_EventID PRIMARY KEY NONCLUSTERED (EventID) ,

                --Completing constraints where startdate and enddate cannot be in the past
                CONSTRAINT StartDate CHECK (StartDate > GetDate() ),
                CONSTRAINT EndDateDate CHECK (EndDate > GetDate() ),

                --Completing constraint where Startdate must be less than enddate
                CONSTRAINT StartDateBeforeEndDate CHECK (StartDate < EndDate)

            );

            CREATE TABLE EventAttendant
            (
                EventID VARCHAR(5) Not Null,
                StudentID VARCHAR(50) Not Null,

                CONSTRAINT PK_EventAttendant_EventID_AND_STUDENTID PRIMARY KEY NONCLUSTERED (EventID, StudentID) ,

                CONSTRAINT FK_EventAttendant_EventID FOREIGN KEY (EventID) REFERENCES Event (EventID) on UPDATE CASCADE ON DELETE No ACTION,
                CONSTRAINT FK_EventAttendant_StudentID FOREIGN KEY (StudentID) REFERENCES Student (StudentID) on UPDATE CASCADE ON DELETE No ACTION
            );

            CREATE TABLE DeclaredMajor
            (
                StudentID VARCHAR(50) Not Null,
                --(2/2)Completing Major code 3 character constraint
                MajorCode CHAR(3) Not Null,

                CONSTRAINT PK_DeclaredMajor_MajorCode PRIMARY KEY NONCLUSTERED (MajorCode, StudentID) ,

                CONSTRAINT FK_DeclaredMajor_StudentID FOREIGN KEY (StudentID) REFERENCES Student (StudentID) on UPDATE CASCADE ON DELETE No ACTION,
                CONSTRAINT  FK_DeclaredMajor_MajorCode FOREIGN Key (MajorCode) REFERENCES Major (MajorCode) on UPDATE CASCADE ON DELETE No ACTION
            );

            CREATE TABLE EventHost
            (
                EventID VARCHAR(5) Not Null,
                DeptCode VarChar(3) Not Null ,

                CONSTRAINT PK_EventHost_EventID_And_DeptCode PRIMARY KEY NONCLUSTERED (EventID, DeptCode) ,

                CONSTRAINT FK_EventHost_EventID FOREIGN KEY (EventID) REFERENCES Event (EventID) on UPDATE CASCADE ON DELETE No ACTION,
                CONSTRAINT FK_EventHost_DeptCode FOREIGN KEY (DeptCode) REFERENCES Department (DeptCode) on UPDATE CASCADE ON DELETE No ACTION
            );
            
            ";
        }

        //INSERTING 5 
        private static string Build_Tsql_Inserts()
        {
            return @"
            
            ";
        }

        private static string Build_Tsql_Select()
        {
            return @"
            
        ";
        }

        //-------------------------------------------------------------
        //
        // End of SQL Statements
        //
        //-------------------------------------------------------------
    }
}
