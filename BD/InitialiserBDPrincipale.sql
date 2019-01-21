USE BD6B8_424Q

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

create table sysdiagrams
(
  name         nvarchar(128) not null,
  principal_id int           not null,
  diagram_id   int           not null,
  version      int,
  definition   varbinary(max)
)
go

create table PPCategories
(
  NoCategorie int not null
    constraint PK_PPCategories
    primary key,
  Description varchar(50),
  Details     varchar(max)
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
  Statut               char(1),
  NoAutorisation       varchar(50)
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
