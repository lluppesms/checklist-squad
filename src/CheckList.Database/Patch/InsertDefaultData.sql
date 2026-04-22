DECLARE 
  @OwnerName nvarchar(50) = 'lyle@luppes.com',
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


DELETE FROM [dbo].[TemplateSet] Where SetName IN ('Lyle''s Check Lists', 'Changing Lanes Check Lists', 'RV Goddess - Setup Tasks')

-- ----------------------------------------------------------------------
SET @Set1Name = 'Lyle''s Check Lists'
-- ----------------------------------------------------------------------
Insert into [dbo].[TemplateSet] (SetName, OwnerName) Values (@Set1Name, 'LLUPPES')
	SET @Set1Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('1-Trip Prep', 'These should be done before you leave home', 10, @Set1Id)
	SET @List1Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List1Id)
	SET @Category1Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('Fuel up the Truck', null, @List1Id, @Category1Id, 10),
  ('Inspect Hitch bolts and connectors, including hitch head retaining pins', null, @List1Id, @Category1Id, 20),
  ('Inspect RV wheels and suspension (torque lug nugs 1x/month)', null, @List1Id, @Category1Id, 30),
  ('Fill fresh tank (% based on next location, usually 1/3 unless boondocking)', null, @List1Id, @Category1Id, 40),
  ('Check RV tires (inflation, general inspection)', null, @List1Id, @Category1Id, 50),
  ('Check Truck tires (inflation, general inspection)', null, @List1Id, @Category1Id, 60),
  ('Top-off RV batteries', null, @List1Id, @Category1Id, 70),
  ('Plan route and fuel (gas station locations, rest stops, get address in phone for GPS)', null, @List1Id, @Category1Id, 80)
  
Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('2-Hitching', 'Do these these when you are hitching up to go somewhere', 20, @Set1Id)
	SET @List2Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List2Id)
	SET @Category2Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('TRUCK - Tailgate down', null, @List1Id, @Category2Id, 80),
  ('TRUCK - Back almost to pin-box', null, @List1Id, @Category2Id, 90),
  ('RV - Set Kingpin height to 1/2" above hitch in bed of truck', null, @List1Id, @Category2Id, 100),
  ('TRUCK - Pull handle on hitch out until it clicks', null, @List1Id, @Category2Id, 110),
  ('TRUCK - Back truck into hitch until hitch engages (you will hear a loud clunk)', null, @List1Id, @Category2Id, 120),
  ('TRUCK - Visually check that kingpin hitch is locked - look for white stipe', null, @List1Id, @Category2Id, 130),
  ('TRUCK - Insert locking pin in hitch handle', null, @List1Id, @Category2Id, 140),
  ('TRUCK - Connect 7 pin cable and breakaway cable to truck.', null, @List1Id, @Category2Id, 150),
  ('TRUCK - Close tailgate', null, @List1Id, @Category2Id, 160),
  ('RV - Retract Front landing gear using Leveling Control to 1" off ground for pull test', null, @List1Id, @Category2Id, 170),
  ('TRUCK - Put into Tow/Haul Mode', null, @List1Id, @Category2Id, 180),
  ('TRUCK - Pull test -- manually engage trailer brakes and try to pull away slowly', null, @List1Id, @Category2Id, 190),
  ('TRUCK - Set parking brake', null, @List1Id, @Category2Id, 200),
  ('RV - Retract all landing gear using Leveling Control and pressing Retract All', null, @List1Id, @Category2Id, 210),
  ('RV - Manually retract front stabilizer legs by pulling pin and lifting them', null, @List1Id, @Category2Id, 220),
  ('RV - Manually retract rear stabilizer legs by removing cotter pin pin and lifting them', null, @List1Id, @Category2Id, 230),
  ('RV - Remove and stow wheel chocks, x-chocks, and landing gear blocks.', null, @List1Id, @Category2Id, 240),
  ('RV - Check and close all storage doors and lock.', null, @List1Id, @Category2Id, 250),
  ('TRUCK - Lights on / rear camera working', null, @List1Id, @Category2Id, 260),
  ('TRUCK - Validate TPMS reads all tires and validate pressure / temperature', null, @List1Id, @Category2Id, 270),
  ('TRUCK / RV - Verify all lights are operable (turn signals, brake lights, reverse)', null, @List1Id, @Category2Id, 280),
  ('Do a walk-around  the trailer and vehicle and check EVERYTHING before pulling-out', null, @List1Id, @Category2Id, 290),
  ('TRUCK - Mirrors extended', null, @List1Id, @Category2Id, 300),
  ('TRUCK - Put into Tow/Haul Mode', null, @List1Id, @Category2Id, 310),
  ('TRUCK - Reset Trip Meter', null, @List1Id, @Category2Id, 320),
  ('GO!', null, @List1Id, @Category2Id, 330)


Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('3-Arrival', 'Do these when you arrive and unhitch', 30, @Set1Id)
	SET @List3Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List3Id)
	SET @Category3Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('Back RV into site and set parking brake on truck', null, @List1Id, @Category3Id, 10),
  ('Verify that there is room to extend all three slides', null, @List1Id, @Category3Id, 20),
  ('Verify that you can reach sewer and electrical connections', null, @List1Id, @Category3Id, 30),
  ('Turn battery disconnect to On (red key in drivers front bin)', null, @List1Id, @Category3Id, 40),
  ('Level the trailer side-by-side by adding Lego-blocks under the tires and pulling the RV over them.  (Note: Auto level will compensate for up to 1 degree out of level.  you can see the level values in the One Control App -> Leveling -> Manual Control.', null, @List1Id, @Category3Id, 50),
  ('Chock the tires on both sides with 4 black chocks.  NOTE: This is critical before detaching hitch or trailer may roll.', null, @List1Id, @Category3Id, 60),
  ('Remove cotter pins on both rear stabilizer jacks and drop down to 4th hole (or same height on both sides) and put pins back in', null, @List1Id, @Category3Id, 70),
  ('Pull pins on front stabilizer jacks and drop them to 8" off the ground (or same on both sides)  Don''t drop too far or the auto-level sequence will fail.  If one side is much lower, put blocks under that side.', null, @List1Id, @Category3Id, 80),
  ('Place at least one Lego block under each stabilizer jack foot', null, @List1Id, @Category3Id, 90),
  ('Turn on leveling panel (in drivers front bin) by pushing up & down buttons simultaneously.', null, @List1Id, @Category3Id, 100),
  ('Push up button on leveling panel to lower front stabilizers to raise trailer up and take weight off truck, raising it just enough to see daylight between hitch and the 5th wheel', null, @List1Id, @Category3Id, 110),
  ('Remove cotter locking pin from 5th wheel hitch handle', null, @List1Id, @Category3Id, 120),
  ('Pull handle on 5th wheel hitch to unlock king pin (you may have to back up a smidge)', null, @List1Id, @Category3Id, 130),
  ('Drop tailgate', null, @List1Id, @Category3Id, 140),
  ('Unhook breakaway cable and 7 pin connection from rear of truck', null, @List1Id, @Category3Id, 150),
  ('Pull truck forward slowly until it is out from under the camper.', null, @List1Id, @Category3Id, 160),
  ('Raise tailgate', null, @List1Id, @Category3Id, 170),
  ('Stow the 7 pin connector cable and breakaway cable up in the hitch', null, @List1Id, @Category3Id, 180),
  ('Press Auto-Level on leveling panel and RV will start moving all 4 stabilizers until it is level.  This can take 2-3 minutes.  You can watch status and abort or fix issues using the One Control app.', null, @List1Id, @Category3Id, 190),
  ('Install the X-chocks between tires on both sides to stabilize RV', null, @List1Id, @Category3Id, 200),
  ('Turn off breaker in the Electrical Panel and attach the Surge Guard to verify the circuit is OK.  Turn off breaker.', null, @List1Id, @Category3Id, 210),
  ('Plug 50 amp extension cord into Surge Guard and into RV.  Turn on breaker again and Surge Guard will re-verify circuit and connect the power', null, @List1Id, @Category3Id, 220),
  ('Use the Control Panel to extend the slides, turn-on water heater using the inside switch (gas AND electric), and put out awning', null, @List1Id, @Category3Id, 230),
  ('Spray city water spigot with bleach to clean, then hook up pressure regulator, then Y adapter, then the Zero-G water supply hose, then water filter, then flexible connector', null, @List1Id, @Category3Id, 240),
  ('Turn on water for a minute to push the air out of the hose before connecting to RV', null, @List1Id, @Category3Id, 250),
  ('Connect flexible connector to inlet on RV, then connect hose and filter', null, @List1Id, @Category3Id, 260),
  ('Set RV to City Water', null, @List1Id, @Category3Id, 270),
  ('Turn on water', null, @List1Id, @Category3Id, 280),
  ('If using water tank (not city water), turn on the water pump', null, @List1Id, @Category3Id, 290),
  ('Turn on propane ', null, @List1Id, @Category3Id, 300),
  ('Turn on the refrigerator to Auto', null, @List1Id, @Category3Id, 310),
  ('Add one scoop of Happy Camper to toilet along with 5 gallons of water', null, @List1Id, @Category3Id, 320)


Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('4-Pre-Depart Interior', 'Do these when you are ready to leave', 40, @Set1Id)
	SET @List4Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List4Id)
	SET @Category4Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('Plan route and fuel (gas stations, rest stops, send next location to phone for GPS in truck)', null, @List1Id, @Category4Id, 10),
  ('Wash dishes and pack away', null, @List1Id, @Category4Id, 20),
  ('Bedroom secure, closet doors latched, bathroom door latched open', null, @List1Id, @Category4Id, 30),
  ('Bedroom windows closed', null, @List1Id, @Category4Id, 40),
  ('Toilet - pour jug of water down toilet to loosen up any build-up during the drive back', null, @List1Id, @Category4Id, 50),
  ('Shower secure (everything off shelves, shower door locked closed)', null, @List1Id, @Category4Id, 60),
  ('Bathroom secure (vent closed, counter items stowed, main bathroom door shut)', null, @List1Id, @Category4Id, 70),
  ('Sweep, vacuum, and if necessary, mop floors and carpets; shake-out mats', null, @List1Id, @Category4Id, 80),
  ('Living room secure (carpets stowed, all drawers and cabinets closed)', null, @List1Id, @Category4Id, 90),
  ('Decorations off wall and stowed, coasters and table top items stowed', null, @List1Id, @Category4Id, 100),
  ('Refrigerator items secure', null, @List1Id, @Category4Id, 110),
  ('Refrigerator power selector to OFF for traveling', null, @List1Id, @Category4Id, 120),
  ('Refrigerator doors shut and latched', null, @List1Id, @Category4Id, 130),
  ('Outdoor stove range vent door closed ', null, @List1Id, @Category4Id, 140),
  ('Table and chairs secured with padding', null, @List1Id, @Category4Id, 150),
  ('Discard trash/recycling - both kitchen and bathroom', null, @List1Id, @Category4Id, 160),
  ('Pantry door closed', null, @List1Id, @Category4Id, 170),
  ('Windows closed / blinds up', null, @List1Id, @Category4Id, 180),
  ('Ceiling vents closed', null, @List1Id, @Category4Id, 190),
  ('If returning home: get laundry and clothes ready so you don''t have to open BR slide', null, @List1Id, @Category4Id, 200),
  ('If returning home: get items out of pantry so you don''t have to open LR slide', null, @List1Id, @Category4Id, 210),
  ('One-Control Items off (water pump, water heater, ALL Lights)', null, @List1Id, @Category4Id, 220),
  ('Purse, drinks, snacks all ready and put into truck', null, @List1Id, @Category4Id, 230),
  ('Final walkthrough - re-check doors, windows, surfaces', null, @List1Id, @Category4Id, 240),
  ('Retract awning', null, @List1Id, @Category4Id, 250),
  ('Slides: one last check for obstacles inside and out', null, @List1Id, @Category4Id, 260),
  ('Slides: Retract with door, vent, or window open for air flow', null, @List1Id, @Category4Id, 270)


Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('5-Pre-Depart Exterior', 'Do these when you are ready to leave', 40, @Set1Id)
	SET @List5Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List5Id)
	SET @Category5Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('Tire pressure checked', null, @List1Id, @Category5Id, 20),
  ('Fluid levels checked', null, @List1Id, @Category5Id, 30),
  ('Put spare key in truck', null, @List1Id, @Category5Id, 40),
  ('Clear outside roof on top of all slides is free of debris', null, @List1Id, @Category5Id, 50),
  ('Close all roof vents', null, @List1Id, @Category5Id, 60),
  ('TV antenna turned to transport position', null, @List1Id, @Category5Id, 70),
  ('Disconnect from television cable', null, @List1Id, @Category5Id, 80),
  ('Replace/store amenities (e.g. sweep and roll carpets, BBQ, chairs and tables)', null, @List1Id, @Category5Id, 90),
  ('When connecting stinky slinky, put in ground first, then connect to RV', null, @List1Id, @Category5Id, 100),
  ('Dump black tank', null, @List1Id, @Category5Id, 110),
  ('Flush/rinse black tank by adding water thru inlet with orange hose until water runs clear', null, @List1Id, @Category5Id, 120),
  ('Dump grey tanks and geo-treat (keep 5 gallons in grey tanks to slosh around)', null, @List1Id, @Category5Id, 130),
  ('Shut both valves, WAIT a few minutes, then disconnect and drain from RV towards sewer', null, @List1Id, @Category5Id, 140),
  ('Rinse out dump hose (using orange hose and hose fitting)', null, @List1Id, @Category5Id, 150),
  ('Put away stinky slinky in rear bumper', null, @List1Id, @Category5Id, 160),
  ('Treat Black Tank (???)', null, @List1Id, @Category5Id, 170),
  ('Wash/Disinfect hands', null, @List1Id, @Category5Id, 180),
  ('Turn off Water pump', null, @List1Id, @Category5Id, 190),
  ('Disconnect from Water', null, @List1Id, @Category5Id, 200),
  ('Connect both ends of the potable water hose together to keep them clean', null, @List1Id, @Category5Id, 210),
  ('Turn off water heater/furnace/air conditioner', null, @List1Id, @Category5Id, 220),
  ('Secure electrical and plumbing (water hose, power cord, poop hose)', null, @List1Id, @Category5Id, 230),
  ('Pull in slides', null, @List1Id, @Category5Id, 240),
  ('Disconnect from Power', null, @List1Id, @Category5Id, 250),
  ('Turn off propane', null, @List1Id, @Category5Id, 260),
  ('Outside storage bay door secured', null, @List1Id, @Category5Id, 270),
  ('Inspect site for left items, litter', null, @List1Id, @Category5Id, 280)


Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('6-Back Home', 'Do these when you are arrive back home', 40, @Set1Id)
	SET @List6Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List6Id)
	SET @Category6Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('Move trailer to home', null, @List1Id, @Category6Id, 10),
  ('Do arrival [dbo].[TemplateList] (up to the point of hookups)', null, @List1Id, @Category6Id, 20),
  ('Put out slides or awning if put away wet', null, @List1Id, @Category6Id, 30),
  ('If dry-camping, empty fresh-water tank and refill the trailer with water', null, @List1Id, @Category6Id, 40),
  ('Remove food from fridge and any other perishables', null, @List1Id, @Category6Id, 50),
  ('Launder clothing/towels/bedding and return to trailer', null, @List1Id, @Category6Id, 60),
  ('Confirm that the fridge and propane are turned-off', null, @List1Id, @Category6Id, 70),
  ('Open fridge/freezer and confirm they are off, set lever so they don''t close', null, @List1Id, @Category6Id, 80),
  ('Top-off batteries (if necessary)', null, @List1Id, @Category6Id, 90),
  ('Wipe hitch clean and cover it', null, @List1Id, @Category6Id, 100),
  ('Lock tools away', null, @List1Id, @Category6Id, 110),
  ('Turn-off propane and turn the battery disconnect switch to off (in front bin)', null, @List1Id, @Category6Id, 120),
  ('Confirm everything is locked', null, @List1Id, @Category6Id, 130),
  ('Do a vehicle walk-around to make sure everything is secured', null, @List1Id, @Category6Id, 140)


Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('7-Maintenance', 'Do these periodically', 40, @Set1Id)
	SET @List7Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List7Id)
	SET @Category7Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr, ListId, CategoryId, SortOrder) Values 
  ('2x/year - ¼ c Clorox for 15 gallons watter into fresh water tank (Freshen water; add and flush)', null, @List1Id, @Category7Id, 10),
  ('2x/year - UV retreat roof', null, @List1Id, @Category7Id, 20),
  ('2x/year - Vaseline toilet (prevents TP sticking)', null, @List1Id, @Category7Id, 30),
  ('3 year - Redo roof sealant (strip and replace ~$1000)', null, @List1Id, @Category7Id, 40),
  ('Frequent - Lube hitch ball', null, @List1Id, @Category7Id, 50),
  ('Inspect and clean roof membrane each month with one step cleaner and treatment like protectall', null, @List1Id, @Category7Id, 60),
  ('Slide Outs:   Keep clean and treated and lubricated.  Two seals - one on slide out and one on wall - side is only water right when fully in or out.  Put protectant spray on seals.', null, @List1Id, @Category7Id, 70),
  ('Axle lubricant: repack bearings and remove old grease every 12 months or 12K miles. ', null, @List1Id, @Category7Id, 80),
  ('Brakes: inspect and adjust every 3000 miles or annually (at least)', null, @List1Id, @Category7Id, 90),
  ('Clean in and UNDER your cabinet. Things will leak under the cabinet. Put a bead of silicone sealant around the cabinets to seal it up. ', null, @List1Id, @Category7Id, 100),
  ('AC: take cover off and clean foam filter and debris from entry. Consider adding screens over openings for A/C or water heater. ', null, @List1Id, @Category7Id, 110),
  ('Propane: have propane systems treated and checked annually by certified technician - pressure/leak down test. Tanks must be less than 12 years old.', null, @List1Id, @Category7Id, 120),
  ('Check water levels in batteries (1x/month or more) - add distilled water', null, @List1Id, @Category7Id, 130)


-- ----------------------------------------------------------------------
SET	@SetName = 'Changing Lanes Check Lists'
-- ----------------------------------------------------------------------
Insert into [dbo].[TemplateList](SetName, OwnerName) Values (@SetName, @OwnerName)
	SET @Set2Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-24', 'These can be done casually any time the day before', 10, @Set2Id)
	SET @List2Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List2Id)
	SET @Category11Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-12', 'Do these the night before departure, usually just before bed', 20, @Set2Id)
	SET @List3Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List3Id)
	SET @Category12Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('T-0', 'Do these the morning of departure', 30, @Set2Id)
	SET @List4Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List4Id)
	SET @Category13Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateList](ListName, ListDscr, SortOrder, SetId) Values ('Hitching (time to roll!)', 
'This is BY FAR the most important list. Missing steps here can cause severe damage to the rig and / or truck, and possibly even injuries.  There are some redundancies on this list, but it''s good to double check some things from the outside.', 40, @Set2Id)
	SET @List5Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Main', @List5Id)
	SET @Category14Id = SCOPE_IDENTITY()

Insert into [dbo].[TemplateAction](ActionText, ActionDscr,  CategoryId, SortOrder, ListId) Values 
  ('Dump / Flush REAR black tank', null, @Category11Id, 10, @List2Id),
  ('Secure REAR Dump Hose', 'Less stuff to stow on departure day.  Has tube under RV to store them)', @Category11Id, 20, @List2Id),
  ('TREAT REAR Black Tank', null, @Category11Id, 30, @List2Id),
  ('Dump / Flush FRONT black tank', 'We leave this hooked up as it''s also the dump for Grey 1 and Grey 2, the hose is under the slide anyway)', @Category11Id, 40, @List2Id),
  ('TREAT FRONT black tank.', null, @Category11Id, 50, @List2Id),
  ('Fuel Truck', 'Much simpler to fuel without the rig attached, and usually cheaper', @Category11Id, 60, @List2Id),
  ('Inspect HITCH bolts and connectors, including hitch head retaining pins', null, @Category11Id, 70, @List2Id),
  ('Inspect Rig wheels and suspension', 'Torque lug nuts once a month - just a general visual inspection of the leaf springs, attachment points, etc. looking for anything amiss)', @Category11Id, 80, @List2Id),
  ('Sweep slide toppers and inspect roof.', 'While slide toppers really eliminate this need, I still like to get stuff off them that may have accumulated during our stay)', @Category11Id, 90, @List2Id),
  ('Fill fresh tank (% based on next location)', '(usually just 1/3 tank unless we''ll be boondocking', @Category11Id, 100, @List2Id),
  ('Check Rig tires (inflation, general inspection)', 'A TPMS / Tire Pressure Monitoring System makes this a simple task.  I always make sure the tires are +/- 3psi of target of 110psi', @Category11Id, 110, @List2Id),
  ('Check Truck tires (inflation, general inspection)', 'A truck TPMS also greatly simplifies this', @Category11Id, 120, @List2Id),
  ('Check Pin Box Airbag pressure (100psi)', 'Our FlexAir pin box has a shock and airbag.  I''ve found that 100psi cold with no load puts it right about where it''s supposed to ride when loaded', @Category11Id, 130, @List2Id),
  ('Check Radios / charge if needed (we use these)', null, @Category11Id, 140, @List2Id)

Insert into [dbo].[TemplateAction](ActionText, ActionDscr,  CategoryId, SortOrder, ListId) Values 
  ('Secure outside items.', 'Rug, chairs, etc', @Category12Id, 10, @List3Id),
  ('Plan route and fuel', 'Get a feel on gas station locations, rest stops, etc.  I also text the next address to myself so I have it in the truck for the GPS in the morning', @Category12Id, 20, @List3Id),
  ('Garage: rear bathroom vent closed and lights off', null, @Category12Id, 30, @List3Id),
  ('Garage: rear bathroom secure and door closed', null, @Category12Id, 40, @List3Id),
  ('Garage: roll up carpet', 'This does require some jugging of the furniture', @Category12Id, 50, @List3Id),
  ('Garage: desk in stow position', 'up against the bathroom door but not tied down yet as I''ll likely still need it', @Category12Id, 60, @List3Id),
  ('Garage: wheel-dock in place and tie downs ready', 'simply on the floor where they will go for travel', @Category12Id, 70, @List3Id),
  ('Garage: Happpijack fully stowed / confirm items strapped to bunk are secure.', null, @Category12Id, 80, @List3Id),
  ('Load Lucile (if possible) and strap down loosely', 'No sense compressing the shocks longer than necessary', @Category12Id, 90, @List3Id),
  ('Secure patio', 'Collapse patio rails and close rear door', @Category12Id, 100, @List3Id),
  ('Dump grey tanks and geo-treat (AFTER showers, dishes, etc)', 'I like to put about 5 gallons of water in both grey tanks along with a cup of water softener and bit of dawn dish soap.  This concoction will splash around in there while we drive and clean the tanks and sensors', @Category12Id, 110, @List3Id)

Insert into [dbo].[TemplateAction](ActionText, ActionDscr,  CategoryId, SortOrder, ListId) Values 
  ('Usually Tara starts in the front (bedroom), and I start in the rear (garage), and we meet in the middle.', null, @Category13Id, 10, @List4Id),
  ('Run generator (exercise) while prepping to leave (if etiquette allows)', 'when we can, we switch over to genny power to run the A/C, etc for the last 30 mins to hour to exercise it', @Category13Id, 20, @List4Id),
  ('Bedroom secure and carpet stowed', 'closet door latched, laundry door closed', @Category13Id, 30, @List4Id),
  ('Bedroom windows closed', null, @Category13Id, 40, @List4Id),
  ('Bedroom slide in', null, @Category13Id, 50, @List4Id),
  ('Bedroom A/C and lights off', null, @Category13Id, 60, @List4Id),
  ('Bathroom shower secure', 'everything off shelves, shower door locked open)', @Category13Id, 70, @List4Id),
  ('Bathroom secure', 'vent closed, counter items stowed, bathroom door latched open', @Category13Id, 80, @List4Id),
  ('Hall window blind up.', null, @Category13Id, 90, @List4Id),
  ('Garage: Desk secured', 'NAS disconnected and stowed, monitor in travel mode, desk and chair strapped down)', @Category13Id, 100, @List4Id),
  ('Garage: secure ', 'blinds up, patio doors latched, garage door locked, rolled rug on floor, tv locked in, happi-jack all the way up)', @Category13Id, 110, @List4Id),
  ('Garage: Motorcycle straps tightened down', null, @Category13Id, 120, @List4Id),
  ('Garage: Rear A/C and lights off', null, @Category13Id, 130, @List4Id),
  ('Living room secure', 'refrigerator doors shut and latched, carpets stowed, chairs secured with padding, all drawers and cabinets closed)', @Category13Id, 140, @List4Id),
  ('Pantry door closed', null, @Category13Id, 150, @List4Id),
  ('Daisy''s food and water bowls put away', null, @Category13Id, 160, @List4Id),
  ('Windows closed / blinds up', null, @Category13Id, 170, @List4Id),
  ('Middle A/C off', null, @Category13Id, 180, @List4Id),
  ('One-Control Items off', 'water pump, water heater, ALL Lights)', @Category13Id, 190, @List4Id),
  ('Generator off', null, @Category13Id, 200, @List4Id),
  ('Inverter off', null, @Category13Id, 210, @List4Id),
  ('Refrigerator switched to gas (auto)', null, @Category13Id, 220, @List4Id),
  ('Slides: one last check for obstacles inside and out', null, @Category13Id, 230, @List4Id),
  ('Slides: Retract with door, vent, or window open for air flow', null, @Category13Id, 240, @List4Id),
  ('Put Spare key in truck', 'We don''t like our home''s keys in the truck when camped', @Category13Id, 250, @List4Id),
  ('Secure main entrance', 'stow steps, lock door, secure hand hold)', @Category13Id, 260, @List4Id),
  ('Secure Electrical and plumbing', 'water hose, power cord, poop hose)', @Category13Id, 270, @List4Id),
  ('Nautilus in dry camp mode', 'relieve system pressure before switching)', @Category13Id, 280, @List4Id)

Insert into [dbo].[TemplateAction](ActionText, ActionDscr,  CategoryId, SortOrder, ListId) Values 
  ('T-24, T-12, T-0 [dbo].[TemplateList]s Complete', null, @Category14Id, 10, @List5Id),
  ('RIG - Slides and awnings IN', null, @Category14Id, 20, @List5Id),
  ('RIG - Stairs and hand rail STOWED.', null, @Category14Id, 30, @List5Id),
  ('RIG - Forward Bay Closed and Latched', null, @Category14Id, 40, @List5Id),
  ('RIG - Pin Lock Removed', 'we use this pin lock - yes we''re paranoid', @Category14Id, 50, @List5Id),
  ('RIG - Jacks / Stabilizers to TOW HEIGHT ', 'Middle and Rear Retracted', @Category14Id, 60, @List5Id),
  ('TRUCK - TPMS ON', null, @Category14Id, 70, @List5Id),
  ('TRUCK - Tailgate DOWN', null, @Category14Id, 80, @List5Id),
  ('TRUCK - Back ALMOST to pin-box', null, @Category14Id, 90, @List5Id),
  ('RIG - Adjust Kingpin height to proper hitch height of truck.', null, @Category14Id, 100, @List5Id),
  ('TRUCK - OPEN Hitch Latch', null, @Category14Id, 110, @List5Id),
  ('TRUCK - Back truck into Kingpin', null, @Category14Id, 120, @List5Id),
  ('TRUCK - Visually check KINGPIN LOCK BAR IS LOCKED', null, @Category14Id, 130, @List5Id),
  ('TRUCK - Connect Electrical cord and Breakaway cable.', null, @Category14Id, 140, @List5Id),
  ('RIG - Lower Rig with Front landing gear to pull test height', 'front jacks 1? off the ground', @Category14Id, 150, @List5Id),
  ('TRUCK - Tow/Haul Mode', null, @Category14Id, 160, @List5Id),
  ('TRUCK - Mirrors EXTENDED', null, @Category14Id, 170, @List5Id),
  ('TRUCK - PULL TEST', 'manually engage trailer brakes and try to pull away SLOW', @Category14Id, 180, @List5Id),
  ('TRUCK - Check that trailer brakes are adjusted and trailer is connected in ford system.', null, @Category14Id, 190, @List5Id),
  ('TRUCK - Set parking brake.', null, @Category14Id, 200, @List5Id),
  ('RIG - Retract ALL on landing gear', null, @Category14Id, 210, @List5Id),
  ('TRUCK - CLOSE TAILGATE', null, @Category14Id, 220, @List5Id),
  ('RIG - Remove and stow wheel chocks and landing gear blocks.', null, @Category14Id, 230, @List5Id),
  ('RIG - Check and close all storage doors and lock.', null, @Category14Id, 240, @List5Id),
  ('TRUCK - Lights on / Rear Camera working', null, @Category14Id, 250, @List5Id),
  ('TRUCK - Validate TPMS reads all 6 tires and validate pressure / temperature', null, @Category14Id, 260, @List5Id),
  ('TRUCK / RIG - Verify all lights are operable (signals, brake, reverse)', null, @Category14Id, 270, @List5Id),
  ('TRUCK / RIG - Full Walkaround', null, @Category14Id, 280, @List5Id),
  ('TRUCK - Rest Trip Meter', null, @Category14Id, 290, @List5Id),
  ('GO!', null, @Category14Id, 300, @List5Id)

-- ----------------------------------------------------------------------
SET	@SetName = 'RV Goddess - Setup Tasks'
-- ----------------------------------------------------------------------
Insert into [dbo].[TemplateSet](SetName, OwnerName) Values (@SetName, @OwnerName)
	SET @Set1Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateList](ListName, SetId) Values ('Departure List', @Set1Id)
	SET @List1Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Interior', @List1Id)
	SET @Category1Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Passenger Area', @List1Id)
	SET @Category2Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('Exterior', @List1Id)
	SET @Category3Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateCategory](CategoryText, ListId) Values ('At Departure', @List1Id)
	SET @Category4Id = SCOPE_IDENTITY()
Insert into [dbo].[TemplateAction](ActionText, CategoryId, SortOrder, ListId) Values 
	('Items stored', @Category1Id, 10, @List1Id),
	('Cupboard doors closed', @Category1Id, 20, @List1Id),
	('Ceiling vents closed', @Category1Id, 30, @List1Id),
	('Windows closed', @Category1Id, 40, @List1Id),
	('TV/satellite dish down', @Category1Id, 50, @List1Id),
	('TV antenna down', @Category1Id, 60, @List1Id),
	('Refrigerator items secure', @Category1Id, 70, @List1Id),
	('Refrigerator power selected', @Category1Id, 80, @List1Id),
	('Water pump turned off', @Category1Id, 90, @List1Id),
	('Slide rooms in and secured', @Category1Id, 100, @List1Id),
	('Interior doors secured', @Category1Id, 110, @List1Id),
	('Turn off water heater/furnace/air conditioner', @Category1Id, 120, @List1Id),
	('Discard trash/recycling', @Category1Id, 130, @List1Id),

	('Drink/coffee cups filled', @Category2Id, 10, @List1Id),
	('Seat belts located', @Category2Id, 20, @List1Id),
	('Note mileage in Camping Journal', @Category2Id, 30, @List1Id),
	('Music/radio ready', @Category2Id, 40, @List1Id),
	('Maps/Route/GPS ready', @Category2Id, 50, @List1Id),
	('Kids games/books/music/videos/snacks ready', @Category2Id, 60, @List1Id),
	('Cell phone', @Category2Id, 70, @List1Id),
	('All keys accounted for', @Category2Id, 80, @List1Id),
	('Sunglasses', @Category2Id, 90, @List1Id),
	('Tissues/wet wipes', @Category2Id, 100, @List1Id),
	('Pets walked & happy', @Category2Id, 110, @List1Id),

	('Drain holding tanks', @Category3Id, 10, @List1Id),
	('Check fresh water tank level', @Category3Id, 20, @List1Id),
	('Power, water, sewer, cable, etc. unplugged from RV', @Category3Id, 30, @List1Id),
	('Propane', @Category3Id, 40, @List1Id),
	('Range vent secured', @Category3Id, 50, @List1Id),
	('Outside storage bay door secured/locked', @Category3Id, 60, @List1Id),
	('Raise leveling jacks', @Category3Id, 70, @List1Id),
	('Remove wheel blocks', @Category3Id, 80, @List1Id),
	('Windshields cleaned', @Category3Id, 90, @List1Id),
	('Tire pressure checked', @Category3Id, 100, @List1Id),
	('Fluid levels checked', @Category3Id, 110, @List1Id),
	('Entry steps stowed', @Category3Id, 120, @List1Id),

	('Safety inspection - turn signals, brakes, headlights', @Category4Id, 10, @List1Id),
	('RV walk-around: windows, slide rooms, roof antennas/vents, steps', @Category4Id, 20, @List1Id),
	('Tow vehicle walk-around, hitch inspection, safety inspection', @Category4Id, 30, @List1Id),
	('Inspect site for left items, litter', @Category4Id, 40, @List1Id)
