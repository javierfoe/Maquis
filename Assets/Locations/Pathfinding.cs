    using System.Collections.Generic;

    public class Pathfinding
    {
        private readonly Dictionary<Location, List<Path>> _edges = new();

        public Pathfinding(Location[] nodes, Path[] edges)
        {
            foreach (var location in nodes)
            {
                _edges.Add(location, new ());
            }

            foreach (var path in edges)
            {
                _edges[path.First].Add(path);
                _edges[path.Second].Add(path);
            }
        }

        public bool FindPathSafeHouse(Location location)
        {
            var traversedLocations = new List<Location>();
            var pendingLocations = new List<Location> { location };
            while (pendingLocations.Count > 0)
            {
                var aux = pendingLocations[0];
                if (aux.IsSafeHouse()) return true;
                foreach (var path in _edges[aux])
                {
                    var other = path.GetOtherEnd(aux);
                    if (other != null && !traversedLocations.Contains(other))
                    {
                        pendingLocations.Add(other);
                    }
                }
                traversedLocations.Add(aux);
                pendingLocations.RemoveAt(0);
            }
            return false;
        }
    }