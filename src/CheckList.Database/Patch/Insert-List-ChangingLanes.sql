-- BEGIN TRAN

DECLARE 
	@OwnerName nvarchar(50) = 'lyle@luppes.com',
	@Set2Id int,
	@Set2Name nvarchar(255),
	@List21Id int,
	@List22Id int,
	@List23Id int,
	@List24Id int,
	@List25Id int,
	@List26Id int,
	@List27Id int,
	@Category21Id int,
	@Category22Id int,
	@Category23Id int,
	@Category24Id int,
	@Category25Id int,
	@Category26Id int,
	@Category27Id int,
	@Category28Id int,
	@Category29Id int,
	@Category2AId int,
	@Category2BId int,
	@Category2CId int,
	@Category2DId int,
	@Category2EId int


SET	@Set2Name = 'Changing Lanes Check Lists'

DELETE FROM [CheckList].[TemplateAction] Where CategoryId IN (Select tl.ListId From TemplateSet ts INNER JOIN TemplateList tl on ts.SetId = tl.SetId INNER JOIN TemplateCategory tc on tc.ListId = tl.ListId Where SetName = @Set2Name)
DELETE FROM [CheckList].[TemplateCategory] Where ListId IN (Select tl.ListId From TemplateSet ts INNER JOIN TemplateList tl on ts.SetId = tl.SetId Where SetName = @Set2Name)
DELETE FROM [CheckList].[TemplateList] Where SetId IN (Select SetId From TemplateSet ts Where SetName = @Set2Name)
DELETE FROM [CheckList].[TemplateSet] Where SetName = @Set2Name


Insert into [CheckList].[TemplateSet](SetName, SetDscr, OwnerName) Values (@Set2Name, '', @OwnerName)
	SET @Set2Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-24', 'These can be done casually any time the day before', 10, @Set2Id)
	SET @List22Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List22Id)
	SET @Category2BId = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-12', 'Do these the night before departure, usually just before bed', 20, @Set2Id)
	SET @List23Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List23Id)
	SET @Category2CId = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-0', 'Do these the morning of departure', 30, @Set2Id)
	SET @List24Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List24Id)
	SET @Category2DId = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('Hitching (time to roll!)', 
'This is BY FAR the most important list. Missing steps here can cause severe damage to the rig and / or truck, and possibly even injuries.  There are some redundancies on this list, but it''s good to double check some things from the outside.', 40, @Set2Id)
	SET @List25Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List25Id)
	SET @Category2EId = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Dump / Flush REAR black tank', null, @Category2BId, 10),
  ('Secure REAR Dump Hose', 'Less stuff to stow on departure day.  Has tube under RV to store them)', @Category2BId, 20),
  ('TREAT REAR Black Tank', null, @Category2BId, 30),
  ('Dump / Flush FRONT black tank', 'We leave this hooked up as it''s also the dump for Grey 1 and Grey 2, the hose is under the slide anyway)', @Category2BId, 40),
  ('TREAT FRONT black tank.', null, @Category2BId, 50),
  ('Fuel Truck', 'Much simpler to fuel without the rig attached, and usually cheaper', @Category2BId, 60),
  ('Inspect HITCH bolts and connectors, including hitch head retaining pins', null, @Category2BId, 70),
  ('Inspect Rig wheels and suspension', 'Torque lug nuts once a month - just a general visual inspection of the leaf springs, attachment points, etc. looking for anything amiss)', @Category2BId, 80),
  ('Sweep slide toppers and inspect roof.', 'While slide toppers really eliminate this need, I still like to get stuff off them that may have accumulated during our stay)', @Category2BId, 90),
  ('Fill fresh tank (% based on next location)', '(usually just 1/3 tank unless we''ll be boondocking', @Category2BId, 100),
  ('Check Rig tires (inflation, general inspection)', 'A TPMS / Tire Pressure Monitoring System makes this a simple task.  I always make sure the tires are +/- 3psi of target of 110psi', @Category2BId, 110),
  ('Check Truck tires (inflation, general inspection)', 'A truck TPMS also greatly simplifies this', @Category2BId, 120),
  ('Check Pin Box Airbag pressure (100psi)', 'Our FlexAir pin box has a shock and airbag.  I''ve found that 100psi cold with no load puts it right about where it''s supposed to ride when loaded', @Category2BId, 130),
  ('Check Radios / charge if needed (we use these)', null, @Category2BId, 140)

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Secure outside items.', 'Rug, chairs, etc', @Category2CId, 10),
  ('Plan route and fuel', 'Get a feel on gas station locations, rest stops, etc.  I also text the next address to myself so I have it in the truck for the GPS in the morning', @Category2CId, 20),
  ('Garage: rear bathroom vent closed and lights off', null, @Category2CId, 30),
  ('Garage: rear bathroom secure and door closed', null, @Category2CId, 40),
  ('Garage: roll up carpet', 'This does require some jugging of the furniture', @Category2CId, 50),
  ('Garage: desk in stow position', 'up against the bathroom door but not tied down yet as I''ll likely still need it', @Category2CId, 60),
  ('Garage: wheel-dock in place and tie downs ready', 'simply on the floor where they will go for travel', @Category2CId, 70),
  ('Garage: Happpijack fully stowed / confirm items strapped to bunk are secure.', null, @Category2CId, 80),
  ('Load Lucile (if possible) and strap down loosely', 'No sense compressing the shocks longer than necessary', @Category2CId, 90),
  ('Secure patio', 'Collapse patio rails and close rear door', @Category2CId, 100),
  ('Dump grey tanks and geo-treat (AFTER showers, dishes, etc)', 'I like to put about 5 gallons of water in both grey tanks along with a cup of water softener and bit of dawn dish soap.  This concoction will splash around in there while we drive and clean the tanks and sensors', @Category2CId, 110)

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Usually Tara starts in the front (bedroom), and I start in the rear (garage), and we meet in the middle.', null, @Category2DId, 10),
  ('Run generator (exercise) while prepping to leave (if etiquette allows)', 'when we can, we switch over to genny power to run the A/C, etc for the last 30 mins to hour to exercise it', @Category2DId, 20),
  ('Bedroom secure and carpet stowed', 'closet door latched, laundry door closed', @Category2DId, 30),
  ('Bedroom windows closed', null, @Category2DId, 40),
  ('Bedroom slide in', null, @Category2DId, 50),
  ('Bedroom A/C and lights off', null, @Category2DId, 60),
  ('Bathroom shower secure', 'everything off shelves, shower door locked open)', @Category2DId, 70),
  ('Bathroom secure', 'vent closed, counter items stowed, bathroom door latched open', @Category2DId, 80),
  ('Hall window blind up.', null, @Category2DId, 90),
  ('Garage: Desk secured', 'NAS disconnected and stowed, monitor in travel mode, desk and chair strapped down)', @Category2DId, 100),
  ('Garage: secure ', 'blinds up, patio doors latched, garage door locked, rolled rug on floor, tv locked in, happi-jack all the way up)', @Category2DId, 110),
  ('Garage: Motorcycle straps tightened down', null, @Category2DId, 120),
  ('Garage: Rear A/C and lights off', null, @Category2DId, 130),
  ('Living room secure', 'refrigerator doors shut and latched, carpets stowed, chairs secured with padding, all drawers and cabinets closed)', @Category2DId, 140),
  ('Pantry door closed', null, @Category2DId, 150),
  ('Daisy''s food and water bowls put away', null, @Category2DId, 160),
  ('Windows closed / blinds up', null, @Category2DId, 170),
  ('Middle A/C off', null, @Category2DId, 180),
  ('One-Control Items off', 'water pump, water heater, ALL Lights)', @Category2DId, 190),
  ('Generator off', null, @Category2DId, 200),
  ('Inverter off', null, @Category2DId, 210),
  ('Refrigerator switched to gas (auto)', null, @Category2DId, 220),
  ('Slides: one last check for obstacles inside and out', null, @Category2DId, 230),
  ('Slides: Retract with door, vent, or window open for air flow', null, @Category2DId, 240),
  ('Put Spare key in truck', 'We don''t like our home''s keys in the truck when camped', @Category2DId, 250),
  ('Secure main entrance', 'stow steps, lock door, secure hand hold)', @Category2DId, 260),
  ('Secure Electrical and plumbing', 'water hose, power cord, poop hose)', @Category2DId, 270),
  ('Nautilus in dry camp mode', 'relieve system pressure before switching)', @Category2DId, 280)

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('T-24, T-12, T-0 [CheckList].[TemplateList]s Complete', null, @Category2EId, 10),
  ('RIG - Slides and awnings IN', null, @Category2EId, 20),
  ('RIG - Stairs and hand rail STOWED.', null, @Category2EId, 30),
  ('RIG - Forward Bay Closed and Latched', null, @Category2EId, 40),
  ('RIG - Pin Lock Removed', 'we use this pin lock - yes we''re paranoid', @Category2EId, 50),
  ('RIG - Jacks / Stabilizers to TOW HEIGHT ', 'Middle and Rear Retracted', @Category2EId, 60),
  ('TRUCK - TPMS ON', null, @Category2EId, 70),
  ('TRUCK - Tailgate DOWN', null, @Category2EId, 80),
  ('TRUCK - Back ALMOST to pin-box', null, @Category2EId, 90),
  ('RIG - Adjust Kingpin height to proper hitch height of truck.', null, @Category2EId, 100),
  ('TRUCK - OPEN Hitch Latch', null, @Category2EId, 110),
  ('TRUCK - Back truck into Kingpin', null, @Category2EId, 120),
  ('TRUCK - Visually check KINGPIN LOCK BAR IS LOCKED', null, @Category2EId, 130),
  ('TRUCK - Connect Electrical cord and Breakaway cable.', null, @Category2EId, 140),
  ('RIG - Lower Rig with Front landing gear to pull test height', 'front jacks 1? off the ground', @Category2EId, 150),
  ('TRUCK - Tow/Haul Mode', null, @Category2EId, 160),
  ('TRUCK - Mirrors EXTENDED', null, @Category2EId, 170),
  ('TRUCK - PULL TEST', 'manually engage trailer brakes and try to pull away SLOW', @Category2EId, 180),
  ('TRUCK - Check that trailer brakes are adjusted and trailer is connected in ford system.', null, @Category2EId, 190),
  ('TRUCK - Set parking brake.', null, @Category2EId, 200),
  ('RIG - Retract ALL on landing gear', null, @Category2EId, 210),
  ('TRUCK - CLOSE TAILGATE', null, @Category2EId, 220),
  ('RIG - Remove and stow wheel chocks and landing gear blocks.', null, @Category2EId, 230),
  ('RIG - Check and close all storage doors and lock.', null, @Category2EId, 240),
  ('TRUCK - Lights on / Rear Camera working', null, @Category2EId, 250),
  ('TRUCK - Validate TPMS reads all 6 tires and validate pressure / temperature', null, @Category2EId, 260),
  ('TRUCK / RIG - Verify all lights are operable (signals, brake, reverse)', null, @Category2EId, 270),
  ('TRUCK / RIG - Full Walkaround', null, @Category2EId, 280),
  ('TRUCK - Rest Trip Meter', null, @Category2EId, 290),
  ('GO!', null, @Category2EId, 300)


 Select s.SetId, s.SetName, l.ListId, l.ListName, l.ListDscr, c.CategoryId, c.CategoryText, a.ActionId, a.ActionText, a.ActionDscr
 From [CheckList].[TemplateSet] s
 INNER JOIN [CheckList].[TemplateList] l on s.SetId = l.SetId
 INNER JOIN [CheckList].[TemplateCategory] c on c.ListId = l.ListId
 INNER JOIN [CheckList].[TemplateAction] a on a.CategoryId = c.CategoryId
 WHERE s.SetId = @Set2Id 
