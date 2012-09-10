using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lab2.Models.Entities.Abstract;
using Lab2.Models.Repositories;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lab2.Models.Entities
{
    public class Post : IEntity
    {
        public Post() { }
        public Post(Guid createdByID, string title, string body)
        {
            ID = Guid.NewGuid();
            CreatedByID = createdByID;
            Title = title;
            Body = body;
            CreateDate = DateTime.UtcNow;
        }

        public Guid ID { get; set; }
        public Guid ThreadID { get; set; }
        private User _CreatedBy { get; set; }
        public Guid CreatedByID { get; set; }
        public User CreatedBy { get { return _CreatedBy; } }
        [DisplayName("Created")]
        public DateTime CreateDate { get; set; }


        [Required(ErrorMessage = "Title is required!")]
        [DisplayName("Title ")]
        public string Title { get; set; }
        public string TitleShort 
        { 
            get 
            {
                return Title.Length > 20 ? Title.Substring(0, 17) + "..." : Title;
            } 
        }

        [Required(ErrorMessage = "Message is required!")]
        [DisplayName("Message ")]
        public string Body { get; set; }
        public string BodyShort {
            get
            {
                return Body.Length > 20 ? Body.Substring(0, 17) + "..." : Body;
            }
        }
        public List<PostTags> Tags { get; set; }
        public string FormattedTagList
        {
            get
            {
                if (Tags.Count == 0)
                    return "";

                string tagListString = "";
                foreach (var tag in Tags)
                {
                    if (!string.IsNullOrEmpty(tagListString))
                        tagListString += ", ";
                    tagListString += tag;
                }

                return tagListString;
            }
        }

        public string ToString(bool ShortFormat = true)
        {
            string postString = string.Format("\tTitle: '{0}' - By: '{1}' - Posted: '{2:d}'", Title, CreatedBy != null ? CreatedBy.UserName : "?", CreateDate);
            if (!ShortFormat)
                postString += string.Format("\n\t\t Message: {0}\n\t\t Tags: {1}", Body, FormattedTagList);

            return postString;
        }

        /*
         * OBS! Denna konstruktion för att ladda relaterad data är varken en bra eller vanlig konstruktion.
         * 
         * Vi kommer se hur detta kan skötas i verkligheten senare i kursen
         * 
         */
        public void LoadUser()
        {
            // OBS! Dålig konstruktion!!! Det är repository's ansvar att ladda användare
            // t.ex. skulle vi kunna tänka oss en konstruktion likt denna:
            // Repository.Instance.All<User>().Include(u => u.CreatedBy)
            // I det här fallet kör vi på en ful-lösning dock :(
            // Skulle jag lösa detta i det här fallet så hade jag byggt en ExtensionMethod för det.
            _CreatedBy = Repository.Instance.All<User>().Where(u => u.ID == CreatedByID).FirstOrDefault();
        }

        public enum PostTags
        {
            Interesting,
            Funny,
            Troll,
            JawDropping,
            Useful
        }
    }
}
