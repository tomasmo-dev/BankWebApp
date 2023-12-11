CREATE DATABASE FakeBank;

USE [FakeBank];

create table Addresses
(
    Id       int identity(1,1) primary key,
    Street   varchar(50) not null,
    City     varchar(45) not null,
    PostCode varchar(45) not null,
    Country  varchar(45) not null
);

create table Contacts
(
    Id          int identity(1,1) primary key,
    Email       varchar(100) null,
    PhoneNumber varchar(50)  null
);

create table Users
(
    Id           int identity(1,1) primary key,
    Username     varchar(50)  not null,
    PasswordHash varchar(max) not null,
    CreatedAt    datetime     not null,
    ContactId    int          not null,
    AddressId    int          not null,
    constraint FK_AddressId
        foreign key (AddressId) references Addresses (Id),
    constraint FK_ContactId
        foreign key (ContactId) references Contacts (Id)
);

create table BankAccount
(
    Id            int identity(1,1) primary key,
    AccountNumber varchar(50)       not null, -- gonna be generated using c# guids
    Balance       decimal default 0 not null,
    UserId        int               not null,
    constraint FK_UserId
        foreign key (UserId) references Users (Id)
);

create index UserId_idx
    on BankAccount (UserId);

create table Transactions
(
    Id         int identity(1,1) primary key,
    SenderId   int      not null,
    ReceiverId int      not null,
    Amount     decimal  not null,
    SentAt     datetime not null,
    constraint FK_ReceiverId
        foreign key (ReceiverId) references BankAccount (Id),
    constraint FK_SenderId
        foreign key (SenderId) references BankAccount (Id)
);

CREATE TABLE UserRoles
(
    Id       int identity(1,1) primary key,
    
    UserId   int              not null,
    RoleName varchar(50)      not null

    FOREIGN KEY (UserId) REFERENCES Users (Id)
);

create index UserId2_idx
    on UserRoles (UserId);

create index ReceiverId_idx
    on Transactions (ReceiverId);

create index SenderId_idx
    on Transactions (SenderId);

create index AddressId_idx
    on Users (AddressId);

create index ContactId_idx
    on Users (ContactId);