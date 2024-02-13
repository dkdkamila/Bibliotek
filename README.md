# Bibliotek
Moment 3 

Översikt över projektets metoder och hur den fungerar:
Projektet följer MVC-mönstret, där:

Modell (Model) representerar data i applikationen och innehåller logiken för att hantera och manipulera den.

Vy (View) är gränssnittet som användaren interagerar med och visar informationen som presenteras för användaren.

Kontroller (Controller) hanterar användarens interaktioner, behandlar inkommande begäranden från användaren, samverkar med modellen och returnerar korrekt vy till användaren.

Entity Framework Core används för att hantera databasoperationer. Det används för att definiera modellklasser och databaskontextklasser samt för att utföra CRUD-operationer (Create, Read, Update, Delete) mot databasen.

Razor Pages och Razor Views används för att skapa webbsidor och generera HTML-innehåll. Razor-syntaxen möjliggör att C#-kod kan blandas med HTML för att dynamiskt generera webbsidor.

Många metoder i kontrollern är asynkrona och använder Task<IActionResult> som returtyp. Detta gör att metoder kan utföras asynkront för att förbättra prestanda och skalbarhet i applikationen.

Validering utförs både på klient- och serversidan. På serversidan används Data Annotations i modellklasserna för att definiera valideringsregler som tillämpas vid modellbindning och databasoperationer.

 TempData används för att lagra tillfällig data som ska visas för användaren under en enda förfrågan, vilket används för att visa meddelanden efter att en åtgärd har utförts.

 Utlåningsfunktionaliteten har implementerats genom att lägga till en Borrow-metod i kontrollern som uppdaterar bokens information i databasen när en användare lånar en bok. Informationen som uppdateras inkluderar låntagarens namn, utlåningsdatum och bokens status.

 Felhantering har implementerats genom att använda try-catch-block i kontrollern för att hantera och logga fel som uppstår vid databasoperationer eller andra oväntade händelser.

Det finns också en sökfunktion som gör det möjligt för användare att söka efter böcker efter titel, författare eller genre.
