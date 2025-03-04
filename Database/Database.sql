-- CREATE DATABASE OnlineLibrary;
-- USE OnlineLibrary;
-- GO
CREATE TABLE Books (
    BookID INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255) NOT NULL,
    ISBN NVARCHAR(13) NOT NULL,
    PublishedYear INT,
    ShortDescription NVARCHAR(255)
);
GO
CREATE TABLE Authors (
    AuthorID INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL
);
GO
CREATE TABLE BookAuthors (
    BookID INT,
    AuthorID INT,
    PRIMARY KEY (BookID, AuthorID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID),
    FOREIGN KEY (AuthorID) REFERENCES Authors(AuthorID)
);
GO
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(50) NOT NULL
);
GO
CREATE TABLE Members (
    MemberID INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(100) NOT NULL,
    Lozinka NVARCHAR(100) NOT NULL,
    JoinDate DATE NOT NULL,
	RoleID INT,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);
GO
CREATE TABLE Loans (
    LoanID INT PRIMARY KEY IDENTITY,
    BookID INT,
    MemberID INT,
    LoanDate DATE NOT NULL,
    ReturnDate DATE,
    FOREIGN KEY (BookID) REFERENCES Books(BookID),
    FOREIGN KEY (MemberID) REFERENCES Members(MemberID)
);
GO
CREATE TABLE Logs (
    LogID INT PRIMARY KEY IDENTITY,
    CreatedTime DATETIME NOT NULL,
	LogLevel INT,
	LogMessage VARCHAR(255) NOT NULL
);
GO
INSERT INTO Roles (RoleName) VALUES ('Admin');
GO
INSERT INTO Roles (RoleName) VALUES ('Member');