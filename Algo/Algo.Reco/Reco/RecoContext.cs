using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

namespace Algo
{
    public class RecoContext
    {
        public User[] Users { get; private set; }
        public Movie[] Movies { get; private set; }

        public void LoadFrom(string folder)
        {
            Users = User.ReadUsers(Path.Combine(folder, "users.dat"));
            Movies = Movie.ReadMovies(Path.Combine(folder, "movies.dat"));
            User.ReadRatings(Users, Movies, Path.Combine(folder, "ratings.dat"));
        }

        public double DistNorm2(User u1, User u2)
        {
            var delta = u1.Ratings.Select(mr1 => new
            {
                R1 = mr1.Value,
                R2 = u2.Ratings.GetValueWithDefault(mr1.Key, -1)
            })
                .Where(r1R2 => r1R2.R2 >= 0)
                .Select(r1R2 => r1R2.R1 - r1R2.R2)
                .Select(d => d * d);
            var enumerable = delta as IList<int> ?? delta.ToList();
            return enumerable.Any() ? Math.Sqrt(enumerable.Sum()) : 0;
        }

        public double SimilarityNorm2(User u1, User u2)
        {
            return 1 / (1 + DistNorm2(u1, u2));
        }

        public double SimilarityPearson(User u1, User u2)
        {
            var ratings = u1.Ratings.Keys.Intersect(u2.Ratings.Keys)
               .Select(m => new KeyValuePair<int, int>(u1.Ratings[m], u2.Ratings[m]));

            return SimilarityPearson(ratings);
        }

        public double SimilarityPearson(params int[] values)
        {
            if (values == null || (values.Length & 1) == 0) throw new ArgumentException();
            return SimilarityPearson(Convert(values));
        }

        public double SimilarityPearson(IEnumerable<int> v1, IEnumerable<int> v2)
        {
            return SimilarityPearson(v1.Zip(v2, (x, y) => new KeyValuePair<int, int>(x, y)));
        }

        public double SimilarityPearson(IEnumerable<KeyValuePair<int, int>> values)
        {
            var sumX = 0.0;
            var sumY = 0.0;
            var sumXY = 0.0;
            var sumX2 = 0.0;
            var sumY2 = 0.0;

            var count = 0;
            var pairs = values as IList<KeyValuePair<int, int>> ?? values.ToList();
            foreach (var m in pairs)
            {
                count++;
                var x = m.Key;
                var y = m.Value;
                sumX += x;
                sumY += y;
                sumXY += x * y;
                sumX2 += x * x;
                sumY2 += y * y;
            }

            if (count == 0) return 0.0;
            if (count == 1)
            {
                var onlyOne = pairs.Single();
                double d = Math.Abs(onlyOne.Key - onlyOne.Value);
                return 1 / (1 + d);
            }

            checked
            {
                var numerator = sumXY - (sumX * sumY / count);
                var denumerator1 = sumX2 - (sumX * sumX / count);
                var denumerator2 = sumY2 - (sumY * sumY / count);
                return numerator / Math.Sqrt(denumerator1 * denumerator2);
            }
        }

        private IEnumerable<KeyValuePair<int, int>> Convert(int[] values)
        {
            Debug.Assert(values != null && (values.Length & 1) == 0);
            for (var i = 0; i < values.Length; i++)
            {
                yield return new KeyValuePair<int, int>(values[i], values[++i]);
            }
        }

        public IEnumerable<MovieWeight> GetBestMoviesOptimized(User user, int maxMovies, int maxUsers)
        {
            if (user == null || maxMovies < 0 || maxUsers <= 0) throw new ArgumentException();

            // Top et flop des users similaires
            var keeperUsers = new BestKeeper<UserDistance>(maxUsers,
                Comparer<UserDistance>.Create(
                    (a, b) =>
                    {
                        if (Math.Abs(a.Similarity) > Math.Abs(b.Similarity))
                        {
                            return 1;
                        }
                        if (Math.Abs(a.Similarity) < Math.Abs(b.Similarity))
                        {
                            return -1;
                        }
                        return 0;
                    }));

            if (!user.Ratings.Any())
                return new List<MovieWeight>();

            foreach (var userInDb in Users)
            {
                // Do not take the same user :p
                if (user.UserID == userInDb.UserID) continue;

                // Calc similarity
                var similarity = SimilarityPearson(user, userInDb);

                // store closest user
                // if user is add on keeper users
                keeperUsers.Add(new UserDistance
                {
                    Similarity = similarity,
                    User = userInDb
                });
            }

            // get movies
            var moviesUnseen = new List<Movie>();
            foreach (var ud in keeperUsers.GetBestKeeper())
            {
                // Get movies not seen by user
                moviesUnseen.AddRange(
                    ud.User.Ratings.Keys
                        .Except(user.Ratings.Keys)
                        .Except(moviesUnseen));
            }


            // Calculate movies weight
            var bestKeeperMovies = new BestKeeper<MovieWeight>(maxMovies,
            Comparer<MovieWeight>.Create(
                (a, b) =>
                {
                    if (Double.IsNaN(a.Weight) || Double.IsNaN(b.Weight))
                    {
                        return -1;
                    }

                    if (a.Weight > b.Weight)
                    {
                        return 1;
                    }
                    if (a.Weight < b.Weight)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }));

            foreach (var movie in moviesUnseen)
            {
                var notes = new List<double>();

                foreach (var ud in keeperUsers.GetBestKeeper())
                {
                    if (!ud.User.Ratings.ContainsKey(movie))
                        continue;

                    // Note * similarité
                    notes.Add((ud.User.Ratings[movie] - 3) * ud.Similarity);
                }

                var mw = new MovieWeight
                {
                    Movie = movie,
                    Weight = notes.Average() + 3
                };
                bestKeeperMovies.Add(mw);
            }

            return bestKeeperMovies.GetBestKeeper();
        }

        private struct UserDistance
        {
            public double Similarity;
            public User User;
        }

        public struct MovieWeight
        {
            public Movie Movie;
            public double Weight;

        }
        #endregion Recommandations
    }




    public class BestKeeper<T>
    {
        List<T> _bestKeeper;
        IComparer<T> _comparer;
        int _length;

        public List<T> GetBestKeeper()
        {
            return _bestKeeper;
        }

        public BestKeeper(int length, IComparer<T> comparer)
        {
            _bestKeeper = new List<T>();
            _comparer = comparer;
            _length = length;
        }

        // Add an element if necessary
        public void Add(T value)
        {
            var index = findIndex(value);
            if (index >= 0)
            {
                _bestKeeper.Insert(index, value);
                if (_bestKeeper.Count > _length)
                {
                    _bestKeeper.RemoveAt(_length);
                }
            }
            else if (_bestKeeper.Count < _length)
            {
                _bestKeeper.Add(value);
            }
        }

        private int findIndex(T value)
        {
            // return _bestKeeper.BinarySearch(value, _comparer);
            foreach (var element in _bestKeeper)
            {
                if (_comparer.Compare(value, element) >= 0)
                {
                    return _bestKeeper.IndexOf(element);
                }
            }
            return -1;
        }
    }

    public static class ListExtension
    {
        public static void AddInBestKeeper<T>(this List<T> @this, T value, BestKeeper<T> bestKeeper)
        {
            bestKeeper.Add(value);
            @this.Add(value);
        }
    }

    public static class DictionaryExtension
    {
        public static TValue GetValueWithDefault<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key, TValue def)
        {
            TValue v;
            return @this.TryGetValue(key, out v) ? v : def;
        }
    }
}
