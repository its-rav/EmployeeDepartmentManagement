create database EmployeeDepartmentManagement
use EmployeeDepartmentManagement

--drop database EmployeeDepartmentManagement

create table Role (
	RoleId varchar(10) primary key,
	RoleNm varchar(50) not null,
	DelFlg bit not null default 0,
	InsBy varchar(50) not null,
	InsDatetime datetime not null default getdate(),
	UpdBy varchar(50) not null,
	UpdDatetime datetime not null default getdate()
)

create table User (
	Id int identity(1,1) primary key,
	Username varchar(100) unique not null,
	PasswordHash varbinary(max) not null,
	PasswordSalt varbinary(max) not null,
	Email varchar(450) not null,
	UserNo varchar(11) not null unique,
	FullName nvarchar(100) not null,
	Phonenumber varchar(11),
	Address nvarchar(450),
	Photo varchar(max),
	RoleId varchar(10) not null,
	DelFlg bit not null default 0,
	InsBy varchar(50) not null,
	InsDatetime datetime not null default getdate(),
	UpdBy varchar(50) not null,
	UpdDatetime datetime not null default getdate()
)
create table Department(
	DepartmentId varchar(10) primary key,
	DepartmentNm nvarchar(50) not null,
	Hotline varchar(11),
	RoomNum varchar(6) not null unique,
	DelFlg bit not null default 0,
	InsBy varchar(50) not null,
	InsDatetime datetime not null default getdate(),
	UpdBy varchar(50) not null,
	UpdDatetime datetime not null default getdate()
)

create table Staff(
	StaffId int primary key,
	DepartmentId varchar(10) not null,
	DelFlg bit not null default 0,
	InsBy varchar(50) not null,
	InsDatetime datetime not null default getdate(),
	UpdBy varchar(50) not null,
	UpdDatetime datetime not null default getdate()
)
alter table Staff
add constraint FK_Staff_User foreign key (StaffId) references User(Id)
alter table Staff
add constraint FK_Staff_Department foreign key (DepartmentId) references Department(departmentId)

alter table Users
add constraint FK_User_Role foreign key (RoleId) references Role(RoleId)