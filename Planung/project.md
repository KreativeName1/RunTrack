# Projekt 
## Anforderungsanalyse
- Login für Admins und Scanner. Der Admin ist in der Datenbank fest. Der Admin kann benutzer verwalten.

1. Verwaltung
- NUR ADMIN, verschoben im ADMIN-Panel
- Verwatlung der Benutzer, zugangsdaten
- Schüler, Klassen, Schulen, Rundenarten in tabelle eintragen oder als csv import
- Ausdruck der Schüler nach Klasse (PDF)
  - Angaben auf Blatt:
  - Schule- und Klassenname
  - Druckdatum
  - List der Schüler
- Generierung der Zugangsdaten als PDF nach Klasse (Benutzername und Passwort) -> FÜR WEB
- Generierung der Barcodes mit Name (PDF)

2. Scannen
- Eingabe des Barcode Scanners
- Liste der zuletzt gescannen Läufer
- Feld zur Bestätigung des Scannens mit Nme und Id des Läufers
- Button zum Beenden mit Bestätigung
- Wird in einer Lokalen Datenbank gespeichert


3. Bewertung
   1. Bewertung sortiert nach (kombinierbar):
      - Rundenart
      - Rundenanzahl
      - Strecke in Metern
      - Schnell -> langsam Runden
      - Nach Schule
      - Nach Klasse

   2. PDF für Bewertung (für jeden Läufer generiert)
   - Name, id schule, klasse
   - Tabelle der Runden mit Zeiten
Nach der Bewertung werden die Daten in einer Datenbank auf Server gespeichert


4. Admin Funktionen
  - Verwaltung siehe 1.
  - Generierung der Zugangsdaten der Benutzer als PDF


## Design

### Login
![Login](/images/Login.jpg)


## Scannen
![Scanner](/images/Scannen.jpg)

## Verwaltung
![Verwaltung](/images/Verwaltung.png)
---------------------------------------------------------------------------------------



## Datenbank

Runner
- int id
- string name
- int class_id

Class
- int id
- string name
- int roundType_id
- id school_id

RoundType
- int id
- int meters
- string name

School
- int id
- string name

Round
- int id
- int runner_id
- DateTime timestamp














## ALT
1. Login
2. Startseite:
  - Verwaltung
  - Scannen
  - Bewertung
  - \[Admin] Benutzerverwaltung

1. Verwalten
   - Anlegen von Schulen, Klassen
   - Liste der Klassen nach Schule für Schülereingabe
   - \[Admin] Löschen der Daten
2. Scannen
   - Barcode einscannen
   - anzeige der letzen eingescannten Läufer
   - Bestätigung des einscannens mit name und id
   - Beenden Button mit Bestätigung
3. Bewertung
4. \[Admin] Benutzerverwaltung
   - Liste der Benutzer mit Edit und Löschen Button
   - Neuer Benutzer Anlegen
   - Drucken der Zugangsdaten
