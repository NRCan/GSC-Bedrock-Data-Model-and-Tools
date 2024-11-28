# Bedrock Data Model and Tools

([Français](#Modèle-de-données-et-outils-pour-la-roche-en-place))
In the early years of use of spatial databases, geologic data models were designed around the traditional content of a geological map. These models were using a map centric, highly denormalized approach characterized by multiple features, each representing a self-contained thematic geological map. Recently, the trend moved towards a data centric type of modeling and several ongoing projects have embraced this concept, such as the North American Geologic Map Data Model (NADM) and the GeoSciML. This repository presents the source code and development of a functioning set of tools that uses a generic feature acquisition model that meets the requirements of the GSC bedrock mapping projects. Focused on a data centric approach, this standardized model offers solutions for the management of data throughout the typical dataset construction workflow such as data acquisition, compilation and interpretation. The application, developed on ArcGIS platform, provides a simple and natural work flow through customized editing tools (add-in) while maintaining a coherent database and feature level metadata in the background.

## Requirements

ESRI ArcGIS Desktop 10.8.X and higher

## Installation

Please refer to the original ESRI method for pluggin installation:
[Installation](https://desktop.arcgis.com/en/arcmap/latest/analyze/python-addins/sharing-and-installing-add-ins.htm)

## Latest Version

Please download the latest version from the [Release page](https://github.com/NRCan/GSC-Bedrock-Data-Model-and-Tools/releases).

## References

* GSC Symbol standard: https://doi.org/10.4095/327025
* Bedrock database model and user guide publication: https://doi.org/10.4095/314673

## Data Format

The current tool supports File Geodatabases (.gbd) only.

## Contacts

Gabriel Huot-Vézina: gabriel.huot-vezina@nrcan-rncan.gc.ca

Étienne Girard: etienne.girard@nrcan-rncan.gc.ca

### How to Contribute

See [CONTRIBUTING.md](CONTRIBUTING.md)

### License
<a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc/4.0/88x31.png" /></a><br />This work is licensed under a <a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/">Creative Commons Attribution-NonCommercial 4.0 International License</a>.

[Licence document](LICENCE.txt)

The Canada wordmark and related graphics associated with this distribution are protected under trademark law and copyright law. No permission is granted to use them outside the parameters of the Government of Canada's corporate identity program. For more information, see [Federal identity requirements](https://www.canada.ca/en/treasury-board-secretariat/topics/government-communications/federal-identity-requirements.html).

#### Natural Resources Canada, Geological Survey of Canada
##### © His Majesty the King in Right of Canada as represented by the Minister of Natural Resources, 2024.

______________________

# Modèle de données et outils pour la roche en place

([English](#Bedrock-Data-Model-and-Tools))

Lors des premières années d'utilisation des bases de données à référence spatiale, les modèles de données géologiques étaient conçus autour du contenu des cartes géologiques traditionnelles. Ces modèles étaient centrés sur la carte elle-même, une approche hautement dénormalisée et caractérisée par de multiples entités spatiales, chacunes contenant un thème propre à la carte géologique. Récemment, la tendance pousse vers une modélisation centrée sur la donnée et leur normalisation. Plusieurs projets en cours s'élaborent autour de ce concept, comme le Modèle Nord-Américain de Carte géologique (MNAD) et GeoSciML. Le présent répertoire contient le code source et le développement d'une suite d'outils fonctionnels qui utilisent des entités spatiales génériques comme modèle d'acquisition afin de subvenir au prérequis des projets de cartographie géologique au sein de la Commission géologique du Canada. En ciblant une approche centrée sur la donnée, cette standardisation offre une solution à la gestion des données tout au long d'une procédure normale d'acquisition, de compilation et d'interprétation. Ces outils, développés pour la plateforme ArcGIS Desktop, fournissent une procédure fluide et simple à l'aide d'une extension (add-in) tout en maintenant une base de données cohérente et des métadonnées pour chacune des entités en arrière-plan.

## Pré-requis

Cet outil a été developé et testé sous ArcGIS Desktop TM version 10.8.X et plus.

## Installation

Veuillez-vous référer au document d'origine d'ESRI en ce qui concerne l'installation des .esriaddin:
[Installation](https://desktop.arcgis.com/en/arcmap/latest/analyze/python-addins/sharing-and-installing-add-ins.htm)

## Dernière version

Veuillez télécharger la dernière version depuis la [page des versions](https://github.com/NRCan/GSC-Bedrock-Data-Model-and-Tools/releases).

## Références

* Standard des symboles de la Commission géologique du Canada: https://doi.org/10.4095/327025
* Publication du guide de l'usager des outils et du modèle de données de roche en place: https://doi.org/10.4095/314673

## Format des données

Les outils supportent les fichiers de type File Geodatabase (.gdb) seulement.

## Contacts

Gabriel Huot-Vézina: gabriel.huot-vezina@nrcan-rncan.gc.ca

Étienne Girard: etienne.girard@nrcan-rncan.gc.ca

### Comment contribuer

See [CONTRIBUTING.md](CONTRIBUTING.md)

### License
<a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/"><img alt="Creative Commons License" style="border-width:0" src="https://i.creativecommons.org/l/by-nc/4.0/88x31.png" /></a><br />Ce projet est supporté par la licence <a rel="license" href="http://creativecommons.org/licenses/by-nc/4.0/">Creative Commons Attribution-NonCommercial 4.0 International License</a>.

[Document de licence](LICENCE_French.txt)

Le mot-symbole « Canada » et les éléments graphiques connexes liés à cette distribution sont protégés en vertu des lois portant sur les marques de commerce et le droit d'auteur. Aucune autorisation n'est accordée pour leur utilisation à l'extérieur des paramètres du programme de coordination de l'image de marque du gouvernement du Canada. Pour obtenir davantage de renseignements à ce sujet, veuillez consulter les [Exigences pour l'image de marque](https://www.canada.ca/fr/secretariat-conseil-tresor/sujets/communications-gouvernementales/exigences-image-marque.html).

#### Ressources naturelles Canada, Commission géologique du Canada.
##### © Sa Majesté le Roi du Chef tu Canada représenté par le ministre des Ressources naturelles, 2024.
