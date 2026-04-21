/*
Post-Deployment Script
Seed template data: Lyle's RV Check Lists (7 lists, ~150 actions)
*/

DECLARE
    @Set1Id INT,
    @Set1Name NVARCHAR(255),
    @List1Id INT,
    @List2Id INT,
    @List3Id INT,
    @List4Id INT,
    @List5Id INT,
    @List6Id INT,
    @List7Id INT,
    @Category1Id INT,
    @Category2Id INT,
    @Category3Id INT,
    @Category4Id INT,
    @Category5Id INT,
    @Category6Id INT,
    @Category7Id INT

SET @Set1Name = N'Lyle''s Check Lists'

-- Only seed if not already present
IF NOT EXISTS (SELECT 1 FROM [dbo].[TemplateSet] WHERE [SetName] = @Set1Name)
BEGIN
    INSERT INTO [dbo].[TemplateSet] ([SetName], [SetDscr], [OwnerName]) VALUES (@Set1Name, N'RV travel checklists', N'LLUPPES')
    SET @Set1Id = SCOPE_IDENTITY()

    -- List 1: Trip Prep
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'1-Trip Prep', N'These should be done before you leave home', 10, @Set1Id)
    SET @List1Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List1Id)
    SET @Category1Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'Fuel up the Truck', NULL, @Category1Id, 10),
      (N'Inspect Hitch bolts and connectors, including hitch head retaining pins', NULL, @Category1Id, 20),
      (N'Inspect RV wheels and suspension (torque lug nugs 1x/month)', NULL, @Category1Id, 30),
      (N'Fill fresh tank (% based on next location, usually 1/3 unless boondocking)', NULL, @Category1Id, 40),
      (N'Check RV tires (inflation, general inspection)', NULL, @Category1Id, 50),
      (N'Check Truck tires (inflation, general inspection)', NULL, @Category1Id, 60),
      (N'Top-off RV batteries', NULL, @Category1Id, 70),
      (N'Plan route and fuel (gas station locations, rest stops, get address in phone for GPS)', NULL, @Category1Id, 80)

    -- List 2: Hitching
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'2-Hitching', N'Do these these when you are hitching up to go somewhere', 20, @Set1Id)
    SET @List2Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List2Id)
    SET @Category2Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'TRUCK - Tailgate down', NULL, @Category2Id, 80),
      (N'TRUCK - Back almost to pin-box', NULL, @Category2Id, 90),
      (N'RV - Set Kingpin height to 1/2" above hitch in bed of truck', NULL, @Category2Id, 100),
      (N'TRUCK - Pull handle on hitch out until it clicks', NULL, @Category2Id, 110),
      (N'TRUCK - Back truck into hitch until hitch engages (you will hear a loud clunk)', NULL, @Category2Id, 120),
      (N'TRUCK - Visually check that kingpin hitch is locked - look for white stipe', NULL, @Category2Id, 130),
      (N'TRUCK - Insert locking pin in hitch handle', NULL, @Category2Id, 140),
      (N'TRUCK - Connect 7 pin cable and breakaway cable to truck.', NULL, @Category2Id, 150),
      (N'TRUCK - Close tailgate', NULL, @Category2Id, 160),
      (N'RV - Retract Front landing gear using Leveling Control to 1" off ground for pull test', NULL, @Category2Id, 170),
      (N'TRUCK - Put into Tow/Haul Mode', NULL, @Category2Id, 180),
      (N'TRUCK - Pull test -- manually engage trailer brakes and try to pull away slowly', NULL, @Category2Id, 190),
      (N'TRUCK - Set parking brake', NULL, @Category2Id, 200),
      (N'RV - Retract all landing gear using Leveling Control and pressing Retract All', NULL, @Category2Id, 210),
      (N'RV - Manually retract front stabilizer legs by pulling pin and lifting them', NULL, @Category2Id, 220),
      (N'RV - Manually retract rear stabilizer legs by removing cotter pin pin and lifting them', NULL, @Category2Id, 230),
      (N'RV - Remove and stow wheel chocks, x-chocks, and landing gear blocks.', NULL, @Category2Id, 240),
      (N'RV - Check and close all storage doors and lock.', NULL, @Category2Id, 250),
      (N'TRUCK - Lights on / rear camera working', NULL, @Category2Id, 260),
      (N'TRUCK - Validate TPMS reads all tires and validate pressure / temperature', NULL, @Category2Id, 270),
      (N'TRUCK / RV - Verify all lights are operable (turn signals, brake lights, reverse)', NULL, @Category2Id, 280),
      (N'Do a walk-around  the trailer and vehicle and check EVERYTHING before pulling-out', NULL, @Category2Id, 290),
      (N'TRUCK - Mirrors extended', NULL, @Category2Id, 300),
      (N'TRUCK - Put into Tow/Haul Mode', NULL, @Category2Id, 310),
      (N'TRUCK - Reset Trip Meter', NULL, @Category2Id, 320),
      (N'GO!', NULL, @Category2Id, 330)

    -- List 3: Arrival
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'3-Arrival', N'Do these when you arrive and unhitch', 30, @Set1Id)
    SET @List3Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List3Id)
    SET @Category3Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'Back RV into site and set parking brake on truck', NULL, @Category3Id, 10),
      (N'Verify that there is room to extend all three slides', NULL, @Category3Id, 20),
      (N'Verify that you can reach sewer and electrical connections', NULL, @Category3Id, 30),
      (N'Turn battery disconnect to On (red key in drivers front bin)', NULL, @Category3Id, 40),
      (N'Level the trailer side-by-side by adding Lego-blocks under the tires and pulling the RV over them.  (Note: Auto level will compensate for up to 1 degree out of level.  you can see the level values in the One Control App -> Leveling -> Manual Control.', NULL, @Category3Id, 50),
      (N'Chock the tires on both sides with 4 black chocks.  NOTE: This is critical before detaching hitch or trailer may roll.', NULL, @Category3Id, 60),
      (N'Remove cotter pins on both rear stabilizer jacks and drop down to 4th hole (or same height on both sides) and put pins back in', NULL, @Category3Id, 70),
      (N'Pull pins on front stabilizer jacks and drop them to 8" off the ground (or same on both sides)  Don''t drop too far or the auto-level sequence will fail.  If one side is much lower, put blocks under that side.', NULL, @Category3Id, 80),
      (N'Place at least one Lego block under each stabilizer jack foot', NULL, @Category3Id, 90),
      (N'Turn on leveling panel (in drivers front bin) by pushing up & down buttons simultaneously.', NULL, @Category3Id, 100),
      (N'Push up button on leveling panel to lower front stabilizers to raise trailer up and take weight off truck, raising it just enough to see daylight between hitch and the 5th wheel', NULL, @Category3Id, 110),
      (N'Remove cotter locking pin from 5th wheel hitch handle', NULL, @Category3Id, 120),
      (N'Pull handle on 5th wheel hitch to unlock king pin (you may have to back up a smidge)', NULL, @Category3Id, 130),
      (N'Drop tailgate', NULL, @Category3Id, 140),
      (N'Unhook breakaway cable and 7 pin connection from rear of truck', NULL, @Category3Id, 150),
      (N'Pull truck forward slowly until it is out from under the camper.', NULL, @Category3Id, 160),
      (N'Raise tailgate', NULL, @Category3Id, 170),
      (N'Stow the 7 pin connector cable and breakaway cable up in the hitch', NULL, @Category3Id, 180),
      (N'Press Auto-Level on leveling panel and RV will start moving all 4 stabilizers until it is level.  This can take 2-3 minutes.  You can watch status and abort or fix issues using the One Control app.', NULL, @Category3Id, 190),
      (N'Install the X-chocks between tires on both sides to stabilize RV', NULL, @Category3Id, 200),
      (N'Turn off breaker in the Electrical Panel and attach the Surge Guard to verify the circuit is OK.  Turn off breaker.', NULL, @Category3Id, 210),
      (N'Plug 50 amp extension cord into Surge Guard and into RV.  Turn on breaker again and Surge Guard will re-verify circuit and connect the power', NULL, @Category3Id, 220),
      (N'Use the Control Panel to extend the slides, turn-on water heater using the inside switch (gas AND electric), and put out awning', NULL, @Category3Id, 230),
      (N'Spray city water spigot with bleach to clean, then hook up pressure regulator, then Y adapter, then the Zero-G water supply hose, then water filter, then flexible connector', NULL, @Category3Id, 240),
      (N'Turn on water for a minute to push the air out of the hose before connecting to RV', NULL, @Category3Id, 250),
      (N'Connect flexible connector to inlet on RV, then connect hose and filter', NULL, @Category3Id, 260),
      (N'Set RV to City Water', NULL, @Category3Id, 270),
      (N'Turn on water', NULL, @Category3Id, 280),
      (N'If using water tank (not city water), turn on the water pump', NULL, @Category3Id, 290),
      (N'Turn on propane ', NULL, @Category3Id, 300),
      (N'Turn on the refrigerator to Auto', NULL, @Category3Id, 310),
      (N'Add one scoop of Happy Camper to toilet along with 5 gallons of water', NULL, @Category3Id, 320)

    -- List 4: Pre-Depart Interior
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'4-Pre-Depart Interior', N'Do these when you are ready to leave', 40, @Set1Id)
    SET @List4Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List4Id)
    SET @Category4Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'Plan route and fuel (gas stations, rest stops, send next location to phone for GPS in truck)', NULL, @Category4Id, 10),
      (N'Wash dishes and pack away', NULL, @Category4Id, 20),
      (N'Bedroom secure, closet doors latched, bathroom door latched open', NULL, @Category4Id, 30),
      (N'Bedroom windows closed', NULL, @Category4Id, 40),
      (N'Toilet - pour jug of water down toilet to loosen up any build-up during the drive back', NULL, @Category4Id, 50),
      (N'Shower secure (everything off shelves, shower door locked closed)', NULL, @Category4Id, 60),
      (N'Bathroom secure (vent closed, counter items stowed, main bathroom door shut)', NULL, @Category4Id, 70),
      (N'Sweep, vacuum, and if necessary, mop floors and carpets; shake-out mats', NULL, @Category4Id, 80),
      (N'Living room secure (carpets stowed, all drawers and cabinets closed)', NULL, @Category4Id, 90),
      (N'Decorations off wall and stowed, coasters and table top items stowed', NULL, @Category4Id, 100),
      (N'Refrigerator items secure', NULL, @Category4Id, 110),
      (N'Refrigerator power selector to OFF for traveling', NULL, @Category4Id, 120),
      (N'Refrigerator doors shut and latched', NULL, @Category4Id, 130),
      (N'Outdoor stove range vent door closed ', NULL, @Category4Id, 140),
      (N'Table and chairs secured with padding', NULL, @Category4Id, 150),
      (N'Discard trash/recycling - both kitchen and bathroom', NULL, @Category4Id, 160),
      (N'Pantry door closed', NULL, @Category4Id, 170),
      (N'Windows closed / blinds up', NULL, @Category4Id, 180),
      (N'Ceiling vents closed', NULL, @Category4Id, 190),
      (N'If returning home: get laundry and clothes ready so you don''t have to open BR slide', NULL, @Category4Id, 200),
      (N'If returning home: get items out of pantry so you don''t have to open LR slide', NULL, @Category4Id, 210),
      (N'One-Control Items off (water pump, water heater, ALL Lights)', NULL, @Category4Id, 220),
      (N'Purse, drinks, snacks all ready and put into truck', NULL, @Category4Id, 230),
      (N'Final walkthrough - re-check doors, windows, surfaces', NULL, @Category4Id, 240),
      (N'Retract awning', NULL, @Category4Id, 250),
      (N'Slides: one last check for obstacles inside and out', NULL, @Category4Id, 260),
      (N'Slides: Retract with door, vent, or window open for air flow', NULL, @Category4Id, 270)

    -- List 5: Pre-Depart Exterior
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'5-Pre-Depart Exterior', N'Do these when you are ready to leave', 50, @Set1Id)
    SET @List5Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List5Id)
    SET @Category5Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'Tire pressure checked', NULL, @Category5Id, 20),
      (N'Fluid levels checked', NULL, @Category5Id, 30),
      (N'Put spare key in truck', NULL, @Category5Id, 40),
      (N'Clear outside roof on top of all slides is free of debris', NULL, @Category5Id, 50),
      (N'Close all roof vents', NULL, @Category5Id, 60),
      (N'TV antenna turned to transport position', NULL, @Category5Id, 70),
      (N'Disconnect from television cable', NULL, @Category5Id, 80),
      (N'Replace/store amenities (e.g. sweep and roll carpets, BBQ, chairs and tables)', NULL, @Category5Id, 90),
      (N'When connecting stinky slinky, put in ground first, then connect to RV', NULL, @Category5Id, 100),
      (N'Dump black tank', NULL, @Category5Id, 110),
      (N'Flush/rinse black tank by adding water thru inlet with orange hose until water runs clear', NULL, @Category5Id, 120),
      (N'Dump grey tanks and geo-treat (keep 5 gallons in grey tanks to slosh around)', NULL, @Category5Id, 130),
      (N'Shut both valves, WAIT a few minutes, then disconnect and drain from RV towards sewer', NULL, @Category5Id, 140),
      (N'Rinse out dump hose (using orange hose and hose fitting)', NULL, @Category5Id, 150),
      (N'Put away stinky slinky in rear bumper', NULL, @Category5Id, 160),
      (N'Treat Black Tank (???)', NULL, @Category5Id, 170),
      (N'Wash/Disinfect hands', NULL, @Category5Id, 180),
      (N'Turn off Water pump', NULL, @Category5Id, 190),
      (N'Disconnect from Water', NULL, @Category5Id, 200),
      (N'Connect both ends of the potable water hose together to keep them clean', NULL, @Category5Id, 210),
      (N'Turn off water heater/furnace/air conditioner', NULL, @Category5Id, 220),
      (N'Secure electrical and plumbing (water hose, power cord, poop hose)', NULL, @Category5Id, 230),
      (N'Pull in slides', NULL, @Category5Id, 240),
      (N'Disconnect from Power', NULL, @Category5Id, 250),
      (N'Turn off propane', NULL, @Category5Id, 260),
      (N'Outside storage bay door secured', NULL, @Category5Id, 270),
      (N'Inspect site for left items, litter', NULL, @Category5Id, 280)

    -- List 6: Back Home
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'6-Back Home', N'Do these when you are arrive back home', 60, @Set1Id)
    SET @List6Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List6Id)
    SET @Category6Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'Move trailer to home', NULL, @Category6Id, 10),
      (N'Do arrival checklist (up to the point of hookups)', NULL, @Category6Id, 20),
      (N'Put out slides or awning if put away wet', NULL, @Category6Id, 30),
      (N'If dry-camping, empty fresh-water tank and refill the trailer with water', NULL, @Category6Id, 40),
      (N'Remove food from fridge and any other perishables', NULL, @Category6Id, 50),
      (N'Launder clothing/towels/bedding and return to trailer', NULL, @Category6Id, 60),
      (N'Confirm that the fridge and propane are turned-off', NULL, @Category6Id, 70),
      (N'Open fridge/freezer and confirm they are off, set lever so they don''t close', NULL, @Category6Id, 80),
      (N'Top-off batteries (if necessary)', NULL, @Category6Id, 90),
      (N'Wipe hitch clean and cover it', NULL, @Category6Id, 100),
      (N'Lock tools away', NULL, @Category6Id, 110),
      (N'Turn-off propane and turn the battery disconnect switch to off (in front bin)', NULL, @Category6Id, 120),
      (N'Confirm everything is locked', NULL, @Category6Id, 130),
      (N'Do a vehicle walk-around to make sure everything is secured', NULL, @Category6Id, 140)

    -- List 7: Maintenance
    INSERT INTO [dbo].[TemplateList] ([ListName], [ListDscr], [SortOrder], [SetId]) VALUES (N'7-Maintenance', N'Do these periodically', 70, @Set1Id)
    SET @List7Id = SCOPE_IDENTITY()
    INSERT INTO [dbo].[TemplateCategory] ([CategoryText], [ListId]) VALUES (N'Main', @List7Id)
    SET @Category7Id = SCOPE_IDENTITY()

    INSERT INTO [dbo].[TemplateAction] ([ActionText], [ActionDscr], [CategoryId], [SortOrder]) VALUES
      (N'2x/year - ¼ c Clorox for 15 gallons watter into fresh water tank (Freshen water; add and flush)', NULL, @Category7Id, 10),
      (N'2x/year - UV retreat roof', NULL, @Category7Id, 20),
      (N'2x/year - Vaseline toilet (prevents TP sticking)', NULL, @Category7Id, 30),
      (N'3 year - Redo roof sealant (strip and replace ~$1000)', NULL, @Category7Id, 40),
      (N'Frequent - Lube hitch ball', NULL, @Category7Id, 50),
      (N'Inspect and clean roof membrane each month with one step cleaner and treatment like protectall', NULL, @Category7Id, 60),
      (N'Slide Outs:   Keep clean and treated and lubricated.  Two seals - one on slide out and one on wall - side is only water right when fully in or out.  Put protectant spray on seals.', NULL, @Category7Id, 70),
      (N'Axle lubricant: repack bearings and remove old grease every 12 months or 12K miles. ', NULL, @Category7Id, 80),
      (N'Brakes: inspect and adjust every 3000 miles or annually (at least)', NULL, @Category7Id, 90),
      (N'Clean in and UNDER your cabinet. Things will leak under the cabinet. Put a bead of silicone sealant around the cabinets to seal it up. ', NULL, @Category7Id, 100),
      (N'AC: take cover off and clean foam filter and debris from entry. Consider adding screens over openings for A/C or water heater. ', NULL, @Category7Id, 110),
      (N'Propane: have propane systems treated and checked annually by certified technician - pressure/leak down test. Tanks must be less than 12 years old.', NULL, @Category7Id, 120),
      (N'Check water levels in batteries (1x/month or more) - add distilled water', NULL, @Category7Id, 130)
END
GO
