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

GO
CREATE PROCEDURE [dbo].[CompanyGuestCustomer_CreateAndOrGetDetails]
     (@CompanyId int,
	 @UniqueId nvarchar(MAX),
	 @Guid nvarchar(36))
AS
BEGIN

	IF EXISTS (SELECT 1 FROM dbo.CompanyGuestCustomer cgc WITH(NOLOCK)
                                                                     WHERE cgc.CompanyId = @CompanyId 
																	 AND cgc.Deleted = 0
																	 AND  ((ISNULL(@UniqueId,'') != '' AND cgc.UniqueId = @UniqueId) OR 
																			(ISNULL(@Guid,'') != '' AND cgc.Guid = @Guid))
																	 BEGIN

																	 END
																	 ELSE 
																	 END

END;