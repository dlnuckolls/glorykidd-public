--changeset dlnuckolls:1 labels:create sp getallpersons context:initial creation
--comment: Creating the initial sp
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllPersons]
AS
BEGIN
  SELECT * FROM dbo.person;
END
--rollback DROP usp_GetAllPersons;