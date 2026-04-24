Declare @RemovePreviousData varchar(1) = 'Y'

DECLARE 
  @OwnerName nvarchar(50) = 'lyle@luppes.com'
  
-- ----------------------------------------------------------------------
IF @RemovePreviousData = 'Y'
-- ----------------------------------------------------------------------
BEGIN
  PRINT ''
  PRINT 'Removing previous set of data...'
  DELETE FROM [CheckList].[CheckSet]
  DELETE FROM [CheckList].[TemplateSet]
  DBCC CHECKIDENT('[CheckList].[CheckAction]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[CheckCategory]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[CheckList]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[CheckSet]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[TemplateAction]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[TemplateCategory]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[TemplateList]', RESEED, 0)
  DBCC CHECKIDENT('[CheckList].[TemplateSet]', RESEED, 0)
END

-- ----------------------------------------------------------------------
DECLARE 
	@Set1Id int,
	@Set1Name nvarchar(255),
	@List1Id int,
	@List2Id int,
	@List3Id int,
	@List4Id int,
	@List5Id int,
	@List6Id int,
	@List7Id int,
	@Category1Id int,
	@Category2Id int,
	@Category3Id int,
	@Category4Id int,
	@Category5Id int,
	@Category6Id int,
	@Category7Id int

SET @Set1Name = 'Lyle''s Check Lists'

DELETE FROM [CheckList].[TemplateAction] Where CategoryId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId INNER JOIN [CheckList].[TemplateCategory] tc on tc.ListId = tl.ListId Where SetName = @Set1Name)
DELETE FROM [CheckList].[TemplateCategory] Where ListId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId Where SetName = @Set1Name)
DELETE FROM [CheckList].[TemplateList] Where SetId IN (Select SetId FROM [CheckList].[TemplateSet] ts Where SetName = @Set1Name)
DELETE FROM [CheckList].[TemplateSet] Where SetName = @Set1Name

Insert into [CheckList].[TemplateSet] (SetName, SetDscr, OwnerName) Values (@Set1Name, '', 'LLUPPES')
	SET @Set1Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('1-Trip Prep', 'These should be done before you leave home', 10, @Set1Id)
	SET @List1Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List1Id)
	SET @Category1Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Fuel up the Truck', null, @Category1Id, 10),
  ('Inspect Hitch bolts and connectors, including hitch head retaining pins', null, @Category1Id, 20),
  ('Inspect RV wheels and suspension (torque lug nugs 1x/month)', null, @Category1Id, 30),
  ('Fill fresh tank (% based on next location, usually 1/3 unless boondocking)', null, @Category1Id, 40),
  ('Check RV tires (inflation, general inspection)', null, @Category1Id, 50),
  ('Check Truck tires (inflation, general inspection)', null, @Category1Id, 60),
  ('Top-off RV batteries', null, @Category1Id, 70),
  ('Plan route and fuel (gas station locations, rest stops, get address in phone for GPS)', null, @Category1Id, 80)

  
Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('2-Hitching', 'Do these these when you are hitching up to go somewhere', 20, @Set1Id)
	SET @List2Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List2Id)
	SET @Category2Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('TRUCK - Tailgate down', null, @Category2Id, 80),
  ('TRUCK - Back almost to pin-box', null, @Category2Id, 90),
  ('RV - Set Kingpin height to 1/2" above hitch in bed of truck', null, @Category2Id, 100),
  ('TRUCK - Pull handle on hitch out until it clicks', null, @Category2Id, 110),
  ('TRUCK - Back truck into hitch until hitch engages (you will hear a loud clunk)', null, @Category2Id, 120),
  ('TRUCK - Visually check that kingpin hitch is locked - look for white stipe', null, @Category2Id, 130),
  ('TRUCK - Insert locking pin in hitch handle', null, @Category2Id, 140),
  ('TRUCK - Connect 7 pin cable and breakaway cable to truck.', null, @Category2Id, 150),
  ('TRUCK - Close tailgate', null, @Category2Id, 160),
  ('RV - Retract Front landing gear using Leveling Control to 1" off ground for pull test', null, @Category2Id, 170),
  ('TRUCK - Put into Tow/Haul Mode', null, @Category2Id, 180),
  ('TRUCK - Pull test -- manually engage trailer brakes and try to pull away slowly', null, @Category2Id, 190),
  ('TRUCK - Set parking brake', null, @Category2Id, 200),
  ('RV - Retract all landing gear using Leveling Control and pressing Retract All', null, @Category2Id, 210),
  ('RV - Manually retract front stabilizer legs by pulling pin and lifting them', null, @Category2Id, 220),
  ('RV - Manually retract rear stabilizer legs by removing cotter pin pin and lifting them', null, @Category2Id, 230),
  ('RV - Remove and stow wheel chocks, x-chocks, and landing gear blocks.', null, @Category2Id, 240),
  ('RV - Check and close all storage doors and lock.', null, @Category2Id, 250),
  ('TRUCK - Lights on / rear camera working', null, @Category2Id, 260),
  ('TRUCK - Validate TPMS reads all tires and validate pressure / temperature', null, @Category2Id, 270),
  ('TRUCK / RV - Verify all lights are operable (turn signals, brake lights, reverse)', null, @Category2Id, 280),
  ('Do a walk-around  the trailer and vehicle and check EVERYTHING before pulling-out', null, @Category2Id, 290),
  ('TRUCK - Mirrors extended', null, @Category2Id, 300),
  ('TRUCK - Put into Tow/Haul Mode', null, @Category2Id, 310),
  ('TRUCK - Reset Trip Meter', null, @Category2Id, 320),
  ('GO!', null, @Category2Id, 330)


Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('3-Arrival', 'Do these when you arrive and unhitch', 30, @Set1Id)
	SET @List3Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List3Id)
	SET @Category3Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Back RV into site and set parking brake on truck', null, @Category3Id, 10),
  ('Verify that there is room to extend all three slides', null, @Category3Id, 20),
  ('Verify that you can reach sewer and electrical connections', null, @Category3Id, 30),
  ('Turn battery disconnect to On (red key in drivers front bin)', null, @Category3Id, 40),
  ('Level the trailer side-by-side by adding Lego-blocks under the tires and pulling the RV over them.  (Note: Auto level will compensate for up to 1 degree out of level.  you can see the level values in the One Control App -> Leveling -> Manual Control.', null, @Category3Id, 50),
  ('Chock the tires on both sides with 4 black chocks.  NOTE: This is critical before detaching hitch or trailer may roll.', null, @Category3Id, 60),
  ('Remove cotter pins on both rear stabilizer jacks and drop down to 4th hole (or same height on both sides) and put pins back in', null, @Category3Id, 70),
  ('Pull pins on front stabilizer jacks and drop them to 8" off the ground (or same on both sides)  Don''t drop too far or the auto-level sequence will fail.  If one side is much lower, put blocks under that side.', null, @Category3Id, 80),
  ('Place at least one Lego block under each stabilizer jack foot', null, @Category3Id, 90),
  ('Turn on leveling panel (in drivers front bin) by pushing up & down buttons simultaneously.', null, @Category3Id, 100),
  ('Push up button on leveling panel to lower front stabilizers to raise trailer up and take weight off truck, raising it just enough to see daylight between hitch and the 5th wheel', null, @Category3Id, 110),
  ('Remove cotter locking pin from 5th wheel hitch handle', null, @Category3Id, 120),
  ('Pull handle on 5th wheel hitch to unlock king pin (you may have to back up a smidge)', null, @Category3Id, 130),
  ('Drop tailgate', null, @Category3Id, 140),
  ('Unhook breakaway cable and 7 pin connection from rear of truck', null, @Category3Id, 150),
  ('Pull truck forward slowly until it is out from under the camper.', null, @Category3Id, 160),
  ('Raise tailgate', null, @Category3Id, 170),
  ('Stow the 7 pin connector cable and breakaway cable up in the hitch', null, @Category3Id, 180),
  ('Press Auto-Level on leveling panel and RV will start moving all 4 stabilizers until it is level.  This can take 2-3 minutes.  You can watch status and abort or fix issues using the One Control app.', null, @Category3Id, 190),
  ('Install the X-chocks between tires on both sides to stabilize RV', null, @Category3Id, 200),
  ('Turn off breaker in the Electrical Panel and attach the Surge Guard to verify the circuit is OK.  Turn off breaker.', null, @Category3Id, 210),
  ('Plug 50 amp extension cord into Surge Guard and into RV.  Turn on breaker again and Surge Guard will re-verify circuit and connect the power', null, @Category3Id, 220),
  ('Use the Control Panel to extend the slides, turn-on water heater using the inside switch (gas AND electric), and put out awning', null, @Category3Id, 230),
  ('Spray city water spigot with bleach to clean, then hook up pressure regulator, then Y adapter, then the Zero-G water supply hose, then water filter, then flexible connector', null, @Category3Id, 240),
  ('Turn on water for a minute to push the air out of the hose before connecting to RV', null, @Category3Id, 250),
  ('Connect flexible connector to inlet on RV, then connect hose and filter', null, @Category3Id, 260),
  ('Set RV to City Water', null, @Category3Id, 270),
  ('Turn on water', null, @Category3Id, 280),
  ('If using water tank (not city water), turn on the water pump', null, @Category3Id, 290),
  ('Turn on propane ', null, @Category3Id, 300),
  ('Turn on the refrigerator to Auto', null, @Category3Id, 310),
  ('Add one scoop of Happy Camper to toilet along with 5 gallons of water', null, @Category3Id, 320)


Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('4-Pre-Depart Interior', 'Do these when you are ready to leave', 40, @Set1Id)
	SET @List4Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List4Id)
	SET @Category4Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Plan route and fuel (gas stations, rest stops, send next location to phone for GPS in truck)', null, @Category4Id, 10),
  ('Wash dishes and pack away', null, @Category4Id, 20),
  ('Bedroom secure, closet doors latched, bathroom door latched open', null, @Category4Id, 30),
  ('Bedroom windows closed', null, @Category4Id, 40),
  ('Toilet - pour jug of water down toilet to loosen up any build-up during the drive back', null, @Category4Id, 50),
  ('Shower secure (everything off shelves, shower door locked closed)', null, @Category4Id, 60),
  ('Bathroom secure (vent closed, counter items stowed, main bathroom door shut)', null, @Category4Id, 70),
  ('Sweep, vacuum, and if necessary, mop floors and carpets; shake-out mats', null, @Category4Id, 80),
  ('Living room secure (carpets stowed, all drawers and cabinets closed)', null, @Category4Id, 90),
  ('Decorations off wall and stowed, coasters and table top items stowed', null, @Category4Id, 100),
  ('Refrigerator items secure', null, @Category4Id, 110),
  ('Refrigerator power selector to OFF for traveling', null, @Category4Id, 120),
  ('Refrigerator doors shut and latched', null, @Category4Id, 130),
  ('Outdoor stove range vent door closed ', null, @Category4Id, 140),
  ('Table and chairs secured with padding', null, @Category4Id, 150),
  ('Discard trash/recycling - both kitchen and bathroom', null, @Category4Id, 160),
  ('Pantry door closed', null, @Category4Id, 170),
  ('Windows closed / blinds up', null, @Category4Id, 180),
  ('Ceiling vents closed', null, @Category4Id, 190),
  ('If returning home: get laundry and clothes ready so you don''t have to open BR slide', null, @Category4Id, 200),
  ('If returning home: get items out of pantry so you don''t have to open LR slide', null, @Category4Id, 210),
  ('One-Control Items off (water pump, water heater, ALL Lights)', null, @Category4Id, 220),
  ('Purse, drinks, snacks all ready and put into truck', null, @Category4Id, 230),
  ('Final walkthrough - re-check doors, windows, surfaces', null, @Category4Id, 240),
  ('Retract awning', null, @Category4Id, 250),
  ('Slides: one last check for obstacles inside and out', null, @Category4Id, 260),
  ('Slides: Retract with door, vent, or window open for air flow', null, @Category4Id, 270)


Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('5-Pre-Depart Exterior', 'Do these when you are ready to leave', 40, @Set1Id)
	SET @List5Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List5Id)
	SET @Category5Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Tire pressure checked', null, @Category5Id, 20),
  ('Fluid levels checked', null, @Category5Id, 30),
  ('Put spare key in truck', null, @Category5Id, 40),
  ('Clear outside roof on top of all slides is free of debris', null, @Category5Id, 50),
  ('Close all roof vents', null, @Category5Id, 60),
  ('TV antenna turned to transport position', null, @Category5Id, 70),
  ('Disconnect from television cable', null, @Category5Id, 80),
  ('Replace/store amenities (e.g. sweep and roll carpets, BBQ, chairs and tables)', null, @Category5Id, 90),
  ('When connecting stinky slinky, put in ground first, then connect to RV', null, @Category5Id, 100),
  ('Dump black tank', null, @Category5Id, 110),
  ('Flush/rinse black tank by adding water thru inlet with orange hose until water runs clear', null, @Category5Id, 120),
  ('Dump grey tanks and geo-treat (keep 5 gallons in grey tanks to slosh around)', null, @Category5Id, 130),
  ('Shut both valves, WAIT a few minutes, then disconnect and drain from RV towards sewer', null, @Category5Id, 140),
  ('Rinse out dump hose (using orange hose and hose fitting)', null, @Category5Id, 150),
  ('Put away stinky slinky in rear bumper', null, @Category5Id, 160),
  ('Treat Black Tank (???)', null, @Category5Id, 170),
  ('Wash/Disinfect hands', null, @Category5Id, 180),
  ('Turn off Water pump', null, @Category5Id, 190),
  ('Disconnect from Water', null, @Category5Id, 200),
  ('Connect both ends of the potable water hose together to keep them clean', null, @Category5Id, 210),
  ('Turn off water heater/furnace/air conditioner', null, @Category5Id, 220),
  ('Secure electrical and plumbing (water hose, power cord, poop hose)', null, @Category5Id, 230),
  ('Pull in slides', null, @Category5Id, 240),
  ('Disconnect from Power', null, @Category5Id, 250),
  ('Turn off propane', null, @Category5Id, 260),
  ('Outside storage bay door secured', null, @Category5Id, 270),
  ('Inspect site for left items, litter', null, @Category5Id, 280)


Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('6-Back Home', 'Do these when you are arrive back home', 40, @Set1Id)
	SET @List6Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List6Id)
	SET @Category6Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('Move trailer to home', null, @Category6Id, 10),
  ('Do arrival [CheckList].[TemplateList] (up to the point of hookups)', null, @Category6Id, 20),
  ('Put out slides or awning if put away wet', null, @Category6Id, 30),
  ('If dry-camping, empty fresh-water tank and refill the trailer with water', null, @Category6Id, 40),
  ('Remove food from fridge and any other perishables', null, @Category6Id, 50),
  ('Launder clothing/towels/bedding and return to trailer', null, @Category6Id, 60),
  ('Confirm that the fridge and propane are turned-off', null, @Category6Id, 70),
  ('Open fridge/freezer and confirm they are off, set lever so they don''t close', null, @Category6Id, 80),
  ('Top-off batteries (if necessary)', null, @Category6Id, 90),
  ('Wipe hitch clean and cover it', null, @Category6Id, 100),
  ('Lock tools away', null, @Category6Id, 110),
  ('Turn-off propane and turn the battery disconnect switch to off (in front bin)', null, @Category6Id, 120),
  ('Confirm everything is locked', null, @Category6Id, 130),
  ('Do a vehicle walk-around to make sure everything is secured', null, @Category6Id, 140)


Insert into [CheckList].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('7-Maintenance', 'Do these periodically', 40, @Set1Id)
	SET @List7Id = SCOPE_IDENTITY()
Insert into [CheckList].[TemplateCategory](CategoryText, ListId) Values ('Main', @List7Id)
	SET @Category7Id = SCOPE_IDENTITY()

Insert into [CheckList].[TemplateAction](ActionText, ActionDscr, CategoryId, SortOrder) Values 
  ('2x/year - ¼ c Clorox for 15 gallons watter into fresh water tank (Freshen water; add and flush)', null, @Category7Id, 10),
  ('2x/year - UV retreat roof', null, @Category7Id, 20),
  ('2x/year - Vaseline toilet (prevents TP sticking)', null, @Category7Id, 30),
  ('3 year - Redo roof sealant (strip and replace ~$1000)', null, @Category7Id, 40),
  ('Frequent - Lube hitch ball', null, @Category7Id, 50),
  ('Inspect and clean roof membrane each month with one step cleaner and treatment like protectall', null, @Category7Id, 60),
  ('Slide Outs:   Keep clean and treated and lubricated.  Two seals - one on slide out and one on wall - side is only water right when fully in or out.  Put protectant spray on seals.', null, @Category7Id, 70),
  ('Axle lubricant: repack bearings and remove old grease every 12 months or 12K miles. ', null, @Category7Id, 80),
  ('Brakes: inspect and adjust every 3000 miles or annually (at least)', null, @Category7Id, 90),
  ('Clean in and UNDER your cabinet. Things will leak under the cabinet. Put a bead of silicone sealant around the cabinets to seal it up. ', null, @Category7Id, 100),
  ('AC: take cover off and clean foam filter and debris from entry. Consider adding screens over openings for A/C or water heater. ', null, @Category7Id, 110),
  ('Propane: have propane systems treated and checked annually by certified technician - pressure/leak down test. Tanks must be less than 12 years old.', null, @Category7Id, 120),
  ('Check water levels in batteries (1x/month or more) - add distilled water', null, @Category7Id, 130)


-- ----------------------------------------------------------------------
DECLARE 
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

DELETE FROM [CheckList].[TemplateAction] Where CategoryId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId INNER JOIN [CheckList].[TemplateCategory] tc on tc.ListId = tl.ListId Where SetName = @Set2Name)
DELETE FROM [CheckList].[TemplateCategory] Where ListId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId Where SetName = @Set2Name)
DELETE FROM [CheckList].[TemplateList] Where SetId IN (Select SetId FROM [CheckList].[TemplateSet] ts Where SetName = @Set2Name)
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



-- ----------------------------------------------------------------------
DECLARE 
	@Set3Name nvarchar(50),
	@Set3Id int,
	@List31Id int,
	@Category31Id int,
	@Category32Id int,
	@Category33Id int,
	@Category34Id int

SET	@Set3Name = 'RV Goddess - Setup Tasks'

DELETE FROM [CheckList].[TemplateAction] Where CategoryId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId INNER JOIN [CheckList].[TemplateCategory] tc on tc.ListId = tl.ListId Where SetName = @Set3Name)
DELETE FROM [CheckList].[TemplateCategory] Where ListId IN (Select tl.ListId FROM [CheckList].[TemplateSet] ts INNER JOIN [CheckList].[TemplateList] tl on ts.SetId = tl.SetId Where SetName = @Set3Name)
DELETE FROM [CheckList].[TemplateList] Where SetId IN (Select SetId FROM [CheckList].[TemplateSet] ts Where SetName = @Set3Name)
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



-- ----------------------------------------------------------------------

Select s.SetId, s.SetName, l.ListId, l.ListName, l.ListDscr, c.CategoryId, c.CategoryText, a.ActionId, a.ActionText, a.ActionDscr
From [CheckList].[TemplateSet] s
INNER JOIN [CheckList].[TemplateList] l on s.SetId = l.SetId
INNER JOIN [CheckList].[TemplateCategory] c on c.ListId = l.ListId
INNER JOIN [CheckList].[TemplateAction] a on a.CategoryId = c.CategoryId
