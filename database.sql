-- Create a new database named FakeBank
CREATE DATABASE FakeBank;

-- Use the newly created database
USE [FakeBank];

-- Create a table named Addresses with the following columns:
-- Id: A primary key that auto increments
-- Street: A string that represents the street name
-- City: A string that represents the city name
-- PostCode: A string that represents the postal code
-- Country: A string that represents the country name
create table Addresses
(
    Id       int identity(1,1) primary key,
    Street   varchar(50) not null,
    City     varchar(45) not null,
    PostCode varchar(45) not null,
    Country  varchar(45) not null
);

-- Create a table named Contacts with the following columns:
-- Id: A primary key that auto increments
-- Email: A string that represents the email address
-- PhoneNumber: A string that represents the phone number
create table Contacts
(
    Id          int identity(1,1) primary key,
    Email       varchar(100) null,
    PhoneNumber varchar(50)  null
);

-- Create a table named Users with the following columns:
-- Id: A primary key that auto increments
-- Username: A string that represents the username
-- PasswordHash: A string that represents the hashed password
-- CreatedAt: A datetime that represents when the user was created
-- ContactId: A foreign key that references the Contacts table
-- AddressId: A foreign key that references the Addresses table
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

-- Create a table named BankAccount with the following columns:
-- Id: A primary key that auto increments
-- AccountNumber: A string that represents the account number
-- Balance: A decimal that represents the account balance
-- UserId: A foreign key that references the Users table
create table BankAccount
(
    Id            int identity(1,1) primary key,
    AccountNumber varchar(50)       not null, -- gonna be generated using c# guids
    Balance       decimal default 0 not null,
    UserId        int               not null,
    constraint FK_UserId
        foreign key (UserId) references Users (Id)
);

-- Create an index on the UserId column of the BankAccount table
create index UserId_idx
    on BankAccount (UserId);

-- Create a table named Transactions with the following columns:
-- Id: A primary key that auto increments
-- SenderId: A foreign key that references the BankAccount table
-- ReceiverId: A foreign key that references the BankAccount table
-- Amount: A decimal that represents the transaction amount
-- SentAt: A datetime that represents when the transaction was sent
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

-- Create a table named UserRoles with the following columns:
-- Id: A primary key that auto increments
-- UserId: A foreign key that references the Users table
-- RoleName: A string that represents the role name
CREATE TABLE UserRoles
(
    Id       int identity(1,1) primary key,

    UserId   int              not null,
    RoleName varchar(50)      not null

        FOREIGN KEY (UserId) REFERENCES Users (Id)
);

-- Create an index on the UserId column of the UserRoles table
create index UserId2_idx
    on UserRoles (UserId);

-- Create an index on the ReceiverId column of the Transactions table
create index ReceiverId_idx
    on Transactions (ReceiverId);

-- Create an index on the SenderId column of the Transactions table
create index SenderId_idx
    on Transactions (SenderId);

-- Create an index on the AddressId column of the Users table
create index AddressId_idx
    on Users (AddressId);

-- Create an index on the ContactId column of the Users table
create index ContactId_idx
    on Users (ContactId);