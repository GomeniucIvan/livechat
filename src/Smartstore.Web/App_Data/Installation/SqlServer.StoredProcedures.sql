CREATE PROCEDURE [dbo].[Company_GetDetails]
     (@CompanyId int = NULL)
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
     (@CompanyId int = NULL,
	 @UniqueId nvarchar(MAX),
	 @Guid nvarchar(36))
AS
BEGIN


END;