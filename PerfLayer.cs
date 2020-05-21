using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore {


    /// <summary>
    /// Classe de dimension 3 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un ensemble cohérent de série de performances de vol
    /// Par contraction, une layer de séries de points de performances de vol s'appelle 'layer de performance'
    /// </summary>
    public class PerfLayer : IComparer<PerfLayer> {

        // FIELDS ////////////////////////////////////////////////////////////////////////////////////


        // PROPERTIES ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clé de la layer dans une base de donnée
        /// </summary>
        public long dataBaseKey {
            get;
            set;
        }

        /// <summary>
        /// Nombre de séries dans la layer
        /// </summary>
        public int count {
            get {
                return perfSerieList.Count;
            }
        }

        /// <summary>
        /// Nom de la Layer (nom des résultats des prédictions réalisées avec cette surface)
        /// </summary>
        public String outputName {
            get;
            private set;
        }

        /// <summary>
        /// Unité de mesure de la prédiction
        /// </summary>
        public int outputUnitCode {
            get;
            private set;
        }

        /// <summary>
        /// ABANDONNE AU PROFIT D'UN TABLEAU DE DISCRETS AU NIVEAU DE LA PILE
        /// Facteur discret associé à la Layer
        /// </summary>
        /// <remarks>
        /// Permet d'ajouter une valeur discrète, par exemple un braquage des volets, l'état de surface de la piste...
        /// Cette valeur entière conduit à construire des familles de Layer dans une même Pile.
        /// Les calculs ne doivent être réalisé que dans une même famille, c'est à dire avec des Layer de même discret
        /// </remarks>
        /*
        public long discretValue {
            get;
            private set;
        }
        */

        /// <summary>
        /// Valeur du facteur associé à la Layer
        /// </summary>
        /// <remarks>Dim 3</remarks>
        public double factorValue {
            get;
            private set;
        }

        /// <summary>
        /// Nom de la deuxième dimension (séries de performanes)
        /// </summary>
        /// <remarks>Dim 2</remarks>
        public String serieFactorName {
            get;
            private set;
        }

        /// <summary>
        /// Unité de mesure de la deuxième dimension, utilisée pour les calculs de prédiction
        /// </summary>
        public int serieFactorUnitCode {
            get;
            private set;
        }

        /// <summary>
        /// Nom de la première dimension (points de performances)
        /// </summary>
        /// <remarks>Dim 1</remarks>
        public String pointFactorName {
            get;
            private set;
        }

        /// <summary>
        /// Unité de mesure de la première dimension, utilisée pour les calculs de prédiction
        /// </summary>
        public int pointFactorUnitCode {
            get;
            private set;
        }

        /// <summary>
        /// Indique si le range de la série a été défini
        /// </summary>
        public bool ranged {
            get;
            private set;
        }

        /// <summary>
        /// Etat de sélection de la Layer. La sélection permet de ne prendre en compte
        /// que certaines Layer pour les calculs de prédiction
        /// </summary>
        public bool selected {
            get;
            set;
        }

        /// <summary>
        /// Etat 'breakpoint' de la layer de points de performance (rupture de linéarité)
        /// </summary>
        public bool isBreak {
            get;
            private set;
        }


        // Private fields ///////////////////////////////////////////////////////////////////////////////////
        
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
        /// Liste générique utilisée pour ordonner les séries de performances
        /// </summary>
        List<PerfSerie> perfSerieList;


        // Constructor(s) ///////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Construction d'une Layer vide de toute série et non paramétrée
        /// </summary>
        public PerfLayer() {
            perfSerieList = new List<PerfSerie>();
        }


        /// <summary>
        /// Construction par clonage d'une Layer de séries de performance
        /// </summary>
        /// <param name="pl">Layer de séries de performance à cloner</param>
        public PerfLayer(PerfLayer pl) {
            this.dataBaseKey = pl.dataBaseKey;
            this.endRange = pl.endRange;
            this.endRangeType = pl.endRangeType;
            this.outputName = pl.outputName;
            this.outputUnitCode = pl.outputUnitCode;
            this.pointFactorName = pl.pointFactorName;
            this.pointFactorUnitCode = pl.pointFactorUnitCode;
            this.ranged = pl.ranged;
            this.serieFactorName = pl.serieFactorName;
            this.serieFactorUnitCode = pl.serieFactorUnitCode;
            this.startRange = pl.startRange;
            this.startRangeType = pl.startRangeType;
            for (int count = 0; count < pl.count; count++) {
                this.add(new PerfSerie(pl.perfSerieList.ElementAt(count)));
            }
        }


        /// <summary>
        /// Construit une Layer vide de toute Serie de performances
        /// </summary>
        /// <param name="ownFactorValue">Valeur du facteur associé à la Layer (dimension 2)</param>
        /// <param name="pointFactorName">Nom de la dimension des Point de la Pile (dimension 1)</param>
        /// <param name="pointFactorUnitCode">Unité de mesure de la dimension des points</param>
        /// <param name="serieFactorName">Nom de la dimension des Serie de la Pile (dimension 2)</param>
        /// <param name="serieFactorUnitCode">Unité de mesure de la dimension des Serie</param>
        /// <param name="outName">Nom de la Layer (nom des résultat des prédictions réalisées avec cette Layer)</param>
        /// <param name="outUnitCode">Unité de mesure de la prédiction</param>
        public PerfLayer(double ownFactorValue,
                         String pointFactorName, int pointFactorUnitCode,
                         String serieFactorName, int serieFactorUnitCode,
                         String outName, int outUnitCode) {
            this.factorValue = ownFactorValue;
            this.pointFactorName = pointFactorName;
            this.pointFactorUnitCode = pointFactorUnitCode;
            this.serieFactorName = serieFactorName;
            this.serieFactorUnitCode = serieFactorUnitCode;
            this.outputName = outName;
            this.outputUnitCode = outUnitCode;
            this.perfSerieList = new List<PerfSerie>();
        }


        // Services /////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Ajoute une Serie de performances à la Layer
        /// </summary>
        /// <param name="newPerfSerie">Nouvelle Serie à ajouter dans la Layer</param>
        /// <returns>True si l'ajout est réussi, False dans le cas contaire</returns>
        public bool add(PerfSerie newPerfSerie) {
            foreach (PerfSerie p in perfSerieList) {
                if (p.Compare(p, newPerfSerie) == 0) {
                    return false;
                }
            }
            // newPerfSerie est bien une nouvelle série de layers de performance de vol
            perfSerieList.Add(newPerfSerie);
            perfSerieList.Sort(newPerfSerie);
            /// <!-- DEBUG
            /// Lors de l'ajout de la première Serie, on récupère les infos de la dimension inférieure
            /// pointFactorName = newPerfSerie.getPrimaryDimension();
            /// pointFactorUnitCode = newPerfSerie.getPrimaryDimensionUnitCode();
            /// -->
            // Le domaine de calcul n'est plus valable
            ranged = false;
            return true;
        }


        /// <summary>
        /// Ajoute un Point de performances de vol à une Serie de la layer
        /// Si la Serie n'existe pas, une Serie est créée pour accueillir le Point
        /// </summary>
        /// <param name="newPerfPoint">Nouveau Point de performances de vol</param>
        /// <param name="serieFactorValue">Facteur de la Serie dans laquelle inscrire le Point</param>
        /// <returns>True si l'ajout est réussi, False dans le cas contaire</returns>
        public bool add(PerfPoint newPerfPoint, double serieFactorValue) {
            
            bool foundIt = false;
            bool success = false;

            // Recherche de la Serie ayant un facteur identique
            for (int count = 0; count < perfSerieList.Count; count++) {
                if (serieFactorValue == SerieAt(count).factorValue) {
                    success = SerieAt(count).add(newPerfPoint);
                    foundIt = true;
                    break;
                }
            }
            // Pas de Serie qui convienne, on la crée
            if (!foundIt) {
                PerfSerie ps = new PerfSerie(serieFactorValue);
                success=ps.add(newPerfPoint);
                if (success) {
                    success = add(ps);
                }
            }
            return success;
        }


        /// <summary>
        /// Renvoie le nombre de Serie de performances sélectionnées dans la Layer
        /// </summary>
        /// <returns>Un entier représentant le nombre de séries sélectionnées</returns>
        public int selectedCount() {

            int selectedSeries = 0;

            foreach (PerfSerie ps in this.perfSerieList) {
                if (ps.selected)
                    selectedSeries++;
            }
            return selectedSeries;
        }


        /// <summary>
        /// Renvoie la Serie de performances ayant l'index fourni en argument
        /// </summary>
        /// <param name="index">Index de la Serie de performances souhaitée</param>
        /// <returns>La Serie de performances située à la position index dans la Layer</returns>
        public PerfSerie SerieAt(int index) {
            return perfSerieList.ElementAt<PerfSerie>(index);
        }


        /// <summary>
        /// Sélectionne toutes les Serie de la Layer
        /// </summary>
        public void selectAll() {
            foreach (PerfSerie ps in perfSerieList) {
                ps.selected = true;
            }
        }


        /// <summary>
        /// Désélectionne toutes les Serie de la Layer
        /// </summary>
        public void selectNone() {
            foreach (PerfSerie ps in perfSerieList) {
                ps.selected = false;
            }
        }


        /// <summary>
        /// Vérifie si un valeur passée en argument est dans le range de la Layer
        /// </summary>
        /// <param name="x">Valeur à tester</param>
        /// <returns>True si la valeur est située dans le range de la Layer, False dans le cas contraire
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
        /// Retourne une chaine de caractère présentant toutes les caractéristiques de la Layer de performances
        /// </summary>
        /// <returns>String, descriptive de la Layer</returns>
        public override String ToString() {
            String msg = "";
            msg = "DB Key            : " + this.dataBaseKey + "\n";
            msg += "Output            : " + this.outputName + "\n";
            msg += "Output unit       :" + this.outputUnitCode + "\n";
            msg += "Point Factor Name : " + this.pointFactorName + "\n";
            msg += "Point Factor Unit :" + this.pointFactorUnitCode + "\n";
            msg += "Serie Factor Name : " + this.serieFactorName + "\n";
            msg += "Serie Factor Unit :" + this.serieFactorUnitCode + "\n";
            msg += "factor value : " + this.factorValue + "\n";
            msg += "Range : ";
            if (this.startRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                msg += "]";
            } else {
                msg += "[";
            }
            msg += this.startRange + ";" + this.endRange;
            if (this.endRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                msg += "[";
            } else {
                msg += "]";
            }
            msg += "\nSelected : ";
            if (selected) {
                msg += "YES";
            } else {
                msg += "NO";
            }
            msg += "\nSéries :\n";
            foreach (PerfSerie ps in perfSerieList) {
                msg += "Index = " + perfSerieList.IndexOf(ps) + "\n";
                msg += ps.ToString() + "\n";
            }
            return msg;
        }


        /// <summary>
        /// Calcule une prédiction pour deux facteurs
        /// </summary>
        /// <param name="pointFactorValue">Facteur lié aux layers de performance</param>
        /// <param name="serieFactorValue">Facteur lié aux séries de layers de performance</param>
        /// <returns></returns>
        public double predict(double pointFactorValue, double serieFactorValue) {

            PerfSerie ps = new PerfSerie();
            double output = double.NaN;
            double serieOutput = double.NaN;

            try {
                if (this.count == 1) {
                    // Il n'y a qu'une série dans la layer, donc serieFactorValue n'est pas utile
                    output = SerieAt(0).predict(pointFactorValue);
                } else {
                    // Si le domaine de calcul n'a pas été défini au préalable, il est réduit à l'étendue
                    // de la layer
                    if (!ranged) {
                        setRange();
                    }
                    // Test du domaine de calcul
                    if (!isInRange(serieFactorValue)) {
                        throw new ModelException(AeroCalc.E_SERIE_VALUE_OUT_OF_RANGE, 
                                                   this.outputName, "", serieFactorValue);
                    }
                    // Sélection des séries
                    selectSubLayer(serieFactorValue, 3);
                    // Calcul de la prédiction pour chaque série sélectionnée
                    for (int count = 0; count < this.count; count++) {
                        if (SerieAt(count).selected) {
                            serieOutput = SerieAt(count).predict(pointFactorValue);
                            ps.add(new PerfPoint(SerieAt(count).factorValue, serieOutput, false));
                        }
                    }
                    if (ps.count >= 1) {
                        output = ps.predict(serieFactorValue);
                    }
                }
            } catch (ModelException e) {
                throw e;
            }
            return output;
        }


        /// <summary>
        /// Définie le domaine de calcul de la série
        /// </summary>
        /// <param name="start">Borne inférieure</param>
        /// <param name="startType">Type de borne</param>
        /// <param name="end">Borne supérieure</param>
        /// <param name="endType">Type de borne</param>
        /// <returns>True en cas de succès</returns>
        public bool setRange(double start, int startType, double end, int endType) {
            if (end < start) {
                return false;
            }
            startRange = start;
            endRange = end;
            if (startType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                startRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            } else {
                startRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            if (endType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT) {
                endRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            } else {
                endRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            ranged = true;
            return true;
        }


        /// <summary>
        /// Définie le domaine de calcul de la série
        /// </summary>
        /// <returns>True en cas de succès</returns>
        public bool setRange() {
            if (this.count > 0) {
                return setRange(SerieAt(0).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT,
                                SerieAt(perfSerieList.Count - 1).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT);
            } else
                return false;
        }


        // Interface(s) /////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Comparaison entre elles des Layer sur la base de la valeur de leur facteur associé (dimension 3)
        /// </summary>
        /// <param name="pl1">Première Layer de performance</param>
        /// <param name="pl2">Deuxième Layer de performance</param>
        /// <returns>-1 si ps1 est avant ps2, O si ps1 = ps2, 1 si ps1 est plus grand que ps2</returns>
        public int Compare(PerfLayer pl1, PerfLayer pl2) {
            if (pl1.factorValue < pl2.factorValue) {
                return -1;
            }
            if (pl1.factorValue > pl2.factorValue) {
                return 1;
            }
            return 0;
        }


        // Methods //////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Renvoie un tableau des indexes des séries de layers de performance, classé par proximité 
        /// de leur facteur avec une abscisse de référence
        /// </summary>
        /// <param name="pp1">Abscisse de référence</param>
        /// <returns>Tableau de Int, null si aucun point n'est présent dans la série</returns>
        private int[] sortedClosestSeries(double x) {

            int[] sortedIndexes;

            if (perfSerieList.Count < 1) {
                return null;
            }

            // Cas général, la Layer contient des séries
            //
            sortedIndexes = new int[perfSerieList.Count];
            int minDistIndex = -1;
            int upIndex = -1;
            int downIndex = -1;
            double dist = -1;
            double minDist = Double.MaxValue;
            double[] distances = new double[perfSerieList.Count];
            int[] classement = new int[perfSerieList.Count];

            // Calcul des distances et détermination du point de plus grande proximité
            for (int count = 0; count < this.count; count++) {
                distances[count] = SerieAt(count).factorValue - x;
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
            if (minDistIndex == perfSerieList.Count - 1) {
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
                } else if (upIndex > distances.Length - 1) {
                    // downIndex désigne le seul point disponible
                    sortedIndexes[count] = downIndex;
                    downIndex--;
                } else {
                    if (Math.Abs(distances[downIndex]) > Math.Abs(distances[upIndex])) {
                        // upIndex désigne le point le plus proche
                        sortedIndexes[count] = upIndex;
                        upIndex++;
                    } else {
                        // downIndex désigne le point le plus proche
                        sortedIndexes[count] = downIndex;
                        downIndex--;
                    }
                }
            }
            return sortedIndexes;
        }
        // Accesseur de test
        public int[] testSortedClosestSeries(double x) {
            return sortedClosestSeries(x);
        }


        /// <summary>
        /// Sélectionne un nombre nb de séries liées de façon continue autour d'une valeur de facteur x
        /// </summary>
        /// <param name="x">Abscisse de référence</param>
        /// <param name="nb">Nombre de layers à sélectionner</param>
        private bool selectSubLayer(double x, int nb) {

            int[] series = sortedClosestSeries(x);
            selectNone();

            if (series == null) {
                return false;
            }
            if (nb == 0) {
                return true;
            }
            if (series.Length == 1 || nb == 1) {
                SerieAt(series[0]).selected = true;
                return true;
            }
            if (series.Length == 2 || nb == 2) {
                SerieAt(series[0]).selected = true;
                SerieAt(series[1]).selected = true;
                return true;
            } else {
                // Trois séries minimum dans la layer, il faut maintenant considérer les breakpoints
                int count = 0;
                int selectCounter = 0;
                int upBreakSerie = series.Length;
                int dnBreakSerie = -1;

                while (selectCounter < nb && count < series.Length) {
                    if (series[count] < dnBreakSerie || series[count] > upBreakSerie) {
                        // Le point ne peut pas être sélectionné, on passe au suivant

                    } else {
                        // Sélection de la série
                        SerieAt(series[count]).selected = true;
                        selectCounter++;
                        if (SerieAt(series[count]).isBreak) {
                            if (SerieAt(series[count]).factorValue < x) {
                                dnBreakSerie = series[count];
                            } else {
                                upBreakSerie = series[count];
                            }
                        }
                    }
                    count++;
                }
                return true;
            }
        }
        // Accesseur pour la classe de test unitaire
        public bool testSelectSubLayer(double x, int nb) {
            return selectSubLayer(x, nb);
        }

    }
    
}