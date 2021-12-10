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
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

                builder.DataSource = "421db.database.windows.net";
                builder.UserID = "redwoodAdmin";
                builder.Password = "Adminadmin1";
                builder.InitialCatalog = "RedwoodUniversityDB";

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    //-------------------------------------------------
                    //Connecting to AzureDB instance 
                    //-------------------------------------------------
                    
                    Submit_Tsql_NonQuery
                        (connection, "Creating Table", Build_Tsql_CreateTables());

                    Submit_Tsql_NonQuery
                        (connection, "Inserting Rows", Build_Tsql_Inserts());

                    //1
                    Tsql_SelectEventAttendees(connection);

                    //2
                    Tsql_SelectDeclaredMajors(connection);

                    //3
                    Tsql_SelectCookoutHost(connection);

                    //4
                    Tsql_SelectGeoDeptChair(connection);

                    //5
                    Tsql_SelectEventInfo(connection);


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

        private static void Tsql_SelectCookoutHost(SqlConnection connection)
        {
            Console.WriteLine("=================================");

            using (var command = new SqlCommand(Tsql_CookoutHost(), connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("Who Hosted the Cookout?");
                Console.WriteLine("-----------------------------");
                while (reader.Read())
                {

                    Console.WriteLine("{0}", reader.GetString(0));
                }
                connection.Close();
            }
        }

        private static void Tsql_SelectEventInfo(SqlConnection connection)
        {
            Console.WriteLine("=================================");

            using (var command = new SqlCommand(Tsql_EventInfo(), connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("EventName\tStartDate\tEndDate");
                Console.WriteLine("---------------------------------");
                while (reader.Read())
                {

                    Console.WriteLine("{0}\t\t{1}\t\t{2}", reader.GetString(0),
                        reader.GetDateTime(1), reader.GetDateTime(2));
                }
                connection.Close();
            }
        }

        private static void Tsql_SelectGeoDeptChair(SqlConnection connection)
        {
            Console.WriteLine("=================================");


            using (var command = new SqlCommand(Tsql_GeoDeptChair(), connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("Who is the Geology Department Chair?");
                Console.WriteLine("-----------------------------");
                while (reader.Read())
                {

                    Console.WriteLine("{0}", reader.GetString(0));
                }
                connection.Close();
            }
        }

        //----------------------------------------------------------------------

        private static void Tsql_SelectDeclaredMajors(SqlConnection connection)
        {
            Console.WriteLine("=================================");

            using (var command = new SqlCommand(TSQL_ShowDeclaredMajors(), connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("Show who declared what major\nMajorCode\t\tStudentFirstName");
                Console.WriteLine("-----------------------------");
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}\t\t{1}",
                        reader.GetString(0),
                        reader.GetString(1));
                    }
                connection.Close();
            }
        }
        private static void Tsql_SelectEventAttendees(SqlConnection connection)
        {
            Console.WriteLine("=================================");

            string tsql = Tsql_EventAttendees();

            using (var command = new SqlCommand(Tsql_EventAttendees(), connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("Show who attended what event\nEventName\t\tStudentFirstName");
                Console.WriteLine("-----------------------------");
                while (reader.Read())
                    {
                        
                        Console.WriteLine("{0}\t\t{1}", reader.GetString(0),
                            reader.GetString(1));
                    }
                connection.Close();
            }
        }
        private static void Submit_Tsql_NonQuery(SqlConnection connection,
                                                    string tsqlPurpose,
                                                    string tsqlSourceCode,
                                                    string parameterName = null,
                                                    string parameterValue = null)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("{0}...", tsqlPurpose);

            using (var command = new SqlCommand(tsqlSourceCode, connection))
            {
                connection.Open();
                if (parameterName != null)
                {
                    command.Parameters.AddWithValue(  
                        parameterName,
                        parameterValue);
                }
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected + " = rows affected.");
                connection.Close();
            }

        }


        //-------------------------------------------------------------
        //
        // SQL Statements
        //
        //-------------------------------------------------------------

        //CREATING ALL TABLES---------------------------------------------------
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
                MajorName Varchar(50) Not Null
				
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
                CONSTRAINT StartDate CHECK (StartDate >= GetDate() ),
                CONSTRAINT EndDateDate CHECK (EndDate > GetDate() ),

                --Completing constraint where Startdate must be less than enddate
                CONSTRAINT StartDateBeforeEndDate CHECK (StartDate < EndDate)

            );

            CREATE TABLE EventAttendant
            (
                EventID VARCHAR(5) Not Null,
                StudentID VARCHAR(50) Not Null,

                CONSTRAINT PK_EventAttendant_EventID_AND_STUDENTID PRIMARY KEY NONCLUSTERED (EventID, StudentID) ,

                CONSTRAINT FK_EventAttendant_EventID FOREIGN KEY (EventID) REFERENCES Event (EventID) on UPDATE CASCADE ON DELETE CASCADE,
                CONSTRAINT FK_EventAttendant_StudentID FOREIGN KEY (StudentID) REFERENCES Student (StudentID) on UPDATE CASCADE ON DELETE CASCADE
            );

            CREATE TABLE DeclaredMajor
            (
                
                --(2/2)Completing Major code 3 character constraint
                MajorCode CHAR(3) Not Null,
                StudentID VARCHAR(50) Not Null,

                CONSTRAINT PK_DeclaredMajor_MajorCode PRIMARY KEY NONCLUSTERED (MajorCode, StudentID) ,

                CONSTRAINT FK_DeclaredMajor_StudentID FOREIGN KEY (StudentID) REFERENCES Student (StudentID) on UPDATE CASCADE ON DELETE CASCADE,
                CONSTRAINT  FK_DeclaredMajor_MajorCode FOREIGN Key (MajorCode) REFERENCES Major (MajorCode) on UPDATE CASCADE ON DELETE No ACTION
            );

            CREATE TABLE EventHost
            (
                EventID VARCHAR(5) Not Null,
                DeptCode VarChar(3) Not Null ,

                CONSTRAINT PK_EventHost_EventID_And_DeptCode PRIMARY KEY NONCLUSTERED (EventID, DeptCode) ,

                CONSTRAINT FK_EventHost_EventID FOREIGN KEY (EventID) REFERENCES Event (EventID) on UPDATE CASCADE ON DELETE CASCADE,
                CONSTRAINT FK_EventHost_DeptCode FOREIGN KEY (DeptCode) REFERENCES Department (DeptCode) on UPDATE CASCADE ON DELETE No ACTION
            );
            
            ";
        }

        //INSERTING INTO TABLES-------------------------------------------------
        //Writing initial data to do preliminary testing on create table statements
        private static string Build_Tsql_Inserts()
        {
            
            return @"
            INSERT INTO Department 
            VALUES 
            ('BIO', 'Biology Department', 'Pepe', 45),
            ('CHM', 'Chemistry Department', 'Jose', 5),
            ('GEO', 'Geology Department', 'John', 15),
            ('ENG', 'English Department', 'Tim', 35),
            ('FRL', 'Foreign Languages Department', 'Sosa', 15);
            
            INSERT INTO Major 
            VALUES 
            ('BIO', 'BIO', 'Biology'),
            ('CHM', 'CHM', 'Chemistry'),
            ('GEO', 'GEO', 'Geology'),
            ('ENG', 'ENG', 'English'),
            ('POR', 'FRL', 'Portuguese');
            
            INSERT INTO Student
            VALUES 
            ('12345','pepe','pepe','pp'),
            ('54321','jose','jose','jj'),
            ('23456','John','john','jj'),
            ('65432','tim','tim','tt'),
            ('98765','sosa','sosa','ss');
            
            INSERT INTO Event 
            VALUES 
            ('abc','Cookout',DATEADD(day, 1, GETDATE()), DATEADD(day, 7, GETDATE())),
            ('rew','Reading',DATEADD(day, 1, GETDATE()), DATEADD(day, 3, GETDATE())),
            ('wxx','Basketball Game',DATEADD(day, 1, GETDATE()), DATEADD(day, 3, GETDATE())),
            ('tec','Fun Day',DATEADD(day, 1, GETDATE()), DATEADD(day, 2, GETDATE())),
            ('nsm','Swimming',DATEADD(day, 1, GETDATE()), DATEADD(day, 5, GETDATE()));
            
            INSERT INTO EventAttendant
            VALUES 
            ('abc','12345'),
            ('rew','54321'),
            ('wxx','23456'),
            ('tec','65432'),
            ('nsm','98765');
            
            INSERT INTO DeclaredMajor 
            VALUES 
            ('BIO','12345'),
            ('CHM','54321'),
            ('ENG','23456'),
            ('POR','65432'),
            ('GEO','98765');
            
            INSERT INTO EventHost 
            VALUES 
            ('abc','BIO'),
            ('rew','CHM'),
            ('wxx','ENG'),
            ('tec','FRL'),
            ('nsm','GEO');
            
            ";
        }

        //5 SQL Queries against tables created----------------------------------
        
        private static string Tsql_EventInfo()
        {
            return @"select eventname, startdate, enddate
                    from event;";
        }
        private static string Tsql_GeoDeptChair()
        {
            return @"select deptchair from department where deptcode = 'GEO'";
        }
        private static string Tsql_CookoutHost()
        {
            return @"select d.deptname
                    from department d
                    join eventhost e
                    on d.deptcode = e.deptcode
                    join event j
                    on e.eventid = j.eventid
                    where eventname = 'cookout';";
        }
        private static string Tsql_EventAttendees()
        {
            return @"Select e.EventName,
                            s.StudentFirstName,
                            s.StudentLastName
                     from EventAttendant j
                            Join Event e
                     on j.eventid = e.eventid
                            Join Student s
                            on s.studentid = j.studentid;";
        }
        private static string TSQL_ShowDeclaredMajors()
        {
            return @"select d.majorCode, s.studentFirstName
                    from DeclaredMajor d
                    Join student s
                    on s.studentid = d.studentID;";
        }

        //-------------------------------------------------------------
        //
        // End of SQL Statements
        //
        //-------------------------------------------------------------
    }
}
