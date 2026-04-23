-- BEGIN TRAN

DECLARE 
  @OwnerName nvarchar(50) = 'lyle@luppes.com',
	@Set3Name nvarchar(50),
	@Set3Id int,
	@List31Id int,
	@Category31Id int,
	@Category32Id int,
	@Category33Id int,
	@Category34Id int

SET	@Set3Name = 'RV Goddess - Setup Tasks'

DELETE FROM [CheckList].[TemplateAction] Where CategoryId IN (Select tl.ListId From TemplateSet ts INNER JOIN TemplateList tl on ts.SetId = tl.SetId INNER JOIN TemplateCategory tc on tc.ListId = tl.ListId Where SetName = @Set3Name)
DELETE FROM [CheckList].[TemplateCategory] Where ListId IN (Select tl.ListId From TemplateSet ts INNER JOIN TemplateList tl on ts.SetId = tl.SetId Where SetName = @Set3Name)
DELETE FROM [CheckList].[TemplateList] Where SetId IN (Select SetId From TemplateSet ts Where SetName = @Set3Name)
DELETE FROM [CheckList].[TemplateSet] Where SetName = @Set3Name

Insert into [CheckList].[TemplateSet](SetName, SetDscr, OwnerName) Values (@Set3Name, '', @OwnerName)
	SET @Set3Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateList](ListName, ListDscr, SetId) Values ('Departure List', '', @Set3Id)
	SET @List31Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Interior', @List31Id)
	SET @Category31Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Passenger Area', @List31Id)
	SET @Category32Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Exterior', @List31Id)
	SET @Category33Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('At Departure', @List31Id)
	SET @Category34Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateAction](ActionText, CategoryId, SortOrder) Values 
	('Items stored', @Category31Id, 10),
	('Cupboard doors closed', @Category31Id, 20),
	('Ceiling vents closed', @Category31Id, 30),
	('Windows closed', @Category31Id, 40),
	('TV/satellite dish down', @Category31Id, 50),
	('TV antenna down', @Category31Id, 60),
	('Refrigerator items secure', @Category31Id, 70),
	('Refrigerator power selected', @Category31Id, 80),
	('Water pump turned off', @Category31Id, 90),
	('Slide rooms in and secured', @Category31Id, 100),
	('Interior doors secured', @Category31Id, 110),
	('Turn off water heater/furnace/air conditioner', @Category31Id, 120),
	('Discard trash/recycling', @Category31Id, 130),

	('Drink/coffee cups filled', @Category32Id, 10),
	('Seat belts located', @Category32Id, 20),
	('Note mileage in Camping Journal', @Category32Id, 30),
	('Music/radio ready', @Category32Id, 40),
	('Maps/Route/GPS ready', @Category32Id, 50),
	('Kids games/books/music/videos/snacks ready', @Category32Id, 60),
	('Cell phone', @Category32Id, 70),
	('All keys accounted for', @Category32Id, 80),
	('Sunglasses', @Category32Id, 90),
	('Tissues/wet wipes', @Category32Id, 100),
	('Pets walked & happy', @Category32Id, 110),

	('Drain holding tanks', @Category33Id, 10),
	('Check fresh water tank level', @Category33Id, 20),
	('Power, water, sewer, cable, etc. unplugged from RV', @Category33Id, 30),
	('Propane', @Category33Id, 40),
	('Range vent secured', @Category33Id, 50),
	('Outside storage bay door secured/locked', @Category33Id, 60),
	('Raise leveling jacks', @Category33Id, 70),
	('Remove wheel blocks', @Category33Id, 80),
	('Windshields cleaned', @Category33Id, 90),
	('Tire pressure checked', @Category33Id, 100),
	('Fluid levels checked', @Category33Id, 110),
	('Entry steps stowed', @Category33Id, 120),

	('Safety inspection - turn signals, brakes, headlights', @Category34Id, 10),
	('RV walk-around: windows, slide rooms, roof antennas/vents, steps', @Category34Id, 20),
	('Tow vehicle walk-around, hitch inspection, safety inspection', @Category34Id, 30),
	('Inspect site for left items, litter', @Category34Id, 40)


 Select s.SetId, s.SetName, l.ListId, l.ListName, l.ListDscr, c.CategoryId, c.CategoryText, a.ActionId, a.ActionText, a.ActionDscr
 From [CheckList].[TemplateSet] s
 INNER JOIN [CheckList].[TemplateList] l on s.SetId = l.SetId
 INNER JOIN [CheckList].[TemplateCategory] c on c.ListId = l.ListId
 INNER JOIN [CheckList].[TemplateAction] a on a.CategoryId = c.CategoryId
 WHERE s.SetId = @Set3Id 
