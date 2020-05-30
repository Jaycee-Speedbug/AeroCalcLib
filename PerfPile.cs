using System;
using System.Collections.Generic;
using System.Linq;


namespace AeroCalcCore {


    /// <summary>
    /// Classe de dimension 4 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un ensemble cohérent de Layer de performances de vol
    /// </summary>
    /// <remarks>
    /// Deux objets de même nature contenant les performances (deux PerfPoint, deux PerfSerie, deux PerfLayer),
    /// dans un même ensemble, ne peuvent pas avoir un factorValue identique.
    /// Les PerfPile n'ont pas cette limitation dans le container les regroupants (PerfContainer).
    /// De plus, les PerfPile peuvent être caractérisées par une valeur discrète.
    /// 
    /// PREV IMPLEMENTATION:
    /// public class PerfPile : IComparer<PerfPile>, IEquatable<PerfPile>
    /// 
    /// TODO: Attention, les implémentations des interfaces sont minimalistes et ne comprennent pas les surcharges d'opérateurs
    /// </remarks>
    /// 
    public class PerfPile : IComparable<PerfPile>, IEquatable<PerfPile> {

        // FIELDS ////////////////////////////////////////////////////////////////////////////////////


        // PROPERTIES ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clé de la pile dans une base de donnée
        /// </summary>
        public long dataBaseKey {
            get;
            set;
        }

        /// <summary>
        /// Nombre de layers présentes dans la pile
        /// </summary>
        public int count {
            get {
                return perfLayerList.Count;
            }
        }

        /// <summary>
        /// Nom de la pile (nom des résultat des prédictions réalisées avec cette pile)
        /// </summary>
        /// <remarks>Dim z</remarks>
        public String outputName {
            get;
            set;
        }

        /// <summary>
        /// Unité de mesure de la prédiction
        /// </summary>
        public int outputUnitCode {
            get;
            set;
        }

        /// <summary>
        /// Nom de la première dimension utilisée pour les calculs de prédiction
        /// </summary>
        public String pointFactorName {
            get;
            set;
        }

        /// <summary>
        /// Unité de mesure de la première dimension, utilisée pour les calculs de prédiction
        /// </summary>
        public int pointFactorUnitCode {
            get;
            set;
        }

        /// <summary>
        /// Nom de la deuxième dimension utilisée pour calculer une prédiction
        /// </summary>
        public String serieFactorName {
            get;
            set;
        }

        /// <summary>
        /// Unité de mesure de la deuxième dimension, utilisée pour les calculs de prédiction
        /// </summary>
        public int serieFactorUnitCode {
            get;
            set;
        }

        /// <summary>
        /// Nom de la troisième dimension utilisée pour calculer une prédiction
        /// </summary>
        public String layerFactorName {
            get;
            set;
        }

        /// <summary>
        /// Unité de mesure de la troisième dimension, utilisée pour les calculs de prédiction
        /// </summary>
        public int layerFactorUnitCode {
            get;
            set;
        }

        /// <summary>
        /// Valeur du facteur de la Pile (quatrième dimension)
        /// </summary>
        /// <remarks>
        /// Pour l'instant, le facteur n'est d'aucune utilité car la nécessité d'ajouter une quatrième dimension
        /// ne s'est pas fait sentir dans la manipulation des modèles de données de performances.
        /// Rappelons que les données de performances publiées sont principalement basées sur l'emploi de graphes
        /// bidimensionnels.
        /// </remarks>
        public double factorValue {
            get;
            set;
        }

        /// <summary>
        /// Nom de la valeur discrète associée à la Pile
        /// </summary>
        public String discretName {
            get;
            set;
        }

        /// <summary>
        /// Valeur discrète associée à la Pile
        /// Permet d'ajouter un paramètre à une Pile, comme un braquage volet, l'état de surface de la piste
        /// Ce paramètre devra figurer dans la liste des paramètres fournis pour un calcul
        /// </summary>
        public long discretValue { get; set; }

        /// <summary>
        /// Indique si le range de la Pile a été défini
        /// </summary>
        public bool ranged {
            get;
            private set;
        }

        /// <summary>
        /// Etat de sélection de la Pile. La sélection permet de ne prendre en compte
        /// que certaines Pile pour les calculs multi-dimensionnels
        /// </summary>
        public bool selected {
            get;
            set;
        }

        /// <summary>
        /// Commentaire texte sur la réalisation du calcul
        /// </summary>
        public String predictComment {
            get;
            private set;
        }

        /// <summary>
        /// Code d'erreur sous forme d'un entier
        /// </summary>
        public int calcErrorCode {
            get;
            private set;
        }

        /// <summary>
        /// Visibilité du nom du modèle de performance stocké dans la Pile
        /// Permet de rendre invisibles les modèles utilisés comme étape de calcul
        /// </summary>
        public bool hidden { get; private set; }



        /*
         *  MEMBRES
         */
        
        /// <summary>
        /// Borne inférieure du domaine de calcul
        /// </summary>
        /// <remarks>Dim pp2</remarks>
        double startRange;

        /// <summary>
        /// Nature de la borne inférieure du domaine de calcul
        /// </summary>
        int startRangeType;

        /// <summary>
        /// Borne supérieure du domaine de calcul
        /// </summary>
        /// <remarks>Dim pp2</remarks>
        double endRange;

        /// <summary>
        /// Nature de la borne supérieure du domaine de calcul
        /// </summary>
        int endRangeType;

        /// <summary>
        /// Liste générique utilisée pour ordonner les Layer de performances
        /// </summary>
        List<PerfLayer> perfLayerList;


        /*
         *  CONSTRUCTEURS
         */

        /// <summary>
        /// Construction d'une pile vide de toute Layer, et non paramétrée
        /// </summary>
        public PerfPile() {
        }



        /// <summary>
        /// Construction par clonage d'une Pile de performances
        /// </summary>
        /// <param name="ps">Pile de performance à cloner</param>
        public PerfPile(PerfPile pp) {
            this.dataBaseKey = pp.dataBaseKey;
            this.endRange = pp.endRange;
            this.endRangeType = pp.endRangeType;
            this.factorValue = pp.factorValue;
            this.outputName = pp.outputName;
            this.outputUnitCode = pp.outputUnitCode;
            this.pointFactorName = pp.pointFactorName;
            this.pointFactorUnitCode = pp.pointFactorUnitCode;
            this.ranged = pp.ranged;
            this.serieFactorName = pp.serieFactorName;
            this.serieFactorUnitCode = pp.serieFactorUnitCode;
            this.startRange = pp.startRange;
            this.startRangeType = pp.startRangeType;
            this.layerFactorName = pp.layerFactorName;
            this.layerFactorUnitCode = pp.layerFactorUnitCode;
            this.discretName = pp.discretName;
            this.discretValue = pp.discretValue;
            this.hidden = pp.hidden;
            for (int count = 0; count < pp.count; count++) {
                this.add(new PerfLayer(pp.perfLayerList.ElementAt(count)));
            }
        }



        /// <summary>
        /// Construit une Pile vide de toute Layer de performances
        /// </summary>
        /// <param name="ownFactorValue">Valeur du facteur associé à la Pile (dimension 3)</param>
        /// <param name="discretName">Nom du paramètre discret </param>
        /// <param name="discretValue">Valeur du paramètre discret</param>
        /// <param name="pointFactorName">Nom de la dimension des Point de la Pile (dimension 1)</param>
        /// <param name="pointFactorUnitCode">Unité de mesure de la dimension des points</param>
        /// <param name="serieFactorName">Nom de la dimension des Serie de la Pile (dimension 2)</param>
        /// <param name="serieFactorUnitCode">Unité de mesure de la dimension des Serie</param>
        /// <param name="layerFactorName">Nom de la dimension des Layer de la Pile (dimension 3)</param>
        /// <param name="layerFactorUnitCode">Unité de mesure de la dimension des Layer</param>
        /// <param name="outName">Nom de la Pile (nom des résultats des prédictions réalisées avec cette Pile)</param>
        /// <param name="outUnitCode">Unité de mesure de la prédiction</param>
        /// <param name="hidden">Propriété de visibilité du nom du modèle de calcul</param>
        /// 
        public PerfPile(double ownFactorValue,
                        String discretName, long discretValue,
                        String pointFactorName, int pointFactorUnitCode,
                        String serieFactorName, int serieFactorUnitCode,
                        String layerFactorName, int layerFactorUnitCode,
                        String outName, int outUnitCode,
                        bool hidden) {
            this.factorValue = ownFactorValue;
            this.pointFactorName = pointFactorName;
            this.pointFactorUnitCode = pointFactorUnitCode;
            this.serieFactorName = serieFactorName;
            this.serieFactorUnitCode = serieFactorUnitCode;
            this.layerFactorName = layerFactorName;
            this.layerFactorUnitCode = layerFactorUnitCode;
            this.discretName = discretName;
            this.discretValue = discretValue;
            this.outputName = outName;
            this.outputUnitCode = outUnitCode;
            this.hidden = hidden;
            this.perfLayerList = new List<PerfLayer>();

        }



        /*
         *  SERVICES
         */

        /// <summary>
        /// Ajoute une layer dans la pile
        /// </summary>
        /// <param name="newPerfLayer">Nouvelle layer à ajouter à la pile</param>
        /// <returns>True si l'ajout est réussi, False dans le cas contaire</returns>
        public bool add(PerfLayer newPerfLayer) {

            // Deux PerfLayer ne peuvent pas avoir le même factorValue
            foreach (PerfLayer pl in perfLayerList) {
                if (pl.Compare(pl, newPerfLayer) == 0) {
                    return false;
                }
            }

            // Insertion dans la liste classée, en réponse à une difficulté avec la fonction Sort()
            bool inserted = false;
            for (int count = 0; count < perfLayerList.Count; count++) {
                if (newPerfLayer.factorValue < layerAt(count).factorValue) {
                    perfLayerList.Insert(count, newPerfLayer);
                    inserted = true;
                }
            }
            // Si la layer n'a pas été insérée, c'est qu'il faut l'insérer à la fin de la liste
            if (!inserted) {
                perfLayerList.Add(newPerfLayer);
            }
            // Récupération des informations des dimensions
            pointFactorName = newPerfLayer.pointFactorName;
            pointFactorUnitCode = newPerfLayer.pointFactorUnitCode;
            serieFactorName = newPerfLayer.serieFactorName;
            serieFactorUnitCode = newPerfLayer.serieFactorUnitCode;
            // Le domaine de calcul est à redéfinir
            ranged = false;
            return true;
        }



        /// <summary>
        /// Ajoute un Point de performances de vol à une Layer de la Pile
        /// Si la Layer n'existe pas, une Layer est créée pour accueillir le point
        /// </summary>
        /// <param name="newPerfPoint">Nouveau point de performance de vol</param>
        /// <param name="layerFactorValue">Facteur de la layer dans laquelle inscrire le point</param>
        /// <param name="serieFactorValue">Facteur de la série dans laquelle inscrire le point</param>
        /// <returns>True si l'ajout est réussi, False dans le cas contaire</returns>
        public bool add(PerfPoint newPerfPoint, double serieFactorValue, double layerFactorValue) {

            bool foundIt = false;
            bool success = false;

            // Recherche de la layer de même factorValue
            for (int count = 0; count < perfLayerList.Count; count++) {
                if (layerFactorValue == layerAt(count).factorValue) {
                    foundIt = true;
                    success = layerAt(count).add(newPerfPoint, serieFactorValue);
                    break;
                }
            }
            // Pas de Layer ayant un facteur de même valeur, on cré une nouvelle layer
            if (!foundIt) {
                // Création d'une nouvelle layer
                PerfLayer newLayer = new PerfLayer(layerFactorValue,
                                                   pointFactorName, pointFactorUnitCode,
                                                   serieFactorName, serieFactorUnitCode,
                                                   outputName, outputUnitCode);
                success = newLayer.add(newPerfPoint, serieFactorValue);
                if (success) {
                    success = add(newLayer);
                }
            }
            return success;
        }



        /// <summary>
        /// Renvoie la Layer ayant la position index
        /// </summary>
        /// <param name="index">index de base 0 de la position de la Layer souhaitée</param>
        /// <returns></returns>
        public PerfLayer layerAt(int index) {
            return perfLayerList.ElementAt<PerfLayer>(index);
        }



        /// <summary>
        /// Sélectionne toutes les Layer de la Pile
        /// </summary>
        public void selectAll() {
            foreach (PerfLayer pl in perfLayerList) {
                pl.selected = true;
            }
        }


        /// <summary>
        /// Désélectionne toutes les Layer
        /// </summary>
        public void selectNone() {
            foreach (PerfLayer pl in perfLayerList) {
                pl.selected = false;
            }
        }



        /// <summary>
        /// Renvoie le nombre de Layer de performances sélectionnées dans la Pile
        /// </summary>
        /// <returns>Un entier représentant le nombre de layers sélectionnés dans la série</returns>
        public int selectedCount() {

            int selected = 0;

            foreach (PerfLayer pl in this.perfLayerList) {
                if (pl.selected == true)
                    selected++;
            }
            return selected;
        }



        /// <summary>
        /// Calcule une prédiction tridimensionnelle
        /// </summary>
        /// <param name="pointFactorValue">Facteur des Point de performances</param>
        /// <param name="serieFactorValue">Facteur des Serie de performances</param>
        /// <param name="layerFactorValue">Facteur des Layer de performances</param>
        /// <returns>Double, valeur prédite pour les facteurs passés en arguments</returns>
        /// <remarks>
        /// TODO Cette fonction doit générer les différentes exceptions qui caractérisent les cas de calculs
        /// impossibles, mais ce n'est pas à la Pile d'intervenir dans le traitement des exceptions
        /// y compris pour stocker les informations liées à ces exceptions.
        /// Il reste à revoir la structure des blocs try, une imbrication n'est pas judicieuse
        /// Le bloc try primaire est visiblement trop étendu
        /// </remarks>
        public double predict(double pointFactorValue, double serieFactorValue, double layerFactorValue) {

            // Serie locale permettant une interpolation polynomiale sur 3 Layer en proximité avec layerFactorValue
            PerfSerie ps = new PerfSerie();
            double output = double.NaN;
            double layerOutput = double.NaN;

            try {
                if (this.count == 1) {
                    // Une seule layer, donc layerFactorValue est inutile
                    output = layerAt(0).predict(pointFactorValue, serieFactorValue);
                }
                else {
                    // Plusieurs Layer à traiter
                    // Définition du domaine de calcul, si non défini au préalable
                    if (!ranged) { setRange(); }
                    // Test du domaine de calcul
                    if (!isInRange(layerFactorValue)) {
                        throw new ModelException(AeroCalc.E_LAYER_VALUE_OUT_OF_RANGE,
                                                   this.outputName, this.layerFactorName, layerFactorValue);
                    }
                    // Sélection des layers
                    selectLayers(layerFactorValue, 3);
                    // Calcul de la prédiction pour chaque layer sélectionnée
                    for (int count = 0; count < this.count; count++) {
                        if (layerAt(count).selected) {
                            layerOutput = layerAt(count).predict(pointFactorValue, serieFactorValue);
                            // Abonde la Serie locale
                            ps.add(new PerfPoint(layerAt(count).factorValue, layerOutput, false));
                        }
                    }
                    if (ps.count >= 1) {
                        //
                        // TODO, Attention, la fonction predict va travailler sur une création de données
                        // nécessaires à une interpolation sur plusieurs Layer. En cas d'exception, ce ne sont pas
                        // des données originales des modèles de performances.
                        // REVOIR l'imbrication des blocs try pour simplifier la structure du code
                        //
                        try {
                            output = ps.predict(layerFactorValue);
                        } catch (ModelException ee) {
                            ee.setFactor(layerFactorName, layerFactorValue);
                        }
                    }
                    else {
                        throw new ModelException(AeroCalc.E_VOID_SYSTEM,
                                                   this.outputName, this.layerFactorName, layerFactorValue);
                    }
                }
            } 
            catch (ModelException e) {
                output = double.NaN;
                if (e.modelName.Equals("")) {
                    e.setModelName(this.outputName);
                }
                switch (e.nature) {

                    case AeroCalc.E_POINT_VALUE_OUT_OF_RANGE: e.setFactor(pointFactorName, pointFactorValue);
                    break;

                    case AeroCalc.E_SERIE_VALUE_OUT_OF_RANGE: e.setFactor(serieFactorName, serieFactorValue);
                    break;

                    case AeroCalc.E_LAYER_VALUE_OUT_OF_RANGE: e.setFactor(layerFactorName, layerFactorValue);
                    break;

                }
                throw;
            }
            return output;
        }



        /// <summary>
        /// Défini le domaine de calcul de la Pile
        /// </summary>
        /// <param name="start">Borne inférieure</param>
        /// <param name="startType">Type de borne</param>
        /// <param name="end">Borne supérieure</param>
        /// <param name="endType">Type de borne</param>
        /// <returns>True, en cas de succès</returns>
        public bool setRange(double start, int startType, double end, int endType) {
            if (end < start) {
                return false;
            }
            this.startRange = start;
            this.endRange = end;
            if (startType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                this.startRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            }
            else {
                this.startRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            if (endType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                this.endRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            }
            else {
                this.endRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            this.ranged = true;
            return true;
        }



        /// <summary>
        /// Défini, par défaut, le domaine de calcul de la Pile
        /// </summary>
        /// <returns>True, en cas de succès</returns>
        public bool setRange() {
            if (this.count > 0) {
                return setRange(layerAt(0).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT,
                                layerAt(this.count - 1).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT);
            }
            else {
                return false;
            }
        }



        /// <summary>
        /// Vérifie si la valeur x est dans le range défini pour la Pile
        /// </summary>
        /// <param name="x">Valeur à tester</param>
        /// <returns>True si la valeur est située dans le range de la Pile, False dans le cas contraire
        /// </returns>
        public bool isInRange(double x) {
            if (x < startRange || (x == startRange && startRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)) {
                return false;
            }
            if (x > endRange || (x == endRange && endRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)) {
                return false;
            }
            return true;
        }



        /// <summary>
        /// Retourne une chaine ce caractères décrivant tous les champs de l'objet Pile
        /// </summary>
        /// <returns>Une chaine de caractères contenant la description de la Pile de points de performance</returns>
        public override string ToString() {

            String msg = "";
            msg = "DB Key            : " + this.dataBaseKey + "\n";
            msg += "Output            : " + this.outputName + "\n";
            msg += "Output unit       :" + this.outputUnitCode + "\n";
            msg += "Discret Factor    : " + this.discretName + " = " + this.discretValue + "\n";
            msg += "Point Factor Name : " + this.pointFactorName + "\n";
            msg += "Point Factor Unit :" + this.pointFactorUnitCode + "\n";
            msg += "Serie Factor Name : " + this.serieFactorName + "\n";
            msg += "Serie Factor Unit :" + this.serieFactorUnitCode + "\n";
            msg += "Layer Factor Name : " + this.layerFactorName + "\n";
            msg += "Layer Factor Unit : " + this.layerFactorUnitCode + "\n";
            msg += "Own Factor  value : " + this.factorValue;
            msg += "Range             : ";
            if (this.startRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                msg += "]";
            }
            else {
                msg += "[";
            }
            msg += this.startRange + ";" + this.endRange;
            if (this.endRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                msg += "[";
            }
            else {
                msg += "]";
            }
            msg += "\nSelected : ";
            if (selected) {
                msg += "YES";
            }
            else {
                msg += "NO";
            }
            msg += "\nLayers :\n";
            foreach (PerfLayer pl in perfLayerList) {
                msg += "Index = " + perfLayerList.IndexOf(pl) + "\n";
                msg += pl.ToString() + "\n";
            }
            return msg;
        }



        /*
         *  INTERFACE
         */

        /// <summary>
        /// Implémente l'interface IComparer
        /// Comparaison de PerfPile entre elles, sur la base de leur factorValue
        /// C'est la valeur de ce facteur qui permet de les classer
        /// </summary>
        /// <param name="pp1">Première PerfPile de la comparaison</param>
        /// <param name="pp2">Deuxième PerfPile de la comparaison</param>
        /// <returns></returns>
        /// <remarks>
        /// L'ajout du facteur discret n'est pas géré pour l'instant...
        /// Le factorValue d'une PerfPile n'a pas d'intérêt pour l'instant, car il représente une dimension
        /// supplémentaire non utilisée dans les données de perfomances manipulées jusqu'à présent.
        /// Il est placé à 1, par défaut.
        /// Pour utiliser cette dimension, il faudrait encapsuler les PerfPile dans un nouvel ensemble, ce qui
        /// n'est pas envisagé aujourd'hui.
        /// </remarks>
        /// 
        public int Compare(PerfPile pp1, PerfPile pp2) {
            if (pp1.factorValue < pp2.factorValue) {
                return -1;
            }
            if (pp1.factorValue > pp2.factorValue) {
                return 1;
            }
            return 0;
        }


        /*
            // Implement the generic CompareTo method with the Temperature 
            // class as the Type parameter. 
            //
            public int CompareTo(Temperature other)
            {
                // If other is not a valid object reference, this instance is greater.
                if (other == null) return 1;

                // The temperature comparison depends on the comparison of 
                // the underlying Double values. 
                return m_value.CompareTo(other.m_value);
            }
        */
        /// <summary>
        /// Implémente l'interface IComparable
        /// </summary>
        /// <param name="pp"></param>
        /// <returns></returns>
        public int CompareTo(PerfPile pp) {
            if (pp == null) return 1;
            return factorValue.CompareTo(pp.factorValue);
        }



        /// <summary>
        /// Implémente l'interface IEquatable
        /// Renvoie True si les deux PerfPile recouvrent le même data model, c'est à dire ont:
        /// Le même nom, le même nom de discret et la même valeur de discret
        /// </summary>
        /// <param name="pp">PerfPile à laquelle se comparer</param>
        /// <returns>True si les deux PerfPile ont le même nom</returns>
        public bool Equals(PerfPile pp) {
            if (pp == null) {
                return false;
            }
            return (this.outputName.Equals(pp.outputName) &&
                    this.discretName.Equals(pp.discretName) &&
                    this.discretValue == pp.discretValue);
        }



        /*
         *  METHODES
         */

        /// <summary>
        /// Renvoie un tableau des indexes des Layer de performances, classées par proximité 
        /// de leur facteur avec une valeur de référence passée en argument
        /// </summary>
        /// <param name="x">Valeur</param>
        /// <returns>Tableau de Int, null si aucune Layer n'est présente dans la Pile</returns>
        private int[] sortedClosestLayers(double x) {

            int[] sortedIndexes;

            if (this.count < 1) {
                return null;
            }
            // Cas général, la Pile contient des Layer
            //
            sortedIndexes = new int[this.count];
            int minDistIndex = -1;
            int upIndex = -1;
            int downIndex = -1;
            double dist = -1;
            double minDist = Double.MaxValue;
            double[] distances = new double[this.count];
            int[] classement = new int[this.count];

            // Calcul des distances et détermination du point de plus grande proximité
            for (int count = 0; count < this.count; count++) {
                distances[count] = layerAt(count).factorValue;
                dist = Math.Abs(distances[count]);
                classement[count] = -1;
                if (dist < minDist) {
                    // La distance associée au point en cours est la plus petite rencontrée jusqu'à présent
                    minDist = dist;
                    minDistIndex = count;
                }
            }
            // Enregistrement du point de plus grande proximité
            sortedIndexes[0] = minDistIndex;

            // Cas des extrémités
            if (minDistIndex == 0) {
                // Trivial, le classement est identique à l'index de tableau
                for (int count = 0; count < sortedIndexes.Length; count++) {
                    sortedIndexes[count] = count;
                }
                return sortedIndexes;
            }
            if (minDistIndex == this.count - 1) {
                // Trivial, le classement est inverse de l'index du tableau
                for (int count = 0; count < sortedIndexes.Length; count++) {
                    classement[count] = sortedIndexes.Length - 1 - count;
                }
                return sortedIndexes;
            }

            // Cas général, minDistIndex n'est pas en limite de tableau
            downIndex = minDistIndex - 1;
            upIndex = minDistIndex + 1;

            for (int count = 1; count < sortedIndexes.Length; count++) {
                if (downIndex < 0) {
                    // upIndex désigne le dernier point disponible vers la limite basse
                    sortedIndexes[count] = upIndex;
                    upIndex++;
                }
                else if (upIndex > distances.Length - 1) {
                    // downIndex désigne le seul point disponible
                    sortedIndexes[count] = downIndex;
                    downIndex--;
                }
                else {
                    if (Math.Abs(distances[downIndex]) > Math.Abs(distances[upIndex])) {
                        // upIndex désigne le point le plus proche
                        sortedIndexes[count] = upIndex;
                        upIndex++;
                    }
                    else {
                        // downIndex désigne le point le plus proche
                        sortedIndexes[count] = downIndex;
                        downIndex--;
                    }
                }
            }
            return sortedIndexes;
        }
        // Accesseur de test
        public int[] __testSortedClosestLayers(double x) {
            return sortedClosestLayers(x);
        }



        /// <summary>
        /// Sélectionne un nombre nb de Layer, ayant pour facteur un nombre proche de
        /// la valeur x
        /// </summary>
        /// <param name="x">Valeur de référence</param>
        /// <param name="nb">Nombre de layers à sélectionner</param>
        /// <returns>
        /// False, si aucune Layer n'a pu être sélectionnée. True dans le cas contraire
        /// </returns>
        private bool selectLayers(double x, int nb) {

            // Tri des Layer, dans l'ordre de proximité avec la valeur x
            int[] layers = sortedClosestLayers(x);
            // Déselection de toutes les Layer de la Pile
            selectNone();

            if (layers == null) {
                return false;
            }
            if (nb == 0) {
                return true;
            }
            if (layers.Length == 1 || nb == 1 || layerAt(layers[0]).factorValue == x) {
                layerAt(layers[0]).selected = true;
                return true;
            }
            if (layers.Length == 2 || nb == 2) {
                layerAt(layers[0]).selected = true;
                layerAt(layers[1]).selected = true;
                return true;
            }
            else {
                // Trois Layer minimum dans la Pile, il faut maintenant analyser les ruptures de linéarité (breakpoints)
                int count = 0;
                int selectCounter = 0;
                int upBreakpoint = layers.Length;
                int dnBreakPoint = -1;

                while (selectCounter < nb && count < layers.Length) {
                    if (layers[count] < dnBreakPoint || layers[count] > upBreakpoint) {
                        // La Layer ne peut pas être sélectionnée, on passe à la suivante

                    }
                    else {
                        // Sélection de la Layer
                        layerAt(layers[count]).selected = true;
                        selectCounter++;
                        if (layerAt(layers[count]).isBreak) {
                            if (layerAt(layers[count]).factorValue < x) {
                                dnBreakPoint = layers[count];
                            }
                            else {
                                upBreakpoint = layers[count];
                            }
                        }
                    }
                    count++;
                }
                return true;
            }
        }
        // Accesseur pour la classe de test unitaire
        public bool __testSelectSubPile(double x, int nb) {
            return selectLayers(x, nb);
        }

    }

} 