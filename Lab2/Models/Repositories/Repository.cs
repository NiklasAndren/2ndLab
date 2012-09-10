using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lab2.Models.Repositories.Abstract;
using Lab2.Models.Entities;
using Lab2.Models.Entities.Abstract;
using System.Collections;
using Lab2.Models.Repositories.Helpers;

namespace Lab2.Models.Repositories
{
    /// <summary>
    /// Klass som hanterar data för applikationen
    /// </summary>
    public class Repository : IRepository
    {
        // privata listor med User- och Post-objekt
        private Dictionary<string,IList> dataSource;

        /// <summary>
        /// Returns the instance of the repository (Makes sure the repository is only created once - Using the Singleton Design Pattern
        /// </summary>
        public static Repository Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot) // Thread-Safe Repository since we are in a multithread environment
                    {
                        if (instance == null)
                            instance = new Repository();
                    }
                }

                return instance;
            }
        }
        private static volatile Repository instance;
        public static object syncRoot = new Object();

        /// <summary>
        /// Privat Konstruktor som genererar 36 dummy Users och 100 dummy Posts
        /// </summary>
        private Repository()
        {
            // Här inne genererar vi dummy-data. För denna lab behöver du inte veta hur denna data genereras.
            List<Guid> UserIDs;
            List<User> users = DummyDataGenerators.GenerateUsers(out UserIDs); // För nyckelordet 'out' se kommentar innan definitionen av GenerateUsers
            List<Post> posts = DummyDataGenerators.GeneratePosts(UserIDs);
            List<ForumThread> forumThreads = DummyDataGenerators.GenerateThreads(ref posts);
            List<News> news = DummyDataGenerators.GenerateNews(UserIDs);

            dataSource = new Dictionary<string, IList>();
            dataSource.Add(users.GetType().GetGenericArguments()[0].FullName, users);
            dataSource.Add(posts.GetType().GetGenericArguments()[0].FullName, posts);
            dataSource.Add(forumThreads.GetType().GetGenericArguments()[0].FullName, forumThreads);
            dataSource.Add(news.GetType().GetGenericArguments()[0].FullName, news);
        }

        /// <summary>
        /// Returns a list of the specified Type
        /// </summary>
        /// <typeparam name="T">Valid types: User, Post, News or ForumThread</typeparam>
        /// <returns></returns>
        public List<T> All<T>() where T : IEntity
        {
            if (!dataSource.ContainsKey(typeof(T).FullName))
                throw new Exception("Unsupported Type!");

            return (List<T>)dataSource[typeof(T).FullName];
        }

        /// <summary>
        /// Returns an entity from the datasource with the specified ID or null if the ID doesn't exist
        /// 
        /// Usage: User myUser = myRepository.Get<User>(id);
        /// </summary>
        /// <typeparam name="T">Valid types: User, Post, News or ForumThread</typeparam>
        /// <param name="id">ID of the Entity</param>
        /// <returns></returns>
        public T Get<T>(Guid id) where T : IEntity
        {
            return All<T>().Where(t => t.ID == id).FirstOrDefault();
        }

        /// <summary>
        /// Deletes the specified entity
        /// </summary>
        /// <typeparam name="T">Valid types: User, Post, News or ForumThread</typeparam>
        /// <param name="entity">The entity to delete</param>
        public void Delete<T>(T entity) where T : IEntity
        {
            if (!dataSource.ContainsKey(typeof(T).FullName))
                throw new Exception("Unsupported Type!");

            dataSource[typeof(T).FullName].Remove(entity);
        }

        /// <summary>
        /// Saves an entity
        /// </summary>
        /// <typeparam name="T">Valid types: User, Post, News or ForumThread</typeparam>
        /// <param name="entity">The entity to be saved</param>
        public void Save<T>(T entity) where T : IEntity
        {
            List<T> list = All<T>();

            if (list.Contains(entity))
                list[list.IndexOf(entity)] = entity;
            else
            {
                if (entity.ID == null || entity.ID == Guid.Empty)
                    entity.ID = Guid.NewGuid();
                list.Add(entity);
            }

            dataSource[typeof(T).FullName] = list;
        }

        // Hämta en sorterad lista av take st. användare med början på skip + 1
        public List<User> GetSortedUsers(int take, int skip)
        {
            return All<User>().OrderBy(u => u.UserName).Skip(skip).Take(take).ToList();
        }

        // Hämta en användare baserat på dess användarnamn
        public User GetUserByUserName(string userName)
        {
            return All<User>().Where(u => u.UserName == userName).FirstOrDefault();
        }

        // Hämta en lista av take st. Post för en användare, senaste först
        public List<Post> GetLatestPostForUser(Guid userID, int take)
        {
            return All<Post>().Where(p => p.CreatedByID == userID).Take(take).ToList();
        }

        public List<ForumThread> GetSortedThreads()
        {
            return All<ForumThread>().OrderBy(t => t.CreateDate).ToList();
        }

        public List<Post> GetPostbyThreadId(Guid id)
        {
            return All<Post>().OrderByDescending(c => c.CreateDate).Where(p => p.ThreadID == id).ToList();
        }

        public List<User> GetSiteAdmins()
        {
            return All<User>().Where(a => a.Type == User.UserType.Admin).ToList();
        }

    }
}
