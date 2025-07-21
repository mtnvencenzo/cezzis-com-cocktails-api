import json

def load_json_file(file_path):
    """Load and parse a JSON file."""
    with open(file_path, 'r', encoding='utf-8') as f:
        return json.load(f)

def extract_ingredient_ids_from_cocktails(cocktails_data):
    """Extract all ingredient IDs from cocktails data."""
    ingredient_ids = set()
    
    for cocktail in cocktails_data:
        if 'ingredients' in cocktail:
            for ingredient in cocktail['ingredients']:
                if 'ingredientId' in ingredient:
                    ingredient_ids.add(ingredient['ingredientId'])
    
    return ingredient_ids

def extract_ingredient_ids_from_ingredients(ingredients_data):
    """Extract all ingredient IDs from ingredients data."""
    ingredient_ids = set()
    
    for ingredient in ingredients_data:
        # Add the main ingredient ID
        ingredient_ids.add(ingredient['id'])
    
    return ingredient_ids

def audit_ingredient_ids():
    """Audit ingredient IDs in cocktails against ingredients data."""
    
    # Load the data files
    cocktails_data = load_json_file('src/Cocktails.Api.Infrastructure/Resources/ModernCocktails/_modern-cocktails-data.json')
    ingredients_data = load_json_file('src/Cocktails.Api.Infrastructure/Resources/Ingredients/_ingredients-data.json')
    
    # Extract ingredient IDs from cocktails
    cocktail_ingredient_ids = extract_ingredient_ids_from_cocktails(cocktails_data)
    
    # Extract ingredient IDs from ingredients data
    master_ingredient_ids = extract_ingredient_ids_from_ingredients(ingredients_data)
    
    # Find missing ingredient IDs
    missing_ingredient_ids = cocktail_ingredient_ids - master_ingredient_ids
    
    # Find valid ingredient IDs
    valid_ingredient_ids = cocktail_ingredient_ids & master_ingredient_ids
    
    # Print results
    print("=== INGREDIENT ID AUDIT RESULTS ===\n")
    
    print(f"Total ingredient IDs found in cocktails: {len(cocktail_ingredient_ids)}")
    print(f"Total ingredient IDs in ingredients data: {len(master_ingredient_ids)}")
    print(f"Valid ingredient IDs: {len(valid_ingredient_ids)}")
    print(f"Missing ingredient IDs: {len(missing_ingredient_ids)}\n")
    
    if valid_ingredient_ids:
        print("VALID INGREDIENT IDs:")
        for iid in sorted(valid_ingredient_ids):
            print(f"  ✓ {iid}")
        print()
    
    if missing_ingredient_ids:
        print("MISSING INGREDIENT IDs (need to be added to ingredients data):")
        for iid in sorted(missing_ingredient_ids):
            print(f"  ✗ {iid}")
        print()
        
        # Show which cocktails use the missing ingredient IDs
        print("COCKTAILS USING MISSING INGREDIENT IDs:")
        for cocktail in cocktails_data:
            if 'ingredients' in cocktail:
                missing_in_cocktail = []
                for ingredient in cocktail['ingredients']:
                    if 'ingredientId' in ingredient and ingredient['ingredientId'] in missing_ingredient_ids:
                        missing_in_cocktail.append(ingredient['ingredientId'])
                
                if missing_in_cocktail:
                    print(f"  {cocktail['title']} ({cocktail['id']}):")
                    for missing in missing_in_cocktail:
                        print(f"    - {missing}")
                    print()
    else:
        print("✓ All ingredient IDs are valid!")
    
    return missing_ingredient_ids

if __name__ == "__main__":
    audit_ingredient_ids() 