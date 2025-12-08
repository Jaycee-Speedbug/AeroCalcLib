using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore
{

    /// <summary>
    /// Classe de dimension 2 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un ensemble cohérent de points de performance de vol
    /// Le RANGE de la série peut être défini pour limiter le domaine de calcul des interpolations ou permettre
    /// des extrapolations contrôlées.
    /// </summary>
    /// 
    public class PerfSerie : IComparer<PerfSerie>
    {

        // FIELDS ////////////////////////////////////////////////////////////////////////////////////

        // PROPERTIES ////////////////////////////////////////////////////////////////////////////////

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
        /// Etat 'breakpoint' de la série de points de performance (rupture de linéarité)
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

            // Table des points de performance, initialisée vide
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
            // Tri de la série après ajout
            perfPointList.Sort();
            // Mise à jour du domaine de la série : tout point ajouté modifie potentiellement le domaine, et entraine donc
            // un reset sur les borne de la série, incluses.
            setRange();

            return true;
        }



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

            //Si le domaine de calcul n'a pas été défini au préalable, il est réduit à l'étendue de la série
            if (!ranged)
            {
                setRange();
            }

            // Test du domaine de calcul, devra évoluer pour permettre des extrapolations contrôlées
            if (!isInRange(inputValue))
            {
                throw new ModelException(AeroCalc.E_POINT_VALUE_OUT_OF_RANGE, "", "", double.NaN);
            }

            // Sélection des points de performance d'intérêt
            int[] selectedPoints = closestPointsAround(inputValue,3);
            PerfSerie tempoSerie = new PerfSerie();
            foreach (int idx in selectedPoints)
            {
                tempoSerie.add(pointAt(idx));
            }
            try
            {
                PerformanceModelSolver solver = new PerformanceModelSolver(tempoSerie);
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
        /// Défini par défaut le domaine de calcul de la série, calqué sur l'étendue des points de performance.
        /// Les bornes sont inclusives par défaut.
        /// </summary>
        /// <returns>True en cas de succès</returns>
        ///
        public bool setRange()
        {
            if (this.count > 0)
            {
                return setRange(pointAt(0).input, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT,
                                pointAt(perfPointList.Count - 1).input, AeroCalc.MODEL_RANGE_INCLUDE_LIMIT);
            }
            return false;
        }



        // METHODS //////////////////////////////////////////////////////////////////////////////////////////



        private double[] unsignedDistances(double x)
        {
            // Au minimum, il faut un point de performance dans la série
            if (perfPointList.Count < 1) { return null; }
            
            double[] distances = new double[perfPointList.Count - 1];

            foreach (PerfPoint pp in perfPointList)
            {
                distances[perfPointList.IndexOf(pp)] = Math.Abs(x - pp.input);
            }
            return distances;
        }



        private double[] signedDistances(double x)
        {
            // Au minimum, il faut un point de performance dans la série
            if (perfPointList.Count < 1) { return null; }

            double[] distances = new double[perfPointList.Count - 1];

            foreach (PerfPoint pp in perfPointList)
            {
                distances[perfPointList.IndexOf(pp)] = pp.input - x;
            }
            return distances;
        }
        // Accesseur de test
        public double[] _A_signedDistances(double x)
        {
            return signedDistances(x);
        }



        private int[] subDomain(double x)
        {
            double[] distTable = signedDistances(x);
            if (distTable == null || distTable.Length<2)
            {
                return null; 
            }

            // Liste temporaire pour stocker les index des points situés dans le sous-domaine
            List<int> subDomainIndexes = new List<int>();
            int inf=0;
            int sup = distTable.Length - 1;

            // Cas où x est en dehors de l'étendue de la PerfSerie
            // Le sous-domaine est limité par le premier point de rupture rencontré
            // Permet de prendre en charge les extrapolations contrôlées
            if (distTable[0] > 0)
            {
                // x est en dehors de l'étendue de la PerfSerie, le sous-domaine commence au premier point
                for (int index = 0; index < distTable.Length; index++)
                {
                    subDomainIndexes.Add(index);
                    if (pointAt(index).isBreak)
                    {
                        // Point de rupture, le sous-domaine s'arrête ici
                        break;
                    }
                }
                return subDomainIndexes.ToArray();
            }
            if (distTable[distTable.Length - 1] < 0)
            {
                // x est en dehors de l'étendue de la PerfSerie, le sous-domaine finit au dernier point
                for(int index = distTable.Length -1; index >=0; index--)
                {
                    subDomainIndexes.Add(index);
                    if (pointAt(index).isBreak)
                    {
                        // Point de rupture, le sous-domaine s'arrête ici
                        break;
                    }
                }
                subDomainIndexes.Reverse();
                return subDomainIndexes.ToArray();
            }

            // Cas général, x est dans l'étendue de la PerfSerie
            for (int index = 0; index < distTable.Length; index++)
            {
                if (distTable[index] <= 0)
                {
                    subDomainIndexes.Add(index);
                }
            }
            return subDomainIndexes.ToArray();
        }
        // Accesseur de test
        public int[] _A_subDomain(double x)
        {
            return subDomain(x);
        }



        /// <summary>
        /// Renvoie un tableau des index de points de performance, dont l'abscisse voisine avec x, en s'assurant de
        /// l'encadrement autour de la valeur x.
        /// Permet de disposer d'un jeu de points cohérent pour une interpolation polynomiale de Lagrange.
        /// </summary>
        /// <param name="x">Abscisse du point d'intérêt</param>
        /// <param name="nbPoints">Nombre de points de performance souhaité</param>
        /// <returns>Table d'int des indexes des points retenus</returns>
        private int[] closestPointsAround(double x, int nbPoints)
        {
            int[] subDomainIndexes = subDomain(x);
            if (subDomainIndexes == null) { return null; }

            if (subDomainIndexes.Length <= nbPoints)
            {
                // Le sous-domaine contient le nombre de points souhaités, on les retourne tous
                return subDomainIndexes;
            }

            // Cas général, au moins quatre points dans le sous-domaine

            // Liste temporaire pour stocker les index des points sélectionnés les plus proches, pour retrourner
            // un tableau de deux ou trois indexes
            List<int> closestPts = new List<int>();

            int[] orderedIndexes = sortIndexesByDistance(subDomainIndexes, x);

            // Le point le plus proche est retenu systématiquement
            closestPts.Add(orderedIndexes[0]);

            // Le point le plus proche est à la même abscisse que le point d'intérêt, il suffit d'ajouter le point suivant
            if (x == pointAt(orderedIndexes[0]).input)
            {
                closestPts.Add(orderedIndexes[1]);
                return closestPts.ToArray();
            }

            // Le point le plus proche est la limite haute ou basse du sous domaine de la PerfSerie
            if (orderedIndexes[0] == 0 || orderedIndexes[0] == orderedIndexes.Length - 1)
            {
                for (int i = 1; i < orderedIndexes.Length; i++)
                {
                    if (closestPts.Count == nbPoints)
                    {
                        // Trois points sélectionnés, on arrête les ajouts
                        break;
                    }
                    closestPts.Add(orderedIndexes[i]);
                    return closestPts.ToArray();
                }
            }

            // Cas général, le point le plus proche n'est pas une limite du sous domaine de la PerfSerie

            if (x > pointAt(orderedIndexes[0]).input)
            {
                // x est situé après l'abscisse du point le plus proche, dans le sous domaine de la PerfSerie
                // On recherche le prochain point dont l'abscisse est supérieure à x
                for (int i = 1; i < orderedIndexes.Length; i++)
                {
                    if (pointAt(orderedIndexes[i]).input > x)
                    {
                        // Point trouvé, on l'ajoute
                        closestPts.Add(orderedIndexes[i]);
                        break;
                    }
                }

            }
            else
            {
                // x est situé avant l'abscisse du point le plus proche, dans le sous domaine de la PerfSerie
                // On recherche le prochain point dont l'abscisse est inférieure à x
                for (int i = 1; i < orderedIndexes.Length; i++)
                {
                    if (pointAt(orderedIndexes[i]).input < x)
                    {
                        // Point trouvé, on l'ajoute
                        closestPts.Add(orderedIndexes[i]);
                        break;
                    }
                }
            }
            // Recherche des points suivants, dans l'ordre de proximité d'abscisse avec x
            for (int i = 1; i < orderedIndexes.Length; i++)
            {
                if (closestPts.Count == nbPoints)
                {
                    // Nombre de points sélectionnés atteint, on arrête les ajouts
                    break;
                }
                if (closestPts.Contains(orderedIndexes[i]))
                {
                    // Point déjà sélectionné, on l'ignore
                    continue;
                }
                closestPts.Add(orderedIndexes[i]);
            }
            return closestPts.ToArray();
        }
        public int[] _A_closestPointsAround(double x, int nb)
        {
            return closestPointsAround(x, nb);
        }



        /// <summary>
        /// Renvoie un tableau des index des points de performance, classés par ordre de proximité avec une valeur x.
        /// </summary>
        /// <param name="domain">Tableau des index du sous domaine de la PerfSerie</param>
        /// <param name="x">Abscisse du point d'intérêt</param>
        /// <returns>Table de Int, null si aucun ou un seul point est présent dans le sous domaine</returns>
        ///
        private int[] sortIndexesByDistance(int[] domain, double x)
        {

            if (domain == null)
            {
                return null;
            }
            if (domain.Length < 2)
            {
                return null;
            }

            // Cas général, le sous domaine de la PerfSerie contient au moins deux points de performance
            int[] sortedIndexesByDist = new int[domain.Length];
            int minDistIndex = -1;
            int upIndex = -1;
            int downIndex = -1;
            double dist = -1;
            double minDist = Double.MaxValue;
            double[] distances = new double[domain.Length];
            int[] classement = new int[domain.Length];

            // Calcul des distances et détermination du point de plus grande proximité
            for (int count = 0; count < domain.Length; count++)
            {
                distances[count] = pointAt(count).input - x;
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
            sortedIndexesByDist[0] = minDistIndex;

            // Cas des extrémités
            if (minDistIndex == 0)
            {
                // Trivial, le classement est identique à l'index de tableau
                for (int count = 0; count < sortedIndexesByDist.Length; count++)
                {
                    sortedIndexesByDist[count] = count;
                }
                return sortedIndexesByDist;
            }
            if (minDistIndex == perfPointList.Count - 1)
            {
                // Trivial, le classement est inverse de l'index du tableau
                for (int count = 0; count < sortedIndexesByDist.Length; count++)
                {
                    classement[count] = sortedIndexesByDist.Length - 1 - count;
                }
                return sortedIndexesByDist;
            }

            // Cas général, minDistIndex n'est pas en limite de tableau
            downIndex = minDistIndex - 1;
            upIndex = minDistIndex + 1;

            for (int count = 1; count < sortedIndexesByDist.Length; count++)
            {
                if (downIndex < 0)
                {
                    // upIndex désigne le dernier point disponible vers la limite basse
                    sortedIndexesByDist[count] = upIndex;
                    upIndex++;
                }
                else if (upIndex > distances.Length - 1)
                {
                    // downIndex désigne le seul point disponible
                    sortedIndexesByDist[count] = downIndex;
                    downIndex--;
                }
                else
                {
                    if (Math.Abs(distances[downIndex]) > Math.Abs(distances[upIndex]))
                    {
                        // upIndex désigne le point le plus proche
                        sortedIndexesByDist[count] = upIndex;
                        upIndex++;
                    }
                    else
                    {
                        // downIndex désigne le point le plus proche
                        sortedIndexesByDist[count] = downIndex;
                        downIndex--;
                    }
                }
            }
            return sortedIndexesByDist;
        }
        // Accesseur de test
        public int[] _A_sortIndexesByDistance(int[] domain,double x)
        {
            return sortIndexesByDistance(domain, x);
        }

    }

}