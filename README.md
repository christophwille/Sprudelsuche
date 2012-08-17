# Sprudelsuche
Windows 8 App zur Suche nach der günstigsten Tankstelle in Österreich.  
  
Developer-centric information on license et al is provided at the end of the page in English. There is a [quick tour](https://github.com/christophwille/Sprudelsuche/wiki/Developer-Quick-Tour)
in the Wiki to get an overview of the project before digging into the code.

## Features

* `Umgebungssuche` - Suche nach der günstigsten Tankstelle nahe der aktuellen Position
* `Hintergrundupdates` - Gespeicherte Suchen werden automatisch stündlich aktualisiert (optional, siehe Einstellungen)
* `Anheftbare Kacheln` - Regelmäßig genutzte Suchen können an Start angeheftet werden damit aktuelle Preise immer im Blick sind
* `Sharing` - Suchergebnisse können an andere Apps weitergegeben werden (zB Mail)

## Installation
Die Installation erfolgt über den Store.

## Ich habe einen Fehler gefunden
Bitte diesen [Fehler melden](https://github.com/christophwille/Sprudelsuche/issues/new) damit er nachverfolgt werden kann.
  
Feedback (Wünsche, Anregungen, Beschwerden) bitte via [Uservoice](https://sprudelsuche.uservoice.com/)

## Credits and Acknowledgements
* [Christoph Wille](mailto:christoph.wille@gmail.com) - concept and initial implementation.
* [Wolfgang Hoffelner](http://www.wo-ho.at/) - logo design

### Services Used
* Geocoding is performed via [Nominatim Search](http://open.mapquestapi.com/nominatim/)
* Pricing information is retrieved from [spritpreisrechner.at](http://www.spritpreisrechner.at/)

## License
The project is licensed under MIT, however, the following sub-projects have diverging licenses:

* NotificationsExtensions - this is a part of the Windows 8 SDK.
* Sprudelsuche.ThirdParty - various code (adapted) with links to source.
