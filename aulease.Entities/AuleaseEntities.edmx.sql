
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 09/26/2013 08:12:09
-- Generated from EDMX file: \\psf\Home\Documents\Visual Studio 2012\Projects\AULease-MVC-development\aulease.Entities\AuleaseEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [auleaseApps];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ChargeType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Charges] DROP CONSTRAINT [FK_ChargeType];
GO
IF OBJECT_ID(N'[dbo].[FK_ChargeVendorRate]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Charges] DROP CONSTRAINT [FK_ChargeVendorRate];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentLease]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Leases] DROP CONSTRAINT [FK_ComponentLease];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentMake]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_ComponentMake];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentRepair]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Repairs] DROP CONSTRAINT [FK_ComponentRepair];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentSignature]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_ComponentSignature];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_ComponentStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_ComponentType];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentLease]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Leases] DROP CONSTRAINT [FK_DepartmentLease];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentSingleCharge]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SingleCharges] DROP CONSTRAINT [FK_DepartmentSingleCharge];
GO
IF OBJECT_ID(N'[dbo].[FK_LeaseOverhead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Leases] DROP CONSTRAINT [FK_LeaseOverhead];
GO
IF OBJECT_ID(N'[dbo].[FK_UserOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Orders] DROP CONSTRAINT [FK_UserOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_StatusRepair]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Repairs] DROP CONSTRAINT [FK_StatusRepair];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentProperty_Components]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComponentProperty] DROP CONSTRAINT [FK_ComponentProperty_Components];
GO
IF OBJECT_ID(N'[dbo].[FK_ComponentProperty_Properties]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ComponentProperty] DROP CONSTRAINT [FK_ComponentProperty_Properties];
GO
IF OBJECT_ID(N'[dbo].[FK_LeaseCharge_Charges]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LeaseCharge] DROP CONSTRAINT [FK_LeaseCharge_Charges];
GO
IF OBJECT_ID(N'[dbo].[FK_LeaseCharge_Leases]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LeaseCharge] DROP CONSTRAINT [FK_LeaseCharge_Leases];
GO
IF OBJECT_ID(N'[dbo].[FK_StandardComponentProperty_StandardComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardComponentProperty] DROP CONSTRAINT [FK_StandardComponentProperty_StandardComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_StandardComponentProperty_Property]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardComponentProperty] DROP CONSTRAINT [FK_StandardComponentProperty_Property];
GO
IF OBJECT_ID(N'[dbo].[FK_ModelComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_ModelComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_ModelStandardComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardComponents] DROP CONSTRAINT [FK_ModelStandardComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_StandardComponentType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardComponents] DROP CONSTRAINT [FK_StandardComponentType];
GO
IF OBJECT_ID(N'[dbo].[FK_MakeStandardComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardComponents] DROP CONSTRAINT [FK_MakeStandardComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_VendorRateType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[VendorRates] DROP CONSTRAINT [FK_VendorRateType];
GO
IF OBJECT_ID(N'[dbo].[FK_TaxCharge]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Charges] DROP CONSTRAINT [FK_TaxCharge];
GO
IF OBJECT_ID(N'[dbo].[FK_SystemGroupLease]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Leases] DROP CONSTRAINT [FK_SystemGroupLease];
GO
IF OBJECT_ID(N'[dbo].[FK_SystemGroupUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SystemGroups] DROP CONSTRAINT [FK_SystemGroupUser];
GO
IF OBJECT_ID(N'[dbo].[FK_SystemGroupOrder]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SystemGroups] DROP CONSTRAINT [FK_SystemGroupOrder];
GO
IF OBJECT_ID(N'[dbo].[FK_MakeModel]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Models] DROP CONSTRAINT [FK_MakeModel];
GO
IF OBJECT_ID(N'[dbo].[FK_SystemGroupComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Components] DROP CONSTRAINT [FK_SystemGroupComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_POSystemGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SystemGroups] DROP CONSTRAINT [FK_POSystemGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_StandardOptionStandardComponentStandardComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardOptionStandardComponents] DROP CONSTRAINT [FK_StandardOptionStandardComponentStandardComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_TypeOverhead]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Overheads] DROP CONSTRAINT [FK_TypeOverhead];
GO
IF OBJECT_ID(N'[dbo].[FK_StandardOptionStandardOptionStandardComponent]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StandardOptionStandardComponents] DROP CONSTRAINT [FK_StandardOptionStandardOptionStandardComponent];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskStatus]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT [FK_TaskStatus];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentCoordinator_Department]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentCoordinator] DROP CONSTRAINT [FK_DepartmentCoordinator_Department];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentCoordinator_Coordinator]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentCoordinator] DROP CONSTRAINT [FK_DepartmentCoordinator_Coordinator];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Coordinators]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Coordinators];
GO
IF OBJECT_ID(N'[dbo].[Charges]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Charges];
GO
IF OBJECT_ID(N'[dbo].[Components]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Components];
GO
IF OBJECT_ID(N'[dbo].[Departments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Departments];
GO
IF OBJECT_ID(N'[dbo].[Leases]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Leases];
GO
IF OBJECT_ID(N'[dbo].[Makes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Makes];
GO
IF OBJECT_ID(N'[dbo].[Orders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Orders];
GO
IF OBJECT_ID(N'[dbo].[Overheads]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Overheads];
GO
IF OBJECT_ID(N'[dbo].[POes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[POes];
GO
IF OBJECT_ID(N'[dbo].[Properties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Properties];
GO
IF OBJECT_ID(N'[dbo].[Repairs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Repairs];
GO
IF OBJECT_ID(N'[dbo].[Signatures]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Signatures];
GO
IF OBJECT_ID(N'[dbo].[SingleCharges]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SingleCharges];
GO
IF OBJECT_ID(N'[dbo].[Status]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Status];
GO
IF OBJECT_ID(N'[dbo].[Types]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Types];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[VendorEmails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VendorEmails];
GO
IF OBJECT_ID(N'[dbo].[VendorRates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[VendorRates];
GO
IF OBJECT_ID(N'[dbo].[StandardComponents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StandardComponents];
GO
IF OBJECT_ID(N'[dbo].[Taxes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Taxes];
GO
IF OBJECT_ID(N'[dbo].[Models]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Models];
GO
IF OBJECT_ID(N'[dbo].[SystemGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SystemGroups];
GO
IF OBJECT_ID(N'[dbo].[StandardOptionStandardComponents]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StandardOptionStandardComponents];
GO
IF OBJECT_ID(N'[dbo].[StandardOptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StandardOptions];
GO
IF OBJECT_ID(N'[dbo].[Tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tasks];
GO
IF OBJECT_ID(N'[dbo].[Statements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Statements];
GO
IF OBJECT_ID(N'[dbo].[ComponentProperty]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ComponentProperty];
GO
IF OBJECT_ID(N'[dbo].[LeaseCharge]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LeaseCharge];
GO
IF OBJECT_ID(N'[dbo].[StandardComponentProperty]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StandardComponentProperty];
GO
IF OBJECT_ID(N'[dbo].[DepartmentCoordinator]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DepartmentCoordinator];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Coordinators'
CREATE TABLE [dbo].[Coordinators] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GID] nvarchar(max)  NOT NULL,
    [BillingAccess] bit  NOT NULL
);
GO

-- Creating table 'Charges'
CREATE TABLE [dbo].[Charges] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] decimal(9,2)  NOT NULL,
    [VendorRateId] int  NULL,
    [TypeId] int  NOT NULL,
    [TaxId] int  NULL
);
GO

-- Creating table 'Components'
CREATE TABLE [dbo].[Components] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SerialNumber] nvarchar(max)  NULL,
    [LeaseTag] nvarchar(max)  NULL,
    [OrderNumber] nvarchar(max)  NULL,
    [MAC] nvarchar(max)  NULL,
    [InstallSoftware] bit  NOT NULL,
    [InstallHardware] bit  NOT NULL,
    [Note] nvarchar(max)  NULL,
    [Renewal] bit  NOT NULL,
    [MakeId] int  NULL,
    [SignatureId] int  NULL,
    [TypeId] int  NULL,
    [StatusId] int  NOT NULL,
    [ModelId] int  NULL,
    [SystemGroupId] int  NULL,
    [Damages] nvarchar(max)  NULL,
    [ReturnDate] datetime  NULL
);
GO

-- Creating table 'Departments'
CREATE TABLE [dbo].[Departments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Fund] nvarchar(max)  NOT NULL,
    [Org] nvarchar(max)  NOT NULL,
    [Program] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Leases'
CREATE TABLE [dbo].[Leases] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BeginDate] datetime  NULL,
    [EndDate] datetime  NULL,
    [StatementName] nvarchar(max)  NULL,
    [Timestamp] datetime  NOT NULL,
    [ContractNumber] nvarchar(max)  NULL,
    [DepartmentId] int  NOT NULL,
    [MonthlyCharge] decimal(9,2)  NULL,
    [OverheadId] int  NULL,
    [ComponentId] int  NOT NULL,
    [SystemGroupId] int  NOT NULL
);
GO

-- Creating table 'Makes'
CREATE TABLE [dbo].[Makes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Orders'
CREATE TABLE [dbo].[Orders] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Note] nvarchar(max)  NULL,
    [UserId] int  NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'Overheads'
CREATE TABLE [dbo].[Overheads] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RateLevel] nvarchar(max)  NOT NULL,
    [Rate] decimal(9,2)  NOT NULL,
    [Term] int  NOT NULL,
    [BeginDate] datetime  NOT NULL,
    [TypeId] int  NOT NULL
);
GO

-- Creating table 'POes'
CREATE TABLE [dbo].[POes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PONumber] nvarchar(max)  NOT NULL,
    [Note] nvarchar(max)  NULL
);
GO

-- Creating table 'Properties'
CREATE TABLE [dbo].[Properties] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Repairs'
CREATE TABLE [dbo].[Repairs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Note] nvarchar(max)  NOT NULL,
    [Assignee] nvarchar(max)  NULL,
    [Timestamp] datetime  NOT NULL,
    [StatusId] int  NOT NULL,
    [ComponentId] int  NOT NULL
);
GO

-- Creating table 'Signatures'
CREATE TABLE [dbo].[Signatures] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MIME] nvarchar(max)  NOT NULL,
    [File] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SingleCharges'
CREATE TABLE [dbo].[SingleCharges] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] decimal(9,2)  NOT NULL,
    [GID] nvarchar(max)  NOT NULL,
    [Note] nvarchar(max)  NOT NULL,
    [Date] datetime  NOT NULL,
    [HasPaid] nvarchar(max)  NOT NULL,
    [DepartmentId] int  NOT NULL
);
GO

-- Creating table 'Status'
CREATE TABLE [dbo].[Status] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Types'
CREATE TABLE [dbo].[Types] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GID] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NULL,
    [Location_Building] nvarchar(max)  NULL,
    [Location_Room] nvarchar(max)  NULL
);
GO

-- Creating table 'VendorEmails'
CREATE TABLE [dbo].[VendorEmails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmailAddress] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'VendorRates'
CREATE TABLE [dbo].[VendorRates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Term] int  NOT NULL,
    [BeginDate] datetime  NOT NULL,
    [TypeId] int  NOT NULL,
    [Rate] decimal(9,2)  NOT NULL
);
GO

-- Creating table 'StandardComponents'
CREATE TABLE [dbo].[StandardComponents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModelId] int  NOT NULL,
    [TypeId] int  NOT NULL,
    [MakeId] int  NOT NULL
);
GO

-- Creating table 'Taxes'
CREATE TABLE [dbo].[Taxes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Price] decimal(18,4)  NOT NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- Creating table 'Models'
CREATE TABLE [dbo].[Models] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [MakeId] int  NOT NULL
);
GO

-- Creating table 'SystemGroups'
CREATE TABLE [dbo].[SystemGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [OrderId] int  NOT NULL,
    [Location_Building] nvarchar(max)  NULL,
    [Location_Room] nvarchar(max)  NULL,
    [POId] int  NULL
);
GO

-- Creating table 'StandardOptionStandardComponents'
CREATE TABLE [dbo].[StandardOptionStandardComponents] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StandardOptionId] int  NOT NULL,
    [StandardComponentId] int  NOT NULL
);
GO

-- Creating table 'StandardOptions'
CREATE TABLE [dbo].[StandardOptions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [BaseRate24] decimal(9,2)  NOT NULL,
    [BaseRate36] decimal(9,2)  NOT NULL,
    [SupportRate24] decimal(9,2)  NOT NULL,
    [SupportRate36] decimal(9,2)  NOT NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Assignee] nvarchar(max)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Note] varchar(max)  NULL,
    [Status_Id] int  NOT NULL
);
GO

-- Creating table 'Statements'
CREATE TABLE [dbo].[Statements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL
);
GO

-- Creating table 'ComponentProperty'
CREATE TABLE [dbo].[ComponentProperty] (
    [Components_Id] int  NOT NULL,
    [Properties_Id] int  NOT NULL
);
GO

-- Creating table 'LeaseCharge'
CREATE TABLE [dbo].[LeaseCharge] (
    [Charges_Id] int  NOT NULL,
    [Leases_Id] int  NOT NULL
);
GO

-- Creating table 'StandardComponentProperty'
CREATE TABLE [dbo].[StandardComponentProperty] (
    [StandardComponents_Id] int  NOT NULL,
    [Properties_Id] int  NOT NULL
);
GO

-- Creating table 'DepartmentCoordinator'
CREATE TABLE [dbo].[DepartmentCoordinator] (
    [Departments_Id] int  NOT NULL,
    [Coordinators_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Coordinators'
ALTER TABLE [dbo].[Coordinators]
ADD CONSTRAINT [PK_Coordinators]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Charges'
ALTER TABLE [dbo].[Charges]
ADD CONSTRAINT [PK_Charges]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [PK_Components]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Departments'
ALTER TABLE [dbo].[Departments]
ADD CONSTRAINT [PK_Departments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Leases'
ALTER TABLE [dbo].[Leases]
ADD CONSTRAINT [PK_Leases]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Makes'
ALTER TABLE [dbo].[Makes]
ADD CONSTRAINT [PK_Makes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [PK_Orders]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Overheads'
ALTER TABLE [dbo].[Overheads]
ADD CONSTRAINT [PK_Overheads]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'POes'
ALTER TABLE [dbo].[POes]
ADD CONSTRAINT [PK_POes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Properties'
ALTER TABLE [dbo].[Properties]
ADD CONSTRAINT [PK_Properties]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [PK_Repairs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Signatures'
ALTER TABLE [dbo].[Signatures]
ADD CONSTRAINT [PK_Signatures]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SingleCharges'
ALTER TABLE [dbo].[SingleCharges]
ADD CONSTRAINT [PK_SingleCharges]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Status'
ALTER TABLE [dbo].[Status]
ADD CONSTRAINT [PK_Status]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Types'
ALTER TABLE [dbo].[Types]
ADD CONSTRAINT [PK_Types]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VendorEmails'
ALTER TABLE [dbo].[VendorEmails]
ADD CONSTRAINT [PK_VendorEmails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'VendorRates'
ALTER TABLE [dbo].[VendorRates]
ADD CONSTRAINT [PK_VendorRates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StandardComponents'
ALTER TABLE [dbo].[StandardComponents]
ADD CONSTRAINT [PK_StandardComponents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Taxes'
ALTER TABLE [dbo].[Taxes]
ADD CONSTRAINT [PK_Taxes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Models'
ALTER TABLE [dbo].[Models]
ADD CONSTRAINT [PK_Models]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SystemGroups'
ALTER TABLE [dbo].[SystemGroups]
ADD CONSTRAINT [PK_SystemGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StandardOptionStandardComponents'
ALTER TABLE [dbo].[StandardOptionStandardComponents]
ADD CONSTRAINT [PK_StandardOptionStandardComponents]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StandardOptions'
ALTER TABLE [dbo].[StandardOptions]
ADD CONSTRAINT [PK_StandardOptions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Statements'
ALTER TABLE [dbo].[Statements]
ADD CONSTRAINT [PK_Statements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Components_Id], [Properties_Id] in table 'ComponentProperty'
ALTER TABLE [dbo].[ComponentProperty]
ADD CONSTRAINT [PK_ComponentProperty]
    PRIMARY KEY NONCLUSTERED ([Components_Id], [Properties_Id] ASC);
GO

-- Creating primary key on [Charges_Id], [Leases_Id] in table 'LeaseCharge'
ALTER TABLE [dbo].[LeaseCharge]
ADD CONSTRAINT [PK_LeaseCharge]
    PRIMARY KEY NONCLUSTERED ([Charges_Id], [Leases_Id] ASC);
GO

-- Creating primary key on [StandardComponents_Id], [Properties_Id] in table 'StandardComponentProperty'
ALTER TABLE [dbo].[StandardComponentProperty]
ADD CONSTRAINT [PK_StandardComponentProperty]
    PRIMARY KEY NONCLUSTERED ([StandardComponents_Id], [Properties_Id] ASC);
GO

-- Creating primary key on [Departments_Id], [Coordinators_Id] in table 'DepartmentCoordinator'
ALTER TABLE [dbo].[DepartmentCoordinator]
ADD CONSTRAINT [PK_DepartmentCoordinator]
    PRIMARY KEY NONCLUSTERED ([Departments_Id], [Coordinators_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TypeId] in table 'Charges'
ALTER TABLE [dbo].[Charges]
ADD CONSTRAINT [FK_ChargeType]
    FOREIGN KEY ([TypeId])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ChargeType'
CREATE INDEX [IX_FK_ChargeType]
ON [dbo].[Charges]
    ([TypeId]);
GO

-- Creating foreign key on [VendorRateId] in table 'Charges'
ALTER TABLE [dbo].[Charges]
ADD CONSTRAINT [FK_ChargeVendorRate]
    FOREIGN KEY ([VendorRateId])
    REFERENCES [dbo].[VendorRates]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ChargeVendorRate'
CREATE INDEX [IX_FK_ChargeVendorRate]
ON [dbo].[Charges]
    ([VendorRateId]);
GO

-- Creating foreign key on [ComponentId] in table 'Leases'
ALTER TABLE [dbo].[Leases]
ADD CONSTRAINT [FK_ComponentLease]
    FOREIGN KEY ([ComponentId])
    REFERENCES [dbo].[Components]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentLease'
CREATE INDEX [IX_FK_ComponentLease]
ON [dbo].[Leases]
    ([ComponentId]);
GO

-- Creating foreign key on [MakeId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_ComponentMake]
    FOREIGN KEY ([MakeId])
    REFERENCES [dbo].[Makes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentMake'
CREATE INDEX [IX_FK_ComponentMake]
ON [dbo].[Components]
    ([MakeId]);
GO

-- Creating foreign key on [ComponentId] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [FK_ComponentRepair]
    FOREIGN KEY ([ComponentId])
    REFERENCES [dbo].[Components]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentRepair'
CREATE INDEX [IX_FK_ComponentRepair]
ON [dbo].[Repairs]
    ([ComponentId]);
GO

-- Creating foreign key on [SignatureId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_ComponentSignature]
    FOREIGN KEY ([SignatureId])
    REFERENCES [dbo].[Signatures]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentSignature'
CREATE INDEX [IX_FK_ComponentSignature]
ON [dbo].[Components]
    ([SignatureId]);
GO

-- Creating foreign key on [StatusId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_ComponentStatus]
    FOREIGN KEY ([StatusId])
    REFERENCES [dbo].[Status]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentStatus'
CREATE INDEX [IX_FK_ComponentStatus]
ON [dbo].[Components]
    ([StatusId]);
GO

-- Creating foreign key on [TypeId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_ComponentType]
    FOREIGN KEY ([TypeId])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentType'
CREATE INDEX [IX_FK_ComponentType]
ON [dbo].[Components]
    ([TypeId]);
GO

-- Creating foreign key on [DepartmentId] in table 'Leases'
ALTER TABLE [dbo].[Leases]
ADD CONSTRAINT [FK_DepartmentLease]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DepartmentLease'
CREATE INDEX [IX_FK_DepartmentLease]
ON [dbo].[Leases]
    ([DepartmentId]);
GO

-- Creating foreign key on [DepartmentId] in table 'SingleCharges'
ALTER TABLE [dbo].[SingleCharges]
ADD CONSTRAINT [FK_DepartmentSingleCharge]
    FOREIGN KEY ([DepartmentId])
    REFERENCES [dbo].[Departments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DepartmentSingleCharge'
CREATE INDEX [IX_FK_DepartmentSingleCharge]
ON [dbo].[SingleCharges]
    ([DepartmentId]);
GO

-- Creating foreign key on [OverheadId] in table 'Leases'
ALTER TABLE [dbo].[Leases]
ADD CONSTRAINT [FK_LeaseOverhead]
    FOREIGN KEY ([OverheadId])
    REFERENCES [dbo].[Overheads]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LeaseOverhead'
CREATE INDEX [IX_FK_LeaseOverhead]
ON [dbo].[Leases]
    ([OverheadId]);
GO

-- Creating foreign key on [UserId] in table 'Orders'
ALTER TABLE [dbo].[Orders]
ADD CONSTRAINT [FK_UserOrder]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserOrder'
CREATE INDEX [IX_FK_UserOrder]
ON [dbo].[Orders]
    ([UserId]);
GO

-- Creating foreign key on [StatusId] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [FK_StatusRepair]
    FOREIGN KEY ([StatusId])
    REFERENCES [dbo].[Status]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StatusRepair'
CREATE INDEX [IX_FK_StatusRepair]
ON [dbo].[Repairs]
    ([StatusId]);
GO

-- Creating foreign key on [Components_Id] in table 'ComponentProperty'
ALTER TABLE [dbo].[ComponentProperty]
ADD CONSTRAINT [FK_ComponentProperty_Components]
    FOREIGN KEY ([Components_Id])
    REFERENCES [dbo].[Components]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Properties_Id] in table 'ComponentProperty'
ALTER TABLE [dbo].[ComponentProperty]
ADD CONSTRAINT [FK_ComponentProperty_Properties]
    FOREIGN KEY ([Properties_Id])
    REFERENCES [dbo].[Properties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ComponentProperty_Properties'
CREATE INDEX [IX_FK_ComponentProperty_Properties]
ON [dbo].[ComponentProperty]
    ([Properties_Id]);
GO

-- Creating foreign key on [Charges_Id] in table 'LeaseCharge'
ALTER TABLE [dbo].[LeaseCharge]
ADD CONSTRAINT [FK_LeaseCharge_Charges]
    FOREIGN KEY ([Charges_Id])
    REFERENCES [dbo].[Charges]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Leases_Id] in table 'LeaseCharge'
ALTER TABLE [dbo].[LeaseCharge]
ADD CONSTRAINT [FK_LeaseCharge_Leases]
    FOREIGN KEY ([Leases_Id])
    REFERENCES [dbo].[Leases]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LeaseCharge_Leases'
CREATE INDEX [IX_FK_LeaseCharge_Leases]
ON [dbo].[LeaseCharge]
    ([Leases_Id]);
GO

-- Creating foreign key on [StandardComponents_Id] in table 'StandardComponentProperty'
ALTER TABLE [dbo].[StandardComponentProperty]
ADD CONSTRAINT [FK_StandardComponentProperty_StandardComponent]
    FOREIGN KEY ([StandardComponents_Id])
    REFERENCES [dbo].[StandardComponents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Properties_Id] in table 'StandardComponentProperty'
ALTER TABLE [dbo].[StandardComponentProperty]
ADD CONSTRAINT [FK_StandardComponentProperty_Property]
    FOREIGN KEY ([Properties_Id])
    REFERENCES [dbo].[Properties]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StandardComponentProperty_Property'
CREATE INDEX [IX_FK_StandardComponentProperty_Property]
ON [dbo].[StandardComponentProperty]
    ([Properties_Id]);
GO

-- Creating foreign key on [ModelId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_ModelComponent]
    FOREIGN KEY ([ModelId])
    REFERENCES [dbo].[Models]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ModelComponent'
CREATE INDEX [IX_FK_ModelComponent]
ON [dbo].[Components]
    ([ModelId]);
GO

-- Creating foreign key on [ModelId] in table 'StandardComponents'
ALTER TABLE [dbo].[StandardComponents]
ADD CONSTRAINT [FK_ModelStandardComponent]
    FOREIGN KEY ([ModelId])
    REFERENCES [dbo].[Models]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ModelStandardComponent'
CREATE INDEX [IX_FK_ModelStandardComponent]
ON [dbo].[StandardComponents]
    ([ModelId]);
GO

-- Creating foreign key on [TypeId] in table 'StandardComponents'
ALTER TABLE [dbo].[StandardComponents]
ADD CONSTRAINT [FK_StandardComponentType]
    FOREIGN KEY ([TypeId])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StandardComponentType'
CREATE INDEX [IX_FK_StandardComponentType]
ON [dbo].[StandardComponents]
    ([TypeId]);
GO

-- Creating foreign key on [MakeId] in table 'StandardComponents'
ALTER TABLE [dbo].[StandardComponents]
ADD CONSTRAINT [FK_MakeStandardComponent]
    FOREIGN KEY ([MakeId])
    REFERENCES [dbo].[Makes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MakeStandardComponent'
CREATE INDEX [IX_FK_MakeStandardComponent]
ON [dbo].[StandardComponents]
    ([MakeId]);
GO

-- Creating foreign key on [TypeId] in table 'VendorRates'
ALTER TABLE [dbo].[VendorRates]
ADD CONSTRAINT [FK_VendorRateType]
    FOREIGN KEY ([TypeId])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_VendorRateType'
CREATE INDEX [IX_FK_VendorRateType]
ON [dbo].[VendorRates]
    ([TypeId]);
GO

-- Creating foreign key on [TaxId] in table 'Charges'
ALTER TABLE [dbo].[Charges]
ADD CONSTRAINT [FK_TaxCharge]
    FOREIGN KEY ([TaxId])
    REFERENCES [dbo].[Taxes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaxCharge'
CREATE INDEX [IX_FK_TaxCharge]
ON [dbo].[Charges]
    ([TaxId]);
GO

-- Creating foreign key on [SystemGroupId] in table 'Leases'
ALTER TABLE [dbo].[Leases]
ADD CONSTRAINT [FK_SystemGroupLease]
    FOREIGN KEY ([SystemGroupId])
    REFERENCES [dbo].[SystemGroups]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SystemGroupLease'
CREATE INDEX [IX_FK_SystemGroupLease]
ON [dbo].[Leases]
    ([SystemGroupId]);
GO

-- Creating foreign key on [UserId] in table 'SystemGroups'
ALTER TABLE [dbo].[SystemGroups]
ADD CONSTRAINT [FK_SystemGroupUser]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SystemGroupUser'
CREATE INDEX [IX_FK_SystemGroupUser]
ON [dbo].[SystemGroups]
    ([UserId]);
GO

-- Creating foreign key on [OrderId] in table 'SystemGroups'
ALTER TABLE [dbo].[SystemGroups]
ADD CONSTRAINT [FK_SystemGroupOrder]
    FOREIGN KEY ([OrderId])
    REFERENCES [dbo].[Orders]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SystemGroupOrder'
CREATE INDEX [IX_FK_SystemGroupOrder]
ON [dbo].[SystemGroups]
    ([OrderId]);
GO

-- Creating foreign key on [MakeId] in table 'Models'
ALTER TABLE [dbo].[Models]
ADD CONSTRAINT [FK_MakeModel]
    FOREIGN KEY ([MakeId])
    REFERENCES [dbo].[Makes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MakeModel'
CREATE INDEX [IX_FK_MakeModel]
ON [dbo].[Models]
    ([MakeId]);
GO

-- Creating foreign key on [SystemGroupId] in table 'Components'
ALTER TABLE [dbo].[Components]
ADD CONSTRAINT [FK_SystemGroupComponent]
    FOREIGN KEY ([SystemGroupId])
    REFERENCES [dbo].[SystemGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SystemGroupComponent'
CREATE INDEX [IX_FK_SystemGroupComponent]
ON [dbo].[Components]
    ([SystemGroupId]);
GO

-- Creating foreign key on [POId] in table 'SystemGroups'
ALTER TABLE [dbo].[SystemGroups]
ADD CONSTRAINT [FK_POSystemGroup]
    FOREIGN KEY ([POId])
    REFERENCES [dbo].[POes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_POSystemGroup'
CREATE INDEX [IX_FK_POSystemGroup]
ON [dbo].[SystemGroups]
    ([POId]);
GO

-- Creating foreign key on [StandardComponentId] in table 'StandardOptionStandardComponents'
ALTER TABLE [dbo].[StandardOptionStandardComponents]
ADD CONSTRAINT [FK_StandardOptionStandardComponentStandardComponent]
    FOREIGN KEY ([StandardComponentId])
    REFERENCES [dbo].[StandardComponents]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StandardOptionStandardComponentStandardComponent'
CREATE INDEX [IX_FK_StandardOptionStandardComponentStandardComponent]
ON [dbo].[StandardOptionStandardComponents]
    ([StandardComponentId]);
GO

-- Creating foreign key on [TypeId] in table 'Overheads'
ALTER TABLE [dbo].[Overheads]
ADD CONSTRAINT [FK_TypeOverhead]
    FOREIGN KEY ([TypeId])
    REFERENCES [dbo].[Types]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TypeOverhead'
CREATE INDEX [IX_FK_TypeOverhead]
ON [dbo].[Overheads]
    ([TypeId]);
GO

-- Creating foreign key on [StandardOptionId] in table 'StandardOptionStandardComponents'
ALTER TABLE [dbo].[StandardOptionStandardComponents]
ADD CONSTRAINT [FK_StandardOptionStandardOptionStandardComponent]
    FOREIGN KEY ([StandardOptionId])
    REFERENCES [dbo].[StandardOptions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StandardOptionStandardOptionStandardComponent'
CREATE INDEX [IX_FK_StandardOptionStandardOptionStandardComponent]
ON [dbo].[StandardOptionStandardComponents]
    ([StandardOptionId]);
GO

-- Creating foreign key on [Status_Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_TaskStatus]
    FOREIGN KEY ([Status_Id])
    REFERENCES [dbo].[Status]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskStatus'
CREATE INDEX [IX_FK_TaskStatus]
ON [dbo].[Tasks]
    ([Status_Id]);
GO

-- Creating foreign key on [Departments_Id] in table 'DepartmentCoordinator'
ALTER TABLE [dbo].[DepartmentCoordinator]
ADD CONSTRAINT [FK_DepartmentCoordinator_Department]
    FOREIGN KEY ([Departments_Id])
    REFERENCES [dbo].[Departments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Coordinators_Id] in table 'DepartmentCoordinator'
ALTER TABLE [dbo].[DepartmentCoordinator]
ADD CONSTRAINT [FK_DepartmentCoordinator_Coordinator]
    FOREIGN KEY ([Coordinators_Id])
    REFERENCES [dbo].[Coordinators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DepartmentCoordinator_Coordinator'
CREATE INDEX [IX_FK_DepartmentCoordinator_Coordinator]
ON [dbo].[DepartmentCoordinator]
    ([Coordinators_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------