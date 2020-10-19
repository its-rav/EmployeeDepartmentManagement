create database EDM
use EDM

--drop database EmployeeDepartmentManagement

create table UserRole (
	Id int primary key,
	RoleName varchar(50) not null,
	IsDeleted bit not null default 0,
	CreatedBy varchar(50) not null,
	CreatedAt datetime not null default getdate(),
	UpdatedBy varchar(50) not null,
	UpdatedAt datetime not null default getdate()
)
INSERT INTO UserRole(Id,RoleName,CreatedBy,UpdatedBy) VALUES
(1,'Adminstrator','NhanVT','NhanVT'),
(2,'Department Staff','NhanVT','NhanVT'),
(3,'Moderator','NhanVT','NhanVT')

create table Account (
	Id uniqueidentifier primary key,
	Username varchar(100) unique not null,
	PasswordHash varbinary(max) null,
	Email varchar(450) not null,
	FullName nvarchar(100) not null,
	PhoneNumber varchar(11),
	Address nvarchar(450),
	Photo varchar(max),
	RoleId int not null,
	FOREIGN KEY(RoleId) REFERENCES UserRole(Id),
	IsDeleted bit not null default 0,
	CreatedBy varchar(50) not null,
	CreatedAt datetime not null default getdate(),
	UpdatedBy varchar(50) not null,
	UpdatedAt datetime not null default getdate()
)

INSERT INTO Account(Id,Username,Email,FullName,PhoneNumber,Address,RoleId,CreatedBy,UpdatedBy) VALUES
('9b6980df-bad6-460a-aaea-64591b3ae7ae','nhanvt','NhanVTSE130478@fpt.edu.vn','Vo Thanh Nhan','0906690322','Cay Tram',1,'NhanVT','NhanVT'),
('5eb93535-b957-461a-a589-3b9e1c0c6bbe','nhanvt2','voasd123@gmail.com','Vo Thanh Nhan2','0906690322','Nguyen Van Khoi',2,'NhanVT','NhanVT')

create table Department(
	Id varchar(10) primary key,
	DepartmentName nvarchar(50) not null,
	Hotline varchar(11),
	RoomNumber varchar(6) not null unique,
	IsDeleted bit not null default 0,
	CreatedBy varchar(50) not null,
	CreatedAt datetime not null default getdate(),
	UpdatedBy varchar(50) not null,
	UpdatedAt datetime not null default getdate()
)
INSERT INTO Department(Id,DepartmentName,Hotline,RoomNumber,CreatedBy,UpdatedBy) VALUES
('AD','Accounting','0906690322','102','NhanVT','NhanVT'),
('BD','Biologistic','0906690322','103','NhanVT','NhanVT')

create table DepartmentStaff(
	AccountId uniqueidentifier not null,
	DepartmentId varchar(10) not null,
	PRIMARY KEY(AccountId,DepartmentId),
	FOREIGN KEY(AccountId) REFERENCES Account(Id),
	FOREIGN KEY(DepartmentId) REFERENCES Department(Id),
	IsDeleted bit not null default 0,
	CreatedBy varchar(50) not null,
	CreatedAt datetime not null default getdate(),
	UpdatedBy varchar(50) not null,
	UpdatedAt datetime not null default getdate()
)
INSERT INTO DepartmentStaff(AccountId,DepartmentId,CreatedBy,UpdatedBy) VALUES
('9b6980df-bad6-460a-aaea-64591b3ae7ae','AD','NhanVT','NhanVT'),
('9b6980df-bad6-460a-aaea-64591b3ae7ae','BD','NhanVT','NhanVT'),
('5eb93535-b957-461a-a589-3b9e1c0c6bbe','BD','NhanVT','NhanVT')