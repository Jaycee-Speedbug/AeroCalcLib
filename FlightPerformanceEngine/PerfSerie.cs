using System;
using System.Collections.Generic;
using System.Linq;



namespace AeroCalcCore.FlightPerformanceEngine
{

    /// <summary>
    /// Classe de dimension 2 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un ensemble cohérent de points de performance de vol
    /// Le RANGE de la série peut être défini pour limiter le domaine de calcul des interpolations ou permettre
    /// des extrapolations contrôlées.
    /// </summary>
    /// 
    public class PerfSerie : IComparable<PerfSerie>
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
            if (ps != null)
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
            int[] selectedPoints = closestPointsAround(inputValue, 3);
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
        /// <param name="ps">PerfSerie à laquelle se comparer</param>
        /// <returns>-1 si le facteur de ps est inférieur, O si la valeur du facteur est identique, 1 si le facteur de ps est supérieur</returns>
        ///
        public int CompareTo(PerfSerie ps) => factorValue.CompareTo(ps.factorValue);


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


        /*
        private double[] unsignedDistances(double x)
        {
            // Au minimum, il faut un point de performance dans la série
            if (perfPointList.Count < 1) { return null; }

            double[] distances = new double[perfPointList.Count];

            foreach (PerfPoint pp in perfPointList)
            {
                distances[perfPointList.IndexOf(pp)] = Math.Abs(x - pp.input);
            }
            return distances;
        }
        // Accesseur de test
        public double[] _A_unsignedDistances(double x)
        {
            return unsignedDistances(x);
        }
        */


        /*
        private double[] signedDistances(double x)
        {
            // Au minimum, il faut un point de performance dans la série
            if (perfPointList.Count < 1) { return null; }

            double[] distances = new double[perfPointList.Count];

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
        */



        /// <summary>
        /// Identifies the contiguous subdomain of points surrounding the specified input value, respecting breakpoints
        /// in the series.
        /// </summary>
        /// <remarks>The subdomain is determined by locating the closest point to the specified input
        /// value and expanding outward until a breakpoint is encountered on either side. If the input value is outside
        /// the range of the series, the subdomain will include points from the nearest end up to the first breakpoint.
        /// The returned indexes are ordered in increasing sequence.</remarks>
        /// <param name="x">The input value for which to determine the relevant subdomain. Represents the target position within the
        /// series.</param>
        /// <returns>An array of indexes representing the points in the subdomain that contains or is closest to the specified
        /// input value. Returns null if the series contains no points.</returns>
        private int[] subDomain(double x)
        {
            int n = this.count;
            if (n == 0)
                return null;
            if (n == 1)
                return new[] { 0 };

            // 1) Récupérer la liste des index de breakpoints, triés naturellement
            List<int> breakIndexes = new List<int>();
            for (int i = 0; i < n; i++)
            {
                if (pointAt(i).isBreak)
                {
                    breakIndexes.Add(i);
                }
            }

            int startIndex = 0;
            int endIndex = n - 1;

            // 2) Déterminer la borne supérieure du segment :
            //    le premier breakpoint dont l'abscisse est STRICTEMENT > x
            //    (sinon, on garde la borne supérieure = fin de série)
            foreach (int b in breakIndexes)
            {
                if (pointAt(b).input > x)
                {
                    endIndex = b;
                    break;
                }
            }

            // 3) Déterminer la borne inférieure du segment :
            //    le dernier breakpoint dont l'abscisse est <= x
            //    (sinon, on garde la borne inférieure = 0)
            for (int i = breakIndexes.Count - 1; i >= 0; i--)
            {
                int b = breakIndexes[i];
                if (pointAt(b).input <= x)
                {
                    startIndex = b;
                    break;
                }
            }

            // 4) Construire le sous-domaine [startIndex .. endIndex]
            int length = endIndex - startIndex + 1;
            int[] subDomainIndexes = new int[length];
            for (int i = 0; i < length; i++)
            {
                subDomainIndexes[i] = startIndex + i;
            }

            return subDomainIndexes;
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
        // Accesseur de test
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
            if (domain == null || domain.Length < 2)
            {
                return null;
            }

            // distances[i] = (indexDansDomain, distanceAbsolue)
            var distances = new (int indexInDomain, double absDistance)[domain.Length];

            for (int i = 0; i < domain.Length; i++)
            {
                double d = pointAt(domain[i]).input - x;
                distances[i] = (domain[i], Math.Abs(d));
            }

            Array.Sort(distances, (a, b) => a.absDistance.CompareTo(b.absDistance));

            int[] sortedIndexesByDist = new int[domain.Length];
            for (int i = 0; i < domain.Length; i++)
            {
                sortedIndexesByDist[i] = distances[i].indexInDomain;
            }

            return sortedIndexesByDist;
        }
        // Accesseur de test
        public int[] _A_sortIndexesByDistance(int[] domain, double x)
        {
            return sortIndexesByDistance(domain, x);
        }

    }

}