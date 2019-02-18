create table PPCategories
(
  NoCategorie int not null
    constraint PK_PPCategories
      primary key,
  Description varchar(50),
  Details     varchar(max)
)
go

create table PPClients
(
  NoClient              bigint not null
    constraint PK_PPClients
      primary key,
  AdresseEmail          varchar(100),
  MotDePasse            varchar(50),
  Nom                   varchar(50),
  Prenom                varchar(50),
  Rue                   varchar(50),
  Ville                 varchar(50),
  Province              char(2),
  CodePostal            varchar(7),
  Pays                  varchar(10),
  Tel1                  varchar(20),
  Tel2                  varchar(20),
  DateCreation          smalldatetime,
  DateMAJ               smalldatetime,
  NbConnexions          smallint,
  DateDerniereConnexion smalldatetime,
  Statut                smallint
)
go

create table PPGestionnaires
(
  NoGestionnaire bigint not null,
  Nom            varchar(50),
  Prenom         varchar(50),
  AdresseEmail   varchar(100),
  MotDePasse     varchar(50)
)
go

create table PPHistoriquePaiements
(
  NoHistorique               bigint not null
    constraint PK_PPHistoriquePaiements
      primary key,
  MontantVenteAvantLivraison smallmoney,
  NoVendeur                  bigint,
  NoClient                   bigint,
  NoCommande                 bigint,
  DateVente                  smalldatetime,
  NoAutorisation             varchar(50),
  FraisLesi                  smallmoney,
  Redevance                  smallmoney,
  FraisLivraison             smallmoney,
  FraisTPS                   smallmoney,
  FraisTVQ                   smallmoney
)
go

create table PPLieu
(
  NoLieu      smallint not null
    constraint PK_PPLieu
      primary key,
  Description nvarchar(50)
)
go

create table PPMessages
(
  NoMsg        int not null
    constraint PK_PPMessages
      primary key,
  NoExpediteur int,
  DescMsg      nvarchar(max),
  FichierJoint sql_variant,
  Lieu         smallint
    constraint FK_PPMessages_PPLieu
      references PPLieu,
  dateEnvoi    smalldatetime,
  objet        nvarchar(50)
)
go

create table PPDestinataires
(
  NoMsg          int not null
    constraint FK_PPDestinataires_PPMessages
      references PPMessages,
  NoDestinataire int not null,
  EtatLu         smallint,
  Lieu           smallint
    constraint FK_PPDestinataires_PPLieu
      references PPLieu,
  constraint PK_PPDestinataires
    primary key (NoMsg, NoDestinataire)
)
go

create table PPTaxeFederale
(
  NoTPS            tinyint not null
    constraint PK_PPTaxeFederale
      primary key,
  DateEffectiveTPS smalldatetime,
  TauxTPS          numeric(4, 2)
)
go

create table PPTaxeProvinciale
(
  NoTVQ            tinyint not null
    constraint PK_PPTaxeProvinciale
      primary key,
  DateEffectiveTVQ smalldatetime,
  TauxTVQ          numeric(5, 3)
)
go

create table PPTypesLivraison
(
  CodeLivraison smallint not null
    constraint PK_PPTypesLivraison
      primary key,
  Description   varchar(50)
)
go

create table PPTypesPoids
(
  CodePoids smallint not null
    constraint PK_PPTypesPoids
      primary key,
  PoidsMin  numeric(8, 1),
  PoidsMax  numeric(8, 1)
)
go

create table PPPoidsLivraisons
(
  CodeLivraison smallint not null
    constraint FK_PPPoidsLivraisons_PPTypesLivraison
      references PPTypesLivraison,
  CodePoids     smallint not null
    constraint FK_PPPoidsLivraisons_PPTypesPoids
      references PPTypesPoids,
  Tarif         smallmoney,
  constraint PK_PPPoidsLivraisons
    primary key (CodeLivraison, CodePoids)
)
go

create table PPVendeurs
(
  NoVendeur         bigint not null
    constraint PK_PPVendeurs
      primary key,
  NomAffaires       varchar(50),
  Nom               varchar(50),
  Prenom            varchar(50),
  Rue               varchar(50),
  Ville             varchar(50),
  Province          char(2),
  CodePostal        varchar(7),
  Pays              varchar(10),
  Tel1              varchar(20),
  Tel2              varchar(20),
  AdresseEmail      varchar(100),
  MotDePasse        varchar(50),
  PoidsMaxLivraison int,
  LivraisonGratuite smallmoney,
  Taxes             bit,
  Pourcentage       numeric(4, 2),
  Configuration     varchar(512),
  DateCreation      smalldatetime,
  DateMAJ           smalldatetime,
  Statut            smallint
)
go

create table PPCommandes
(
  NoCommande           bigint not null
    constraint PK_PPCommandes
      primary key,
  NoClient             bigint
    constraint FK_PPCommandes_PPClients
      references PPClients,
  NoVendeur            bigint
    constraint FK_PPCommandes_PPVendeurs
      references PPVendeurs,
  DateCommande         smalldatetime,
  CoutLivraison        smallmoney,
  TypeLivraison        smallint
    constraint FK_PPCommandes_PPTypesLivraison
      references PPTypesLivraison,
  MontantTotAvantTaxes smallmoney,
  TPS                  smallmoney,
  TVQ                  smallmoney,
  PoidsTotal           numeric(8, 1),
  Statut               char,
  NoAutorisation       varchar(50)
)
go

create table PPProduits
(
  NoProduit     bigint not null
    constraint PK_PPProduits
      primary key,
  NoVendeur     bigint
    constraint FK_PPProduits_PPVendeurs1
      references PPVendeurs,
  NoCategorie   int
    constraint FK_PPProduits_PPCategories
      references PPCategories,
  Nom           varchar(50),
  Description   varchar(max),
  Photo         varchar(50),
  PrixDemande   smallmoney,
  NombreItems   smallint,
  Disponibilité bit,
  DateVente     smalldatetime,
  PrixVente     smallmoney,
  Poids         numeric(8, 1),
  DateCreation  smalldatetime,
  DateMAJ       smalldatetime
)
go

create table PPArticlesEnPanier
(
  NoPanier     bigint not null
    constraint PK_PPArticlesEnPanier
      primary key,
  NoClient     bigint
    constraint FK_PPArticlesEnPanier_PPClients
      references PPClients,
  NoVendeur    bigint
    constraint FK_PPArticlesEnPanier_PPVendeurs
      references PPVendeurs,
  NoProduit    bigint
    constraint FK_PPArticlesEnPanier_PPProduits
      references PPProduits,
  DateCreation smalldatetime,
  NbItems      smallint
)
go

create table PPDetailsCommandes
(
  NoDetailCommandes bigint not null
    constraint PK_PPDetailsCommandes
      primary key,
  NoCommande        bigint
    constraint FK_PPDetailsCommandes_PPCommandes
      references PPCommandes,
  NoProduit         bigint
    constraint FK_PPDetailsCommandes_PPProduits
      references PPProduits,
  PrixVente         smallmoney,
  Quantité          smallint
)
go

create table PPVendeursClients
(
  NoVendeur  bigint        not null
    constraint FK_PPVendeursClients_PPVendeurs
      references PPVendeurs,
  NoClient   bigint        not null
    constraint FK_PPVendeursClients_PPClients
      references PPClients,
  DateVisite smalldatetime not null,
  constraint PK_PPVendeursClients
    primary key (NoVendeur, NoClient, DateVisite)
)
go

create table sysdiagrams
(
  name         nvarchar(128) not null,
  principal_id int           not null,
  diagram_id   int           not null,
  version      int,
  definition   varbinary(max)
)
go


CREATE FUNCTION dbo.fn_diagramobjects()
  RETURNS int
  WITH EXECUTE AS N'dbo'
AS
BEGIN
  declare @id_upgraddiagrams int
  declare @id_sysdiagrams int
  declare @id_helpdiagrams int
  declare @id_helpdiagramdefinition int
  declare @id_creatediagram int
  declare @id_renamediagram int
  declare @id_alterdiagram int
  declare @id_dropdiagram int
  declare @InstalledObjects int

  select @InstalledObjects = 0

  select @id_upgraddiagrams = object_id(N'dbo.sp_upgraddiagrams'),
         @id_sysdiagrams = object_id(N'dbo.sysdiagrams'),
         @id_helpdiagrams = object_id(N'dbo.sp_helpdiagrams'),
         @id_helpdiagramdefinition = object_id(N'dbo.sp_helpdiagramdefinition'),
         @id_creatediagram = object_id(N'dbo.sp_creatediagram'),
         @id_renamediagram = object_id(N'dbo.sp_renamediagram'),
         @id_alterdiagram = object_id(N'dbo.sp_alterdiagram'),
         @id_dropdiagram = object_id(N'dbo.sp_dropdiagram')

  if @id_upgraddiagrams is not null
    select @InstalledObjects = @InstalledObjects + 1
  if @id_sysdiagrams is not null
    select @InstalledObjects = @InstalledObjects + 2
  if @id_helpdiagrams is not null
    select @InstalledObjects = @InstalledObjects + 4
  if @id_helpdiagramdefinition is not null
    select @InstalledObjects = @InstalledObjects + 8
  if @id_creatediagram is not null
    select @InstalledObjects = @InstalledObjects + 16
  if @id_renamediagram is not null
    select @InstalledObjects = @InstalledObjects + 32
  if @id_alterdiagram is not null
    select @InstalledObjects = @InstalledObjects + 64
  if @id_dropdiagram is not null
    select @InstalledObjects = @InstalledObjects + 128

  return @InstalledObjects
END
go


CREATE PROCEDURE dbo.sp_alterdiagram(@diagramname sysname,
                                     @owner_id int = null,
                                     @version int,
                                     @definition varbinary(max))
  WITH EXECUTE AS 'dbo'
AS
BEGIN
  set nocount on

  declare @theId int
  declare @retval int
  declare @IsDbo int

  declare @UIDFound int
  declare @DiagId int
  declare @ShouldChangeUID int

  if (@diagramname is null)
    begin
      RAISERROR ('Invalid ARG', 16, 1)
      return -1
    end

  execute as caller;
  select @theId = DATABASE_PRINCIPAL_ID();
  select @IsDbo = IS_MEMBER(N'db_owner');
  if (@owner_id is null)
    select @owner_id = @theId;
  revert;

  select @ShouldChangeUID = 0
  select @DiagId = diagram_id, @UIDFound = principal_id
  from dbo.sysdiagrams
  where principal_id = @owner_id
    and name = @diagramname

  if (@DiagId IS NULL or (@IsDbo = 0 and @theId <> @UIDFound))
    begin
      RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
      return -3
    end

  if (@IsDbo <> 0)
    begin
      if (@UIDFound is null or USER_NAME(@UIDFound) is null) -- invalid principal_id
        begin
          select @ShouldChangeUID = 1 ;
        end
    end

  -- update dds data			
  update dbo.sysdiagrams set definition = @definition where diagram_id = @DiagId;

  -- change owner
  if (@ShouldChangeUID = 1)
    update dbo.sysdiagrams set principal_id = @theId where diagram_id = @DiagId;

  -- update dds version
  if (@version is not null)
    update dbo.sysdiagrams set version = @version where diagram_id = @DiagId ;

  return 0
END
go


CREATE PROCEDURE dbo.sp_creatediagram(@diagramname sysname,
                                      @owner_id int = null,
                                      @version int,
                                      @definition varbinary(max))
  WITH EXECUTE AS 'dbo'
AS
BEGIN
  set nocount on

  declare @theId int
  declare @retval int
  declare @IsDbo int
  declare @userName sysname
  if (@version is null or @diagramname is null)
    begin
      RAISERROR (N'E_INVALIDARG', 16, 1);
      return -1
    end

  execute as caller;
  select @theId = DATABASE_PRINCIPAL_ID();
  select @IsDbo = IS_MEMBER(N'db_owner');
  revert;

  if @owner_id is null
    begin
      select @owner_id = @theId;
    end
  else
    begin
      if @theId <> @owner_id
        begin
          if @IsDbo = 0
            begin
              RAISERROR (N'E_INVALIDARG', 16, 1);
              return -1
            end
          select @theId = @owner_id
        end
    end
  -- next 2 line only for test, will be removed after define name unique
  if EXISTS(select diagram_id from dbo.sysdiagrams where principal_id = @theId and name = @diagramname)
    begin
      RAISERROR ('The name is already used.', 16, 1);
      return -2
    end

  insert into dbo.sysdiagrams(name, principal_id, version, definition)
  VALUES (@diagramname, @theId, @version, @definition);

  select @retval = @@IDENTITY
  return @retval
END
go


CREATE PROCEDURE dbo.sp_dropdiagram(@diagramname sysname,
                                    @owner_id int = null)
  WITH EXECUTE AS 'dbo'
AS
BEGIN
  set nocount on
  declare @theId int
  declare @IsDbo int

  declare @UIDFound int
  declare @DiagId int

  if (@diagramname is null)
    begin
      RAISERROR ('Invalid value', 16, 1);
      return -1
    end

  EXECUTE AS CALLER;
  select @theId = DATABASE_PRINCIPAL_ID();
  select @IsDbo = IS_MEMBER(N'db_owner');
  if (@owner_id is null)
    select @owner_id = @theId;
  REVERT;

  select @DiagId = diagram_id, @UIDFound = principal_id
  from dbo.sysdiagrams
  where principal_id = @owner_id
    and name = @diagramname
  if (@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
    begin
      RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1)
      return -3
    end

  delete from dbo.sysdiagrams where diagram_id = @DiagId;

  return 0;
END
go


CREATE PROCEDURE dbo.sp_helpdiagramdefinition(@diagramname sysname,
                                              @owner_id int = null)
  WITH EXECUTE AS N'dbo'
AS
BEGIN
  set nocount on

  declare @theId int
  declare @IsDbo int
  declare @DiagId int
  declare @UIDFound int

  if (@diagramname is null)
    begin
      RAISERROR (N'E_INVALIDARG', 16, 1);
      return -1
    end

  execute as caller;
  select @theId = DATABASE_PRINCIPAL_ID();
  select @IsDbo = IS_MEMBER(N'db_owner');
  if (@owner_id is null)
    select @owner_id = @theId;
  revert;

  select @DiagId = diagram_id, @UIDFound = principal_id
  from dbo.sysdiagrams
  where principal_id = @owner_id
    and name = @diagramname;
  if (@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
    begin
      RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1);
      return -3
    end

  select version, definition FROM dbo.sysdiagrams where diagram_id = @DiagId;
  return 0
END
go


CREATE PROCEDURE dbo.sp_helpdiagrams(@diagramname sysname = NULL,
                                     @owner_id int = NULL)
  WITH EXECUTE AS N'dbo'
AS
BEGIN
  DECLARE @user sysname
  DECLARE @dboLogin bit
  EXECUTE AS CALLER;
  SET @user = USER_NAME();
  SET @dboLogin = CONVERT(bit, IS_MEMBER('db_owner'));
  REVERT;
  SELECT [Database] = DB_NAME(),
         [Name]     = name,
         [ID]       = diagram_id,
         [Owner]    = USER_NAME(principal_id),
         [OwnerID]  = principal_id
  FROM sysdiagrams
  WHERE (@dboLogin = 1 OR USER_NAME(principal_id) = @user)
    AND (@diagramname IS NULL OR name = @diagramname)
    AND (@owner_id IS NULL OR principal_id = @owner_id)
  ORDER BY 4, 5, 1
END
go


CREATE PROCEDURE dbo.sp_renamediagram(@diagramname sysname,
                                      @owner_id int = null,
                                      @new_diagramname sysname)
  WITH EXECUTE AS 'dbo'
AS
BEGIN
  set nocount on
  declare @theId int
  declare @IsDbo int

  declare @UIDFound int
  declare @DiagId int
  declare @DiagIdTarg int
  declare @u_name sysname
  if ((@diagramname is null) or (@new_diagramname is null))
    begin
      RAISERROR ('Invalid value', 16, 1);
      return -1
    end

  EXECUTE AS CALLER;
  select @theId = DATABASE_PRINCIPAL_ID();
  select @IsDbo = IS_MEMBER(N'db_owner');
  if (@owner_id is null)
    select @owner_id = @theId;
  REVERT;

  select @u_name = USER_NAME(@owner_id)

  select @DiagId = diagram_id, @UIDFound = principal_id
  from dbo.sysdiagrams
  where principal_id = @owner_id
    and name = @diagramname
  if (@DiagId IS NULL or (@IsDbo = 0 and @UIDFound <> @theId))
    begin
      RAISERROR ('Diagram does not exist or you do not have permission.', 16, 1)
      return -3
    end

  -- if((@u_name is not null) and (@new_diagramname = @diagramname))	-- nothing will change
  --	return 0;

  if (@u_name is null)
    select @DiagIdTarg = diagram_id from dbo.sysdiagrams where principal_id = @theId and name = @new_diagramname
  else
    select @DiagIdTarg = diagram_id from dbo.sysdiagrams where principal_id = @owner_id and name = @new_diagramname

  if ((@DiagIdTarg is not null) and @DiagId <> @DiagIdTarg)
    begin
      RAISERROR ('The name is already used.', 16, 1);
      return -2
    end

  if (@u_name is null)
    update dbo.sysdiagrams set [name] = @new_diagramname, principal_id = @theId where diagram_id = @DiagId
  else
    update dbo.sysdiagrams set [name] = @new_diagramname where diagram_id = @DiagId
  return 0
END
go


CREATE PROCEDURE dbo.sp_upgraddiagrams
AS
BEGIN
  IF OBJECT_ID(N'dbo.sysdiagrams') IS NOT NULL
    return 0;

  CREATE TABLE dbo.sysdiagrams
  (
    name         sysname NOT NULL,
    principal_id int     NOT NULL, -- we may change it to varbinary(85)
    diagram_id   int PRIMARY KEY IDENTITY,
    version      int,

    definition   varbinary(max)
      CONSTRAINT UK_principal_name UNIQUE
        (
         principal_id,
         name
          )
  );


  /* Add this if we need to have some form of extended properties for diagrams */
  /*
  IF OBJECT_ID(N'dbo.sysdiagram_properties') IS NULL
  BEGIN
    CREATE TABLE dbo.sysdiagram_properties
    (
      diagram_id int,
      name sysname,
      value varbinary(max) NOT NULL
    )
  END
  */

  IF OBJECT_ID(N'dbo.dtproperties') IS NOT NULL
    begin
      insert into dbo.sysdiagrams
      ([name],
       [principal_id],
       [version],
       [definition])
      select convert(sysname, dgnm.[uvalue]),
             DATABASE_PRINCIPAL_ID(N'dbo'), -- will change to the sid of sa
             0,                             -- zero for old format, dgdef.[version],
             dgdef.[lvalue]
      from dbo.[dtproperties] dgnm
             inner join dbo.[dtproperties] dggd
                        on dggd.[property] = 'DtgSchemaGUID' and dggd.[objectid] = dgnm.[objectid]
             inner join dbo.[dtproperties] dgdef
                        on dgdef.[property] = 'DtgSchemaDATA' and dgdef.[objectid] = dgnm.[objectid]

      where dgnm.[property] = 'DtgSchemaNAME'
        and dggd.[uvalue] like N'_EA3E6268-D998-11CE-9454-00AA00A3F36E_'
      return 2;
    end
  return 1;
END
go


