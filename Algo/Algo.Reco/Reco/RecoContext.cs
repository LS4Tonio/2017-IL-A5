using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Collections;

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
                .Where(r1r2 => r1r2.R2 >= 0)
                .Select(r1r2 => r1r2.R1 - r1r2.R2)
                .Select(d => d * d);
            return delta.Any() ? Math.Sqrt(delta.Sum()) : 0;
        }

        public double SimilarityNorm2(User u1, User u2)
        {
            return 1 / (1 + DistNorm2(u1, u2));
        }

        public double SimilarityPearson(User u1, User u2)
        {
            // Impl. 1
            var ratings = u1.Ratings.Keys
                .Intersect(u2.Ratings.Keys)
                .Select(m => new KeyValuePair<int, int>(u1.Ratings[m], u2.Ratings[m]));

            // Impl 2
            //var ratings = u1.Ratings
            //    .Where(x => u2.Ratings.ContainsKey(x.Key))
            //    .Select(x => new KeyValuePair<int, int>(x.Value, u2.Ratings[x.Key]));

            // Impl 3
            //var ratings = new List<KeyValuePair<int, int>>();
            //foreach (var rating in u1.Ratings)
            //{
            //    if (!u2.Ratings.ContainsKey(rating.Key))
            //        continue;
            //    ratings.Add(new KeyValuePair<int, int>(rating.Value, u2.Ratings[rating.Key]));
            //}

            // Calculate
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
            var count = 0;
            double sumU1 = 0;
            double sumU2 = 0;
            double squareSumU1 = 0;
            double squareSumU2 = 0;
            double sumProd = 0;

            foreach (var v in values)
            {
                ++count;
                var r1 = v.Key;
                var r2 = v.Value;

                // Sum rating
                sumU1 += r1;
                sumU2 += r2;

                // Sum square
                squareSumU1 += r1 * r1;
                squareSumU2 += r2 * r2;

                // Sum product
                sumProd += r1 * r2;
            }

            if (count == 0) return 0;
            if (count == 1)
            {
                var single = values.Single();
                var d = Math.Abs(single.Key - single.Value);
                return 1 / (1 + d);
            }

            // Calc similarity
            checked
            {
                var numerator = sumProd - (sumU1 * sumU2 / count);
                var denominator = Math.Sqrt((squareSumU1 - Math.Pow(sumU1, 2) / count) * (squareSumU2 - Math.Pow(sumU2, 2) / count));
                return numerator / denominator;
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

        #region Recommandations

        public IEnumerable<MovieWeight> GetBestMoviesOptimized(User user, int max)
        {
            if (user == null || max < 0) throw new ArgumentException();

            var userDistance = new List<UserDistance>();
            var moviesUnseen = new List<Movie>();
            foreach (var userInDb in Users)
            {
                // Do not take the same user :p
                if (user.UserID == userInDb.UserID) continue;

                // Calc similarity
                var similarity = SimilarityPearson(user, userInDb);

                // store closest user
                userDistance.Add(new UserDistance { Similarity = similarity, User = userInDb });

                // Get movies not seen by user
                moviesUnseen.AddRange(
                    userInDb.Ratings.Keys.Where(k => k != null)
                        .Except(user.Ratings.Keys.Where(k => k != null))
                        .Except(moviesUnseen));
            }

            // Calculate movies weight
            var bestKeeperMovies = new BestKeeper<MovieWeight>(10,
                Comparer<MovieWeight>.Create(
                    (a, b) =>
                    {
                        if (a.Weight > b.Weight)
                        {
                            return 1;
                        }
                        else if (a.Weight < b.Weight)
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
                foreach (var ud in userDistance)
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

        public struct UserDistance
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

        public void Add(T value)
        {
            var index = findIndex(value);
            if (index >= 0)
            {
                _bestKeeper.Insert(index, value);
                if (_bestKeeper.Count > _length)
                {
                    _bestKeeper.RemoveAt(_length - 1);
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
                if (_comparer.Compare(element, value) >= 0)
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
