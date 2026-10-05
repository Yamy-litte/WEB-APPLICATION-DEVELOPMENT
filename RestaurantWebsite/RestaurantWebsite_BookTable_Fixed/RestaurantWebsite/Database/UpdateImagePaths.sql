/*
    Run this script once against the existing RestaurantWebsite database
    after moving the image files into:
        Resources/User/images/dish
        Resources/User/images/combo
*/

-- Move existing DishImage paths from the old flat folder to /dish.
UPDATE dbo.DishImages
SET ImageUrl = REPLACE(
    ImageUrl,
    '/Resources/User/images/dish',
    '/Resources/User/images/dish/dish'
)
WHERE ImageUrl LIKE '/Resources/User/images/dish%'
  AND ImageUrl NOT LIKE '/Resources/User/images/dish/dish%';

-- Also handle ~/ style paths if any old records use them.
UPDATE dbo.DishImages
SET ImageUrl = REPLACE(
    ImageUrl,
    '~/Resources/User/images/dish',
    '~/Resources/User/images/dish/dish'
)
WHERE ImageUrl LIKE '~/Resources/User/images/dish%'
  AND ImageUrl NOT LIKE '~/Resources/User/images/dish/dish%';

-- Update the demo combo records to use the images that actually exist in the project.
IF EXISTS (SELECT 1 FROM dbo.Combos WHERE Name = 'Family Combo')
BEGIN
    UPDATE dbo.Combos
    SET ImageUrl = '~/Resources/User/images/combo/combo01.jpg'
    WHERE Name = 'Family Combo';
END

IF EXISTS (SELECT 1 FROM dbo.Combos WHERE Name = 'Chicken Meal Combo')
BEGIN
    UPDATE dbo.Combos
    SET ImageUrl = '~/Resources/User/images/combo/combo03.jpg'
    WHERE Name = 'Chicken Meal Combo';
END

IF EXISTS (SELECT 1 FROM dbo.Combos WHERE Name = 'Dessert Combo')
BEGIN
    UPDATE dbo.Combos
    SET ImageUrl = '~/Resources/User/images/o2.jpg'
    WHERE Name = 'Dessert Combo';
END
