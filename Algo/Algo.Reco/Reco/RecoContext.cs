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

        public IEnumerable<UserDistance> GetClosestUsers(User userRef)
        {
            var list = new List<UserDistance>();

            foreach (var user in Users)
            {
                if (user.UserID == userRef.UserID) continue;
                list.Add(new UserDistance
                {
                    Similarity = SimilarityPearson(userRef, user),
                    User = user
                });
            }

            return list.OrderByDescending(x => Math.Abs(x.Similarity));
        }

        public IEnumerable<Movie> GetUnseenMovies(User u, IEnumerable<User> users)
        {
            var list = new List<Movie>();

            foreach (var user in users)
            {
                list.AddRange(user.Ratings.Keys.Where(k => k != null)
                    .Except(u.Ratings.Keys.Where(k => k != null))
                    .Except(list));
            }

            return list;
        }

        public IEnumerable<MovieWeight> GetBestMovies(User u, int max)
        {
            var closestUsers = GetClosestUsers(u);
            var movies = GetUnseenMovies(u, closestUsers.Select(x => x.User));

            var list = new List<MovieWeight>();
            foreach (var movie in movies)
            {
                var notes = new List<double>();
                foreach (var user in closestUsers)
                {
                    if (!user.User.Ratings.ContainsKey(movie))
                        continue;

                    // Note * similarité
                    notes.Add(user.User.Ratings[movie] * user.Similarity);
                }

                var mw = new MovieWeight
                {
                    Movie = movie,
                    Weight = notes.Average()
                };
                list.Add(mw);
            }

            var listOrdered = list.OrderByDescending(x => x.Weight);
            return listOrdered.Take(max);
        }

        public IEnumerable<MovieWeight> GetBestMoviesOptimized(User user, int max)
        {
            if (user == null || max < 0) throw new ArgumentException();

            if (!user.Ratings.Any())
                return new List<MovieWeight>();

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
            var bestMovies = new List<MovieWeight>();
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
                bestMovies.Add(mw);
            }

            return bestMovies.OrderByDescending(x => x.Weight).Take(max);
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

    public static class DictionaryExtension
    {
        public static TValue GetValueWithDefault<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key, TValue def)
        {
            TValue v;
            return @this.TryGetValue(key, out v) ? v : def;
        }
    }
}