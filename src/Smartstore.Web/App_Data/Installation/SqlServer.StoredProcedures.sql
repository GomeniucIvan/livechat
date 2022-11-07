CREATE PROCEDURE [dbo].[Company_GetDetails]
     (@CompanyId INT)
AS
BEGIN

    SELECT TOP 1 c.Id AS Id,
				 c.Name AS Name,
				 c.Hash AS Hash

    FROM   dbo.Company c WITH(NOLOCK)
    WHERE  c.Deleted = 0 AND c.Id = @CompanyId;

END;
GO

CREATE PROCEDURE [dbo].[CompanyGuestCustomer_CreateAndOrGetDetails]
     (
    @CompanyId INT,
	@UniqueId NVARCHAR(MAX),
	@Guid NVARCHAR(36))
AS
BEGIN
    DECLARE @companyGuestCustomerId INT;

    IF EXISTS (SELECT 1
               FROM   dbo.CompanyGuestCustomer cgc WITH(NOLOCK)
               WHERE  cgc.CompanyId = @CompanyId
                      AND cgc.Deleted = 0
                      AND ((ISNULL(@UniqueId, '') <> '' AND cgc.CustomerUniqueId = @UniqueId)
                           OR (ISNULL(@UniqueId, '') <> '' OR (ISNULL(@Guid, '') <> '' AND cgc.Guid = @Guid))))
    BEGIN
        SET @companyGuestCustomerId = (SELECT TOP 1 cgc.Id
                                       FROM   dbo.CompanyGuestCustomer cgc WITH(NOLOCK)
                                       WHERE  cgc.CompanyId = @CompanyId
                                              AND cgc.Deleted = 0
                                              AND ((ISNULL(@UniqueId, '') <> '' AND cgc.CustomerUniqueId = @UniqueId)
                                                   OR (ISNULL(@UniqueId, '') <> ''
                                                       OR (ISNULL(@Guid, '') <> '' AND cgc.Guid = @Guid))));
    END;
    ELSE
    BEGIN
        INSERT INTO dbo.CompanyGuestCustomer
        (   Guid,
            CustomerUniqueId,
            Deleted,
            CompanyId)
        VALUES
             (@Guid,          -- Guid - uniqueidentifier
              @UniqueId,      -- CompanyUniqueId - nvarchar(max)
              CAST(0 AS BIT), -- Deleted - bit
              @CompanyId      -- CompanyId - int
            );

        SET @companyGuestCustomerId = SCOPE_IDENTITY();
    END;

    SELECT cgc.Id, 
           cgc.Guid, 
           cgc.CustomerUniqueId, 
           cgc.Deleted, 
           cgc.CompanyId

    FROM   dbo.CompanyGuestCustomer cgc WITH(NOLOCK)
    WHERE  cgc.Id = @companyGuestCustomerId 
           AND cgc.CompanyId = @CompanyId;
END;
GO

CREATE PROCEDURE [dbo].[CompanyMessage_Insert]
     (
    @CompanyId INT,
	@CompanyCustomerId INT,
	@CompanyGuestCustomerId INT,
	@Message nvarchar(MAX))
AS
BEGIN

    INSERT INTO dbo.CompanyMessage
    (   Message,
        CompanyCustomerId,
        CompanyGuestCustomerId,
        CompanyId,
		CreatedOnUTc)
    VALUES
         (@Message,                 -- Message - nvarchar(max)
          @CompanyCustomerId,       -- CompanyCustomerId - int
          @CompanyGuestCustomerId,  -- CompanyGuestCustomerId - int
          @CompanyId,               -- CompanyId - int
          GETUTCDATE())             -- CreatedOnUTc - datetime
END;
GO

CREATE PROCEDURE [dbo].[CompanyMessage_GetList]
     (
    @CompanyId int, 
	@CompanyGuestCustomerId int, 
	@CompanyCustomerId int)
AS
BEGIN
    SELECT cm.Id, 
		   cm.Message, 
		   cm.CompanyCustomerId, 
		   cm.CompanyGuestCustomerId,
		   cm.CompanyId, 
		   cm.CreatedOnUtc

    FROM   dbo.CompanyMessage cm WITH(NOLOCK)
    WHERE  (ISNULL(cm.CompanyCustomerId, 0) = 0 OR cm.CompanyCustomerId = @CompanyCustomerId)
           AND (ISNULL(cm.CompanyGuestCustomerId, 0) = 0 OR cm.CompanyGuestCustomerId = @CompanyGuestCustomerId)
           AND cm.CompanyId = @CompanyId;
END;
GO