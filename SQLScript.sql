CREATE TABLE Positions
(
    PositionId INT IDENTITY(1,1) PRIMARY KEY,
    PositionName NVARCHAR(100) NOT NULL,
    Salary DECIMAL(10,2) NOT NULL
);

INSERT INTO Positions (PositionName, Salary)
VALUES
(N'Програміст', 30000),
(N'Тестувальник', 25000),
(N'Project Manager', 40000),
(N'Системний адміністратор', 28000);

CREATE TABLE Employees
(
    EmployeeId INT IDENTITY(1,1) PRIMARY KEY,

    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50) NULL,

    PositionId INT NOT NULL,

    Address NVARCHAR(200) NOT NULL,
    Characteristic NVARCHAR(500) NULL,

    CONSTRAINT FK_Employees_Positions
        FOREIGN KEY (PositionId)
        REFERENCES Positions(PositionId)
);

create index IX_Employees_PositionId on Employees(PositionId);

INSERT INTO Employees
(
    LastName,
    FirstName,
    MiddleName,
    PositionId,
    Address,
    Characteristic
)
VALUES
(N'Іваненко', N'Іван', N'Іванович', 1,
 N'м. Чернівці, вул. Головна, 10',
 N'Відповідальний та комунікабельний'),

(N'Петренко', N'Петро', N'Олександрович', 1,
 N'м. Чернівці, вул. Героїв Майдану, 20',
 N'Досвідчений програміст'),

(N'Шевченко', N'Олена', N'Ігорівна', 2,
 N'м. Чернівці, вул. Руська, 15',
 N'Уважна до деталей'),

(N'Коваленко', N'Андрій', N'Васильович', 3,
 N'м. Львів, вул. Франка, 12',
 N'Організований керівник'),

(N'Мельник', N'Марія', N'Олександрівна', 1,
 N'м. Чернівці, вул. Комарова, 5',
 N'Креативний розробник'),

(N'Бондаренко', N'Олег', N'Миколайович', 4,
 N'м. Чернівці, вул. Шевченка, 8',
 N'Відповідальний системний адміністратор'),

(N'Ткаченко', N'Наталія', N'Петрівна', 2,
 N'м. Чернівці, вул. Сторожинецька, 25',
 N'Уважний тестувальник'),

(N'Романюк', N'Дмитро', N'Вікторович', 1,
 N'м. Чернівці, вул. Південно-Кільцева, 30',
 N'Аналітичне мислення'),

(N'Гнатюк', N'Сергій', N'Іванович', 3,
 N'м. Київ, вул. Хрещатик, 50',
 N'Досвідчений менеджер'),

(N'Левченко', N'Анна', N'Романівна', 1,
 N'м. Чернівці, вул. Кобилянської, 7',
 N'Наполегливий розробник');

 CREATE TABLE Projects
(
    ProjectId INT IDENTITY(1,1) PRIMARY KEY,
    ProjectName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL
);

INSERT INTO Projects (ProjectName, Description)
VALUES
(
    N'CRM System',
    N'Розробка системи управління взаємовідносинами з клієнтами'
),
(
    N'Mobile Banking',
    N'Розробка мобільного банківського застосунку'
);

 CREATE TABLE EmployeeProjects
(
    EmployeeProjectId INT IDENTITY(1,1) PRIMARY KEY,

    EmployeeId INT NOT NULL,
    ProjectId INT NOT NULL,

    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,

    CONSTRAINT FK_EmployeeProjects_Employees
        FOREIGN KEY (EmployeeId)
        REFERENCES Employees(EmployeeId),

    CONSTRAINT FK_EmployeeProjects_Projects
        FOREIGN KEY (ProjectId)
        REFERENCES Projects(ProjectId),

    CONSTRAINT CK_EmployeeProjects_Dates
        CHECK (EndDate >= StartDate),

    CONSTRAINT UQ_EmployeeProjects
        UNIQUE (EmployeeId, ProjectId)
);

create index IX_EmployeeProjects_EmployeeId on EmployeeProjects(EmployeeId);
create index IX_EmployeeProjects_ProjectId on EmployeeProjects(ProjectId);

INSERT INTO EmployeeProjects
(EmployeeId, ProjectId, StartDate, EndDate)
VALUES

-- Працівник 1
(1, 1, '2026-01-01', '2026-08-31'),
(1, 2, '2026-04-01', '2026-12-31'),

-- Працівник 2
(2, 1, '2026-01-01', '2026-07-31'),
(2, 2, '2026-03-01', '2026-09-30'),

-- Працівник 3
(3, 1, '2026-02-01', '2026-08-31'),
(3, 2, '2026-05-01', '2026-11-30'),

-- Працівник 4
(4, 1, '2026-01-15', '2026-06-30'),
(4, 2, '2026-04-01', '2026-10-31'),

-- Працівник 5
(5, 1, '2026-01-01', '2026-09-30'),
(5, 2, '2026-06-01', '2026-12-31'),

-- Працівник 6
(6, 1, '2026-02-01', '2026-07-31'),
(6, 2, '2026-05-01', '2026-08-31'),

-- інші працівники
(7, 1, '2026-01-01', '2026-05-31'),
(8, 2, '2026-03-01', '2026-09-30'),
(9, 1, '2026-02-01', '2026-06-30'),
(10, 2, '2026-04-01', '2026-10-31');