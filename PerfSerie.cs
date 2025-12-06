using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe de dimension 2 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un ensemble cohérent de allPoints de performances de vol
    /// Par contraction, une série de allPoints de performance s'appelle 'série de performance'
    /// </summary>
    /// 
    public class PerfSerie : IComparer<PerfSerie>
    {

        // FIELDS ////////////////////////////////////////////////////////////////////////////////////


        // PROPERTIES ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clé de la série dans une base de donnée
        /// </summary>
        public long dataBaseKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Valeur du facteur associé à la série
        /// </summary>
        /// <remarks>Dim 2</remarks>
        public double factorValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Indique si le range de la série a été défini
        /// </summary>
        public bool ranged
        {
            get;
            private set;
        }

        /// <summary>
        /// Etat 'breakpoint' de la série de allPoints de performance (rupture de linéarité)
        /// </summary>
        public bool isBreak
        {
            get;
            private set;
        }

        /// <summary>
        /// Borne inférieure du domaine de calcul
        /// </summary>
        public double startRange { get; private set; }

        /// <summary>
        /// Nature de la borne inférieure du domaine de calcul
        /// </summary>
        public int startRangeType { get; private set; }

        /// <summary>
        /// Borne supérieure du domaine de calcul
        /// </summary>
        public double endRange { get; private set; }

        /// <summary>
        /// Nature de la borne supérieure du domaine de calcul
        /// </summary>
        public int endRangeType { get; private set; }

        /// <summary>
        /// Flag de sélection de la série. La sélection permet de ne prendre en compte
        /// que certaines séries pour les calculs de prédiction
        /// </summary>
        /// <remarks>
        /// TODO: Problématique en multithreading, à revoir
        /// Eviter une mutation de l'objet lors d'un calcul
        /// Il est recommandé de confier l'interpolation à un objet externe, qui pourra adapter la méthode de calcul à la situation
        /// </remarks>
        
        
        public bool selected
        {
            get;
            set;
        }
        

        /// <summary>
        /// Nombre de points de performance dans la série
        /// </summary>
        public int count
        {
            get
            {
                return perfPointList.Count;
            }
        }


        // PRIVATE FIELDS ///////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Liste générique utilisée pour ordonner les points de performance PerfPoint de la série
        /// </summary>
        List<PerfPoint> perfPointList;


        // CONSTRUCTOR(S) ///////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Construction d'une série vide de tout point et non paramétrée
        /// </summary>
        public PerfSerie()
        {
            this.perfPointList = new List<PerfPoint>();
        }


        /// <summary>
        /// Construction par clonage d'une série de points de performance
        /// </summary>
        /// <param name="ps">Série de points de performance à cloner</param>
        public PerfSerie(PerfSerie ps)
        {
            if (ps!=null)
            {
                this.dataBaseKey = ps.dataBaseKey;
                this.endRange = ps.endRange;
                this.endRangeType = ps.endRangeType;
                this.factorValue = ps.factorValue;
                this.ranged = ps.ranged;
                this.startRange = ps.startRange;
                this.startRangeType = ps.startRangeType;
                this.perfPointList = new List<PerfPoint>(ps.perfPointList);
                // Tri de la nouvelle série
                perfPointList.Sort();
                
            }
        }


        /// <summary>
        /// Construction d'une série de points de performance
        /// </summary>
        /// <param name="factorValue">Valeur du facteur associé à la série (paramètre de la série)</param>
        /// <param name="pointFactorName">Nom du facteur d'entrée pour calculer une prédiction</param>
        /// <param name="pointFactorUnitCode">Unité de mesure du facteur d'entrée</param>
        /// <param name="outName">Nom de la série (des résultat des prédictions réalisées avec cette série)</param>
        /// <param name="outUnitCode">Unité de mesure de la prédiction</param>
        public PerfSerie(double factorValue)
        {
            this.factorValue = factorValue;
            this.perfPointList = new List<PerfPoint>();
        }



        public PerfSerie(long dataBaseKey, double factorValue, bool isBreak)
        {
            this.dataBaseKey = dataBaseKey;
            this.factorValue = factorValue;
            this.isBreak = isBreak;

            // Table des allPoints de performance, initialisée vide
            this.perfPointList = new List<PerfPoint>();

        }






        // SERVICES /////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Ajoute un point de performance à la série, et réalise le tri de la série
        /// </summary>
        /// <param name="pp">Point de perforance à ajouter</param>
        /// <returns>True si l'opération a réussi, False en cas d'échec, typiquement quand on essaye d'ajouter 
        /// un point de performance ayant la même abscisse qu'un point déjà présent dans la série.
        /// </returns>
        ///
        public bool add(PerfPoint pp)
        {
            foreach (PerfPoint p in perfPointList)
            {
                if (p.CompareTo(pp) == 0)
                {
                    return false;
                }
            }
            // pp est bien un nouveau point de performance de vol
            perfPointList.Add(pp);
            perfPointList.Sort();

            // Mise à jour du domaine de la série
            

            /*
            // Le domaine de la série n'est plus valable
            startRange = this.pointAt(0).factorValue;
            endRange = this.pointAt(perfPointList.Count - 1).factorValue;
            ranged = false;
            */
            return true;
        }


        /// <summary>
        /// Renvoie le nombre de allPoints de performances sélectionnés dans la série
        /// </summary>
        /// <returns>Un entier représentant le nombre de allPoints sélectionnés dans la série</returns>
        ///
        /*
         * public int selectedCount()
        {
            int selPtsNb = 0;

            foreach (PerfPoint pp in this.perfPointList)
            {
                if (pp.selected == true)
                    selPtsNb++;
            }
            return selPtsNb;
        }
        */


        /// <summary>
        /// Renvoie le point de performance ayant l'index Index
        /// </summary>
        /// <param name="index">Index du point de performance souhaité</param>
        /// <returns>Le point de performance situé à la position index dans la série</returns>
        ///
        public PerfPoint pointAt(int index)
        {
            return perfPointList.ElementAt<PerfPoint>(index);
        }


        /// <summary>
        /// Renvoie l'index d'un point de la série
        /// </summary>
        /// <param name="pp">Point de performance dont on souhaite l'index</param>
        /// <returns>Index du point de performance passé en paramètre</returns>
        ///
        public int getIndexOf(PerfPoint pp)
        {
            return perfPointList.IndexOf(pp);
        }


        /// <summary>
        /// Sélectionne tous les allPoints de la série
        /// </summary>
        ///
        /*
        public void selectAll()
        {
            foreach (PerfPoint pp in perfPointList)
            {
                pp.selected = true;
            }
        }
        */



        /// <summary>
        /// Désélectionne tous les allPoints de la série
        /// </summary>
        ///
        /*
        public void selectNone()
        {
            foreach (PerfPoint pp in perfPointList)
            {
                pp.selected = false;
            }
        }
        */


        /// <summary>
        /// Vérifie si x est dans le range défini pour la série
        /// </summary>
        /// <param name="x">Abscisse de référence</param>
        /// <returns>True si l'abscisse de référence est située dans le range de la série</returns>
        ///
        public bool isInRange(double x)
        {
            if (x < this.startRange || (x == this.startRange && this.startRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT))
            {
                return false;
            }
            if (x > this.endRange || (x == this.endRange && this.endRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Prédiction d'une valeur
        /// </summary>
        /// <param name="inputValue">Abscisse de référence</param>
        /// <returns>
        /// Valeur prédite, null si le calcul est impossible
        /// </returns>
        ///
        public double predict(double inputValue)
        {
            PerformanceModelSolver solver = new PerformanceModelSolver(this);

            //Si le domaine de calcul n'a pas été défini au préalable, il est réduit à l'étendue de la série
            if (!ranged)
            {
                setRange();
            }

            // Test du domaine de calcul
            if (!isInRange(inputValue))
            {
                throw new ModelException(AeroCalc.E_POINT_VALUE_OUT_OF_RANGE, "", "", double.NaN);
            }

            // Sélection des points de performance d'intérêt
            int[] selectedPoints = sortedClosestPointsIndexes(inputValue);

            //selectPoints(inputValue, 3);
            try
            {
                return solver.interpolateLagrange(inputValue);
            }
            catch (ModelException)
            {
                throw;
            }
        }


        /// <summary>
        /// Retourne une chaine de caractère présentant toutes les caractéristiques de la Serie de performances
        /// </summary>
        /// <returns>String, descriptive de la Serie</returns>
        ///
        public override String ToString()
        {
            String msg = "";
            msg = "DB Key : " + this.dataBaseKey + "\n";
            msg += "factor value : " + this.factorValue + "\n";
            msg += "Range : ";
            if (this.startRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)
            {
                msg += "]";
            }
            else
            {
                msg += "[";
            }
            msg += this.startRange + ";" + this.endRange;
            if (this.endRangeType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)
            {
                msg += "[";
            }
            else
            {
                msg += "]";
            }
            msg += "\n";
            msg += "Points :\n";
            foreach (PerfPoint pp in perfPointList)
            {
                msg += "Index = " + perfPointList.IndexOf(pp) + "\n";
                msg += pp.ToString() + "\n";
            }
            return msg;
        }


        // INTERFACE(S) /////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Comparaison des séries entre elles par la valeur de leur facteur associé (secondary dimension)
        /// C'est la valeur de ce facteur qui permet de les classer
        /// </summary>
        /// <param name="ps1">Première série de layers de performance</param>
        /// <param name="ps2">Deuxième série de layers de performance</param>
        /// <returns>-1 si ps1 est avant ps2, O si ps1 = ps2, 1 si ps1 est plus grand que ps2</returns>
        ///
        public int Compare(PerfSerie ps1, PerfSerie ps2)
        {
            if (ps1.factorValue < ps2.factorValue)
            {
                return -1;
            }
            if (ps1.factorValue > ps2.factorValue)
            {
                return 1;
            }
            return 0;
        }


        // SETTERS //////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Défini le domaine de calcul de la série
        /// </summary>
        /// <param name="start">Borne inférieure</param>
        /// <param name="startType">Type de borne</param>
        /// <param name="end">Borne supérieure</param>
        /// <param name="endType">Type de borne</param>
        /// <returns>True en cas de succès</returns>
        ///
        public bool setRange(double start, int startType, double end, int endType)
        {
            if (end < start)
            {
                return false;
            }
            startRange = start;
            endRange = end;
            if (startType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)
            {
                startRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            }
            else
            {
                startRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            if (endType == AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT)
            {
                endRangeType = AeroCalc.MODEL_RANGE_EXCLUDE_LIMIT;
            }
            else
            {
                endRangeType = AeroCalc.MODEL_RANGE_INCLUDE_LIMIT;
            }
            ranged = true;
            return true;
        }



        /// <summary>
        /// Défini par défaut le domaine de calcul de la série. C'est à dire l'étendue des allPoints de performance, en incluant
        /// les limites dans le domaine de calcul
        /// </summary>
        /// <returns>True en cas de succès</returns>
        ///
        public bool setRange()
        {
            if (this.count > 0)
            {
                return setRange(pointAt(0).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT,
                                pointAt(perfPointList.Count - 1).factorValue, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT);
            }
            else return false;
        }



        // METHODS //////////////////////////////////////////////////////////////////////////////////////////



        private double[] distances(double x)
        {
            // Au minimum, il faut un point de performance dans la série
            if (perfPointList.Count < 1) { return null; }
            
            double[] distances = new double[perfPointList.Count - 1];

            foreach (PerfPoint pp in perfPointList)
            {
                distances[perfPointList.IndexOf(pp)] = Math.Abs(x - pp.factorValue);
            }
            return distances;
        }



        private int[] orderedClosestPoints(double x)
        {     
            double[] distTable = distances(x);
            if (distTable == null) { return null; }

            int[] orderedIndexes = new int[distTable.Length];
            bool[] indexUsed = new bool[distTable.Length];
            for (int count = 0; count < distTable.Length; count++)
            {
                double minDist = Double.MaxValue;
                int minIndex = -1;
                for (int index = 0; index < distTable.Length; index++)
                {
                    if (!indexUsed[index])
                    {
                        if (distTable[index] < minDist)
                        {
                            minDist = distTable[index];
                            minIndex = index;
                        }
                    }
                }
                orderedIndexes[count] = minIndex;
                indexUsed[minIndex] = true;
            }
            return orderedIndexes;
        }


        /// <summary>
        /// Renvoie un tableau des index des allPoints de performance, dont l'abscisse voisine avec x
        /// et s'assurant de l'encadrement autour de la valeur x, si existant
        /// </summary>
        /// <param name="x">Absisse de la cible</param>
        /// <returns></returns>
        private int[] closestPointsAround(double x)
        {
            int[] orderedIndexes = orderedClosestPoints(x);
            int[] cpa = new int[orderedIndexes.Length];

            if (orderedIndexes == null) { return null; }
            List<int> closestPoints = new List<int>();

            closestPoints.Add(orderedIndexes[0]);
            for (int count = 1; count < orderedIndexes.Length; count++)
            {
                int index = orderedIndexes[count];
                if (pointAt(index).factorValue < x)
                {
                    // Point en dessous de x
                    if (closestPoints.Min() > index)
                    {
                        closestPoints.Insert(0, index);
                    }
                }
                else if (pointAt(index).factorValue > x)
                {
                    // Point au dessus de x
                    if (closestPoints.Max() < index)
                    {
                        closestPoints.Add(index);
                    }
                }
                else
                {
                    // Point exactement à x, on l'a déjà ajouté
                }
            }
            return closestPoints.ToArray();
        }






        /// <summary>
        /// Renvoie un tableau des index des allPoints de performance, classés par ordre de proximité avec une valeur x.
        /// </summary>
        /// <param name="x">Valeur</param>
        /// <returns>Tableau de Int, null si aucun point n'est présent dans la série</returns>
        ///
        private int[] sortedClosestPointsIndexes(double x)
        {

            int[] sortedIndexes;

            if (perfPointList.Count < 1)
            {
                return null;
            }

            // Cas général, la série contient des allPoints de performance
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
            for (int count = 0; count < this.count; count++)
            {
                distances[count] = pointAt(count).factorValue - x;
                dist = Math.Abs(distances[count]);
                classement[count] = -1;
                if (dist < minDist)
                {
                    // La distance associée au point en cours est la plus petite rencontrée jusqu'à présent
                    minDist = dist;
                    minDistIndex = count;
                }
            }
            // Enregistrement du point de plus grande proximité
            sortedIndexes[0] = minDistIndex;

            // Cas des extrémités
            if (minDistIndex == 0)
            {
                // Trivial, le classement est identique à l'index de tableau
                for (int count = 0; count < sortedIndexes.Length; count++)
                {
                    sortedIndexes[count] = count;
                }
                return sortedIndexes;
            }
            if (minDistIndex == perfPointList.Count - 1)
            {
                // Trivial, le classement est inverse de l'index du tableau
                for (int count = 0; count < sortedIndexes.Length; count++)
                {
                    classement[count] = sortedIndexes.Length - 1 - count;
                }
                return sortedIndexes;
            }

            // Cas général, minDistIndex n'est pas en limite de tableau
            downIndex = minDistIndex - 1;
            upIndex = minDistIndex + 1;

            for (int count = 1; count < sortedIndexes.Length; count++)
            {
                if (downIndex < 0)
                {
                    // upIndex désigne le dernier point disponible vers la limite basse
                    sortedIndexes[count] = upIndex;
                    upIndex++;
                }
                else if (upIndex > distances.Length - 1)
                {
                    // downIndex désigne le seul point disponible
                    sortedIndexes[count] = downIndex;
                    downIndex--;
                }
                else
                {
                    if (Math.Abs(distances[downIndex]) > Math.Abs(distances[upIndex]))
                    {
                        // upIndex désigne le point le plus proche
                        sortedIndexes[count] = upIndex;
                        upIndex++;
                    }
                    else
                    {
                        // downIndex désigne le point le plus proche
                        sortedIndexes[count] = downIndex;
                        downIndex--;
                    }
                }
            }
            return sortedIndexes;
        }
        // Accesseur de test
        public int[] _A_sortedClosestPointsIndexes(double x)
        {
            return sortedClosestPointsIndexes(x);
        }



        /// <summary>
        /// Sélectionne les allPoints de performance liés de façon continue autour d'une abscisse pp1
        /// </summary>
        /// <param name="x">Abscisse de référence</param>
        /// <param name="nb">Nombre de allPoints de performance à sélectionner</param>
        ///
        private int[] selectPoints(double x, int nb)
        {

            int[] allPoints = sortedClosestPointsIndexes(x);

            if (allPoints == null || nb < 2) { return null; }

            // Construction tableau des index de points de performance d'intérêt
            int[] poi = new int[nb];

            // Cas triviaux, un ou deux points de performance seulement dans la série
            if (allPoints.Length == 1 || nb == 1)
            {
                poi[0] = allPoints[0];
                for (int i = 1; i < nb; i++)
                {
                    poi[i] = -1;
                }
                return poi;
            }
            if (allPoints.Length == 2)
            {
                poi[0]= allPoints[0];
                poi[1]= allPoints[1];
                for (int i = 2; i < nb; i++)
                {
                    poi[i] = -1;
                }
                return poi;
            }

            // Trois PerfPoints minimum dans la série, il faut maintenant considérer les breakpoints
            int count = 0;
            int selectCounter = 0;
            int upBreakpoint = allPoints.Length;
            int dnBreakPoint = -1;

            while (selectCounter < nb && count < allPoints.Length)
            {
                if (allPoints[count] < dnBreakPoint || allPoints[count] > upBreakpoint)
                {
                    // Le point ne peut pas être sélectionné, on passe au suivant

                }
                else
                {
                    // Sélection du point
                    poi[selectCounter] = allPoints[count];
                    selectCounter++;
                    if (pointAt(allPoints[count]).isBreak)
                    {
                        if (pointAt(allPoints[count]).factorValue < x)
                        {
                            dnBreakPoint = allPoints[count];
                        }
                        else
                        {
                            upBreakpoint = allPoints[count];
                        }
                    }
                }
                count++;
            }
            return poi;
        }
        // Accesseur pour la classe de test unitaire
        public int[] _A_selectPoints(double x, int nb)
        {
            return selectPoints(x, nb);
        }

    }

}