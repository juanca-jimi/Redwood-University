# Redwood-University
Database implementation for Redwood University 

Redwood University a university has requested the design and implementation of a database to store its data. The university encompasses multiple departments, each of which has a chair. The university does not want to store particular information regarding the chair, rather information pertaining to the department name and chair name, as well as the number of faculty members the department has. Department names must always start with Department. 

The university has numerous students and each of them has declared at least one major. Additionally, the name and initials of a student are stored. Initials must be more than one character long. For each major, the university wants to store the major name, the department it is associated with, and a code. For example, ‘Biology’ is associated with department 3 (i.e., the Department of Biology) and has the code ‘BIO’. Major codes must be three characters. Majors can be declared by one or more students. A major references one department, however a department offers one or more majors. 

Each department has the possibility of hosting events, and an event can be (collaboratively) hosted by one or more departments. In addition to the event name, the university would like to store the start and end dates of the event. As it is logical, an event cannot end before the start date. Information pertaining to events are stored ahead of time, therefore at the time of insertion an event cannot be a past date or the current date. Students must attend one or more events, and each event will comprise one or more students.


