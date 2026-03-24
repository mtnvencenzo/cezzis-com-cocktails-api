# Cocktail Metadata Skill Guide

**Purpose:**  
Generate JSON metadata for cocktails suitable for Qdrant vector search. Arrays must be accurate, historically-informed, and consistent. Creativity is allowed and encuraged when justified.  

**Usage:**
As a user of this skill, you have the ability to take in data about a cocktail for analysis.  You can recommend better keywords and search terms that can be used ias filters against a qdrant vector search for the most professiona and accurate results possible.  The search against the qdrant data is a human initiated search via entering common terms and phrases into a single text input that relates to there biases, generational vocabulary and trends.  Teh usage of the search spans across multiple generations.

You are also able to take in existing keywords and era values to analyze the accuracy of those based on the given content to recommend better search keyowords that are pertainient to todays and yesterdays searches.

---

## Arrays & Descriptions

### `keywordsBaseSpirit`
**Description:** Primary alcoholic base of the cocktail.  
**Examples (common values):**  
- gin  
- rum  
- bourbon  
- rye  
- vodka  
- tequila  
- brandy  

---

### `keywordsSpiritSubtype`
**Description:** Specific subtype of the base spirit. Optional if not needed.  
**Examples:**  
- white rum  
- dark rum  
- aged rum  
- London dry gin  

---

### `keywordsFlavorProfile`
**Description:** Flavor, aroma, mouthfeel descriptors. Can include multiple entries.  
**Common values:**  
- citrus  
- sweet  
- tart  
- bitter  
- herbal  
- floral  
- tropical  
- nutty  
- complex  
- smooth  
- effervescent  
- frothy  

---

### `keywordsCocktailFamily`
**Description:** Category or cocktail family.  
**Common values:**  
- sour  
- daisy  
- fizz  
- highball  
- tiki  
- stirred-spirit  
- equal-parts  
- whiskey-variant  
- daiquiri-variant  
- martini-variant  

---

### `keywordsTechnique`
**Description:** Preparation or mixing methods.  
**Common values:**  
- shaken  
- dry-shaken  
- stirred  
- built  
- topped  

---

### `keywordsStrength`
**Description:** Alcoholic strength.  
**Values:**  
- light  
- medium  
- strong  

---

### `keywordsTemperature`
**Description:** Served temperature.
**Values:**  
- cold  
- warm  
- frozen

---

### `keywordsSeason`
**Description:** Seasonal context. Can include multiple entries.  
**Common values:**  
- spring  
- summer  
- fall  
- winter  
- all-season  

---

### `keywordsOccasion`
**Description:** Appropriate context or setting. Multiple values allowed.  
**Common values:**  
- evening  
- afternoon  
- brunch  
- garden party  
- craft bar  
- tiki bar  
- nightcap  
- casual  
- party  
- pool party  
- dinner party  

---

### `keywordsMood`
**Description:** Overall impression or experience. Can include multiple descriptors.  
**Common values:**  
- refreshing  
- clean  
- cheerful  
- elegant  
- classic  
- refined  
- sophisticated  
- fun  
- tropical  
- bold  
- lively  
- adventurous  

---

### `keywordsSearchTerms`
**Description:** Key searchable items like spirits, liqueurs, garnishes, bartenders, bars, cities, or nicknames. Include 3–8 terms.  
**Common values (examples from prior cocktails):**  
- lime  
- maraschino  
- Chartreuse  
- Angostura  
- simple syrup  
- tonic water  
- sweet vermouth  
- New York  
- Trader Vic  
- tiki  
- Prohibition  

---

### `eras`
**Description:** Historical era(s) relevant to the cocktail’s creation or popularity. Use historically accurate periods. Multiple entries allowed if needed. Content in parentheses should be excluded and is only for reference.  The eras array must always contain either 'traditional' or 'modern' depending on the cocktail but never both. If the cocktail is referenced prior to prohibition it must have the label -pre-prohibition but can contain others.
**Common values:**  
- traditional
- modern
- pre-prohibition  
- prohibition  
- post-prohibition  
- gilded age  
- roaring twenties  
- mid-20th century  
- 19th century  
- 20th century  
- 21st century  

---

## Guidelines for Use

1. **Array Names:** Must use arrays exactly as named. No renaming or omission.  
2. **Accuracy:** Entries must be accurate to the cocktail’s ingredients, history, and flavor.  
3. **Eras:** Must reflect creation date or peak popularity. Combine eras if relevant.  
4. **Creativity:** Allowed in mood, flavor profile, occasion, or search terms if justified by the cocktail.  
5. **Consistency:** Use common values where applicable for vector search effectiveness, but new keywords are allowed.  
6. **Search Terms:** Include key searchable elements (ingredients, alternate names, bars, cities, bartenders).  

---

## Qdrant Optimization Rules (Critical)

### 1. Use Natural Language (No Artificial Hyphenation)
Always prefer human-readable phrases:
- "London dry gin" ✅  
- "blanco tequila" ✅  
- "rye whiskey" ✅  
- Avoid: "London-dry-gin", "blanco-tequila"

---

### 2. Think Like a User Search
Use terms people would realistically type:
- "lime juice" instead of "lime"  
- "orange bitters" instead of "bitters"  
- "maraschino liqueur" instead of "maraschino"

---

### 3. Duplicate Important Terms Across Fields
Reinforce key concepts for better retrieval:
- Include "London dry gin" in both subtype and search terms  
- Repeat key ingredients where helpful  

---

### 4. Use Both Specific and General Terms
Improve recall by covering variations:
- "Cointreau" + "triple sec"  
- "maraschino liqueur" + "cherry"  

---

### 5. Avoid Over-Normalization
Do not sacrifice clarity for rigid taxonomy:
- Prefer readable, familiar phrasing  
- Optimize for embeddings and human queries  

---

### 6. Include Contextual Search Hooks
Use a mix of:
- Ingredients (lime juice, vermouth)  
- People (bartenders, cultural figures)  
- Places (cities, bars)  
- Variants (Tommy’s Margarita, Martinez cocktail)  

---

### 7. Balance Precision and Breadth
Each cocktail should include:
- Core ingredients (high precision)  
- Broader category terms (better recall)  

---

**Goal:**  
Maximize search filters used as filtering clauses on a semantic vector search while providing historical, cultural and cocktail accuracy.