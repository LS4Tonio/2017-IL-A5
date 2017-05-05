using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace Algo
{
    public partial class User
    {
        internal static string[] CellSeparator = { "::" };

        private ushort _userId;
        private readonly byte _age;

        /// <summary>
        /// User information is in the file "users.dat" and is in the following
        /// format:
        /// UserID::Gender::Age::Occupation::Zip-code
        /// </summary>
        /// <param name="line"></param>
        private User(string line)
        {
            string[] cells = line.Split(CellSeparator, StringSplitOptions.None);
            _userId = UInt16.Parse(cells[0]);
            Male = cells[1] == "M";
            _age = Byte.Parse(cells[2]);
            Occupation = String.Intern(cells[3]);
            ZipCode = String.Intern(cells[4]);
            Ratings = new Dictionary<Movie, int>();
        }

        public static User[] ReadUsers(string path)
        {
            List<User> u = new List<User>();
            using (TextReader r = File.OpenText(path))
            {
                string line;
                while ((line = r.ReadLine()) != null) u.Add(new User(line));
            }
            return u.ToArray();
        }

        static public int ReadRatings( User[] users, Movie[] movies, string path )
        {
            int count = 0;
            using (TextReader r = File.OpenText(path))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    string[] cells = line.Split(CellSeparator, StringSplitOptions.None);
                    int idUser = int.Parse( cells[0] ) - 1;
                    int idMovie = int.Parse( cells[1] ) - 1;
                    Debug.Assert(idMovie >= 0 && idMovie < movies.Length);
                    Debug.Assert(idUser >= 0 && idUser < users.Length);
                    users[idUser].Ratings.Add( movies[idMovie], int.Parse( cells[2] ) );
                    ++count;
                }
            }
            return count;
        }

        public int UserID { get { return _userId; } set { _userId = (ushort)value; } }

        public bool Male { get; }

        public int Age { get { return _age; } }

        public string Occupation { get; }

        public string ZipCode { get; }

        public Dictionary<Movie, int> Ratings { get; }
    }
}