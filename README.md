# Redwood-University

## Our Case Study

Database implementation for Redwood University 

Redwood University a university has requested the design and implementation of a database to store its data. The university encompasses multiple departments, each of which has a chair. The university does not want to store particular information regarding the chair, rather information pertaining to the department name and chair name, as well as the number of faculty members the department has. Department names must always start with Department. 

The university has numerous students and each of them has declared at least one major. Additionally, the name and initials of a student are stored. Initials must be more than one character long. For each major, the university wants to store the major name, the department it is associated with, and a code. For example, ‘Biology’ is associated with department 3 (i.e., the Department of Biology) and has the code ‘BIO’. Major codes must be three characters. Majors can be declared by one or more students. A major references one department, however a department offers one or more majors. 

Each department has the possibility of hosting events, and an event can be (collaboratively) hosted by one or more departments. In addition to the event name, the university would like to store the start and end dates of the event. As it is logical, an event cannot end before the start date. Information pertaining to events are stored ahead of time, therefore at the time of insertion an event cannot be a past date or the current date. Students must attend one or more events, and each event will comprise one or more students.

---

# Conceptual Model

### Main Identity Types
<ul>
  <li>Department</li>
<li>Major</li>
<li>Student</li>
<li> Event</li>
  </ul>
  
### Main Relationship Types Between the Entity Types & Multiplicity Constraints for Each Relationship Identified 

| Entity Name |	Multiplicity |	Relationship |	Multiplicity | 	Entity Name |
|------|------|------|------|------|
| Department |	1..1 |	Has |	1..* | Faculty |
| Student |	0..* |	Declares |	1..* |	Major |
| Major |	1..*	| Belongs |	1..1 |	Department |
| Department |	1..* |	Host |	0..* |	Event |
| Student |	1..* |	Attends |	1..* |	Event |
•	assuming all departments have at least one major 

### Attributes Associated with Antity or Relationship Types 
Department Attribute (DeptCode, DeptName, DeptChair, DeptMembers)
Major Attribute (MajorCode, MajorName)
Student Attribute (StudentID, StudentName, StudentInitials) 
Event Attribute (EventID, StartDate, EndDate) 

### Candidate & Primary Key Attributes for Each (strong) Entity Type  

•	Department (DeptCode, DeptName, DeptChair, DeptMembers) DeptCode, DeptName and DeptCode will be our candidate keys and DeptCode will be our  primary key because of its ability to more accurately identify the tuple when compared to DeptChair, while also being a more succinct way to query our table when compared to  DeptName. 

---

•	Major (MajorCode, DeptCode, MajorName) MajorCode will be our candidate and primary key to avoid future issues of having two majors with the same name in different departments. 

---

•	Student (StudentID, StudentName, StudentInitials) StudentID is our candidate and primary key since it is the only attribute able to uniquely identify this entity.

---

•	Event (EventID, EventName, StartDate, EndDate) EventID is our candidate and primary key since it is the only attribute able to uniquely identify this entity.

---

## Conceptual ER Diagram
<img width="1016" alt="Screen Shot 2021-12-10 at 8 23 22 AM" src="https://user-images.githubusercontent.com/46760723/145580755-7bc2dd53-4443-4cb8-9cc0-0185c3d66dda.png">


# Logical Model

•	Department (DeptCode, DeptName, DeptChair, DeptMembers)
Primary Key DeptCode
Alternate Key DeptName

---

•	Major (MajorCode, DeptCode, MajorName)
Primary Key MajorCode
Foreign Key DeptCode References Department(DeptCode)
ON UPDATE CASCADE

---

•	Student (StudentID, StudentName, StudentInitials) 
Primary Key StudentID
Derived StudentInitials From StudentName

---

•	Event (EventID, EventName, StartDate, EndDate) 
Primary Key EventID

---

•	EventAttendant (EventID, StudentID) 
Primary Key EventID, StudentID
Foreign Key EventID References Event(EventID)
		   StudentID References Student(StudentID)
ON UPDATE CASCADE

---

•	DeclaredMajor(StudentID, MajorCode) 
Primary Key StudentID, MajorCode
Foreign Key MajorCode References Major(MajorCode)
		   StudentID References Student(StudentID)
ON UPDATE CASCADE 

---

•	EventHost (EventID, DeptCode)
Primary Key EventID, DeptCode
Foreign Key EventID References Event(EventID)
		   DeptCode References Department(DeptCode)
ON UPDATE CASCADE 

---

The model is in 3rd normal form because it has no repeating groups, the partial dependencies are removed, and the transitive dependencies are removed. The exception is the student table where we have chosen to leave it in 2nd normal form to reduce complexity when querying. 


<ol> 
  
  
### <li> Primary key constraints </li>
Department Primary Key DeptCode
Faculty Primary Key FacultyId

Major Primary Key MajorCode

Student Primary Key StudentID

Event Primary Key EventID

EventAttendant Primary Key EventID, StudentID

DeclaredMajor Primary Key StudentID, MajorCode

EventHost Primary Key EventID, DeptCode

  ### <li>Referential integrity/Foreign key constraints </li>
Department Foreign Key DeptChair References Faculty(FacultyID)

Faculty Foreign Key DeptCode References Department(DeptCode) 
Nonnullable. A faculty member must belong to a department.

Major Foreign Key Key DeptCode References Department(DeptCode)
Nonnullable

EventAttendant Foreign Key EventID References Event(EventID)
		   StudentID References Student(StudentID)

DeclaredMajor Foreign Key MajorCode References Major(MajorCode)
		   StudentID References Student(StudentID)

EventHost Foreign Key EventID References Event(EventID)
		   DeptCode References Department(DeptCode)

### <li>Alternate key constraints  </li>

Department Alternate Key DeptName Nonnullable

  ### <li>General constraints </li>

In the Event table, start date must be before end date.

In the Event table, start date must be before end date.

- Major Codes must be three characters
- Student Initials must be more than 1 character
- Department names must include "Department"
- Events must start and end in the future, not today or the past
</ol>

### Logical Model ER Diagram

![image](https://user-images.githubusercontent.com/46760723/145521780-d8573219-85c1-4225-ae7f-63678ce80318.png)
