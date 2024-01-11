using DBA5.Data;
using DBA5.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.UI.WebControls;
using System.Data.SqlTypes;

// ************************************************************************************
// WEB524 Project Template V2 == 2237-0cb84679-1cb9-400b-b700-ebcc80d36e30
//
// By submitting this assignment you agree to the following statement.
// I declare that this assignment is my own work in accordance with the Seneca Academic
// Policy. No part of this assignment has been copied manually or electronically from
// any other source (including web sites) or distributed to other students.
// ************************************************************************************

namespace DBA5.Controllers
{
    public class Manager
    {

        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper instance
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            var config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below, for example...
                // cfg.CreateMap<SourceType, DestinationType>();
                // cfg.CreateMap<Product, ProductBaseViewModel>();

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();

                // Genre
                cfg.CreateMap<Genre, GenreBaseViewModel>();

                // Actor
                cfg.CreateMap<Actor, ActorBaseViewModel>();
                cfg.CreateMap<Actor, ActorAddViewModel>();
                cfg.CreateMap<Actor, ActorWithShowInfoViewModel>();
                cfg.CreateMap<ActorAddViewModel, Actor>();
                cfg.CreateMap<ActorMediaItem, ActorMediaItemWithContentViewModel>();
                cfg.CreateMap<ActorMediaItem, ActorMediaItemBaseViewModel>();

                // Show
                cfg.CreateMap<Show, ShowBaseViewModel>();
                cfg.CreateMap<ShowAddViewModel, Show>();
                cfg.CreateMap<Show, ShowWithInfoViewModel>();              

                // Episode
                cfg.CreateMap<Episode, EpisodeBaseViewModel>();
                cfg.CreateMap<Episode, EpisodeWithShowNameViewModel>();
                cfg.CreateMap<EpisodeAddViewModel, Episode>();
                cfg.CreateMap<Episode, EpisodeVideoViewModel>();
            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }


        // Add your methods below and call them from controllers. Ensure that your methods accept
        // and deliver ONLY view model objects and collections. When working with collections, the
        // return type is almost always IEnumerable<T>.
        //
        // Remember to use the suggested naming convention, for example:
        // ProductGetAll(), ProductGetById(), ProductAdd(), ProductEdit(), and ProductDelete().

        // Genre
        public IEnumerable<GenreBaseViewModel> GenreGetAll()
        {
            var allGenres = ds.Genres.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Genre>, IEnumerable<GenreBaseViewModel>>(allGenres);
        }

        // Actor
        public IEnumerable<ActorBaseViewModel> ActorGetAll()
        {
            var allActors = ds.Actors.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Actor>, IEnumerable<ActorBaseViewModel>>(allActors);
        }

        public ActorWithShowInfoViewModel ActorAdd(ActorAddViewModel newItem)
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;
            if (user == null)
            {
                return null;
            }
            else 
            {
                var addedItem = ds.Actors.Add(mapper.Map<ActorAddViewModel, Actor>(newItem));
                addedItem.Executive = user;
                ds.SaveChanges();
                return (addedItem == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(addedItem);
            }
        }

        public ActorWithShowInfoViewModel ActorGetByIdWithShowInfo(int id)
        {
            // Attempt to fetch the object
            var theActor = ds.Actors
                             .Include("Shows")
                             .Include("ActorMediaItems")
                             .SingleOrDefault(a => a.Id == id);
            
            // Return the result, or null if not found
            return (theActor == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(theActor);
        }

        public ActorMediaItemWithContentViewModel ActorMediaItemGetById(int id)
        {
            var o = ds.ActorMediaItems.SingleOrDefault(p => p.Id == id);

            return (o == null) ? null : mapper.Map<ActorMediaItem, ActorMediaItemWithContentViewModel>(o);
        }

        // This works like a typical add/edit Manager class method
        // that has an associated object
        public ActorBaseViewModel ActorMediaItemAdd(ActorMediaItemAddViewModel newItem)
        {
            // Validate the associated item
            var a = ds.Actors.Find(newItem.ActorId);

            if (a == null)
            {
                return null;
            }
            else
            {
                // Attempt to add the new item
                var addedItem = new ActorMediaItem();
                ds.ActorMediaItems.Add(addedItem);

                addedItem.Caption = newItem.Caption;
                addedItem.Actor = a;

                // Handle the uploaded photo...

                // First, extract the bytes from the HttpPostedFile object
                byte[] contentBytes = new byte[newItem.ContentUpload.ContentLength];
                newItem.ContentUpload.InputStream.Read(contentBytes, 0, newItem.ContentUpload.ContentLength);

                // Then, configure the new object's properties
                addedItem.Content = contentBytes;
                addedItem.ContentType = newItem.ContentUpload.ContentType;
                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<Actor, ActorBaseViewModel>(a);
            }
        }

        // Show
        public IEnumerable<ShowBaseViewModel> ShowGetAll()
        {
            var allShows = ds.Shows.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Show>, IEnumerable<ShowBaseViewModel>>(allShows);
        }

        public ShowWithInfoViewModel ShowAdd(ShowAddViewModel newItem) 
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;        
            if (user == null)
            {
                return null;
            }
            else
            {
                
                var addedItem = ds.Shows.Add(mapper.Map<ShowAddViewModel, Show>(newItem));

                addedItem.Coordinator = user;
                // Create a collection to store actors for the current show
                var showActors = new List<Actor>();

                foreach (var actorId in newItem.ActorIds)
                {
                    // Find the actor by ID
                    var actor = ds.Actors.Find(actorId);

                    // Check if the actor was found
                    if (actor != null)
                    {
                        // Add the actor to the show's collection
                        showActors.Add(actor);
                    }
                }

                // Assign the collection of actors to the show
                addedItem.Actors = showActors;

                ds.SaveChanges();

                return (addedItem == null) ? null : mapper.Map<Show, ShowWithInfoViewModel>(addedItem);
            }

        }

        public ShowWithInfoViewModel ShowGetByIdWithInfo(int id)
        {
            // Attempt to fetch the object
            var theShow = ds.Shows
                .Include("Actors")
                .Include("Episodes")
                .SingleOrDefault(a => a.Id == id);

            // Return the result, or null if not found
            return (theShow == null) ? null : mapper.Map<Show, ShowWithInfoViewModel>(theShow);
        }

        // Episode
        public IEnumerable<EpisodeBaseViewModel> EpisodeGetAll()
        {
            var allEpisodes = ds.Episodes
                                .Include("Show")
                                .OrderBy(a => a.Name)
                                .ThenBy(a => a.SeasonNumber)
                                .ThenBy(a => a.EpisodeNumber);

            return mapper.Map<IEnumerable<Episode>, IEnumerable<EpisodeBaseViewModel>>(allEpisodes);
        }

        public EpisodeWithShowNameViewModel EpisodeAdd(EpisodeAddViewModel newItem)
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;
            //var genre = ds.Genres.Find(newItem.GenreId);
            var show = ds.Shows.Find(newItem.ShowId);
            if (user == null || show == null)
            {
                return null;
            }
            else
            {
                var addedItem = ds.Episodes.Add(mapper.Map<EpisodeAddViewModel, Episode>(newItem));
                byte[] videoBytes = new byte[newItem.VideoUpload.ContentLength];
                newItem.VideoUpload.InputStream.Read(videoBytes, 0, newItem.VideoUpload.ContentLength);

                addedItem.Clerk = user;
                addedItem.Show = show;

                addedItem.Video = videoBytes;
                addedItem.VideoContentType = newItem.VideoUpload.ContentType;

                ds.SaveChanges();
                return (addedItem == null) ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(addedItem);
            }
        }

        public EpisodeWithShowNameViewModel EpisodeGetByIdWithShowName(int id)
        {
            // Attempt to fetch the object
            var theEpisode = ds.Episodes
                .Include("Show")
                .SingleOrDefault(a => a.Id == id);

            // Return the result, or null if not found
            return (theEpisode == null) ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(theEpisode);
        }

         public bool UpdateEpisode(EpisodeEditViewModel model)
         {
            var episode = ds.Episodes.Find(model.Id);
            if (episode == null) return false;

            // Map properties from the model to the episode
            episode.Name = model.Name;
            episode.SeasonNumber = model.SeasonNumber;
            episode.EpisodeNumber = model.EpisodeNumber;
            episode.Genre = model.Genre;
            episode.AirDate = model.AirDate;
            episode.ImageUrl = model.ImageUrl;

            ds.SaveChanges();
            return true;
         }
         public bool DeleteEpisode(int id)
         {
            var episode = ds.Episodes.Find(id);
            if (episode == null) return false;

            ds.Episodes.Remove(episode);
            ds.SaveChanges();
            return true;
         }

      public bool UpdateShow(ShowEditViewModel model)
      {
         var show = ds.Shows.Find(model.Id);
         if (show == null) return false;

         // Map properties from the model to the show
         show.Name = model.Name;
         show.Genre = model.Genre;

         // Handle DateTime conversion
         if (model.ReleaseDate < (DateTime)SqlDateTime.MinValue)
         {
            show.ReleaseDate = (DateTime)SqlDateTime.MinValue;
         }
         else if (model.ReleaseDate > (DateTime)SqlDateTime.MaxValue)
         {
            show.ReleaseDate = (DateTime)SqlDateTime.MaxValue;
         }
         else
         {
            show.ReleaseDate = model.ReleaseDate;
         }

         show.ImageUrl = model.ImageUrl;
         show.Coordinator = model.Coordinator;
         show.Premise = model.Premise;

         ds.SaveChanges();
         return true;
      }
      public bool DeleteShow(int id)
      {
         var show = ds.Shows.Find(id);
         if (show == null) return false;

         ds.Shows.Remove(show);
         ds.SaveChanges();
         return true;
      }

      public bool UpdateActor(ActorEditViewModel model)
      {
         var actor = ds.Actors.Find(model.Id);
         if (actor == null) return false;

         // Map the updated fields
         actor.Name = model.Name;
         actor.AlternateName = model.AlternateName;
         actor.BirthDate = model.BirthDate;
         actor.Height = model.Height;
         actor.ImageUrl = model.ImageUrl;
         actor.Executive = model.Executive;
         actor.Biography = model.Biography;

         ds.SaveChanges();
         return true;
      }

      public bool DeleteActor(int id)
      {
         var actor = ds.Actors.Find(id);
         if (actor == null) return false;

         ds.Actors.Remove(actor);
         ds.SaveChanges();
         return true;
      }


      public EpisodeVideoViewModel EpisodeVideoGetById(int id)
        {
            var o = ds.Episodes.Find(id);

            return (o == null) ? null : mapper.Map<Episode, EpisodeVideoViewModel>(o);
        }

        // *** Add your methods ABOVE this line **

        #region Role Claims

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        #endregion

        #region Load Data Methods

        // Add some programmatically-generated objects to the data store
        // You can write one method or many methods but remember to
        // check for existing data first.  You will call this/these method(s)
        // from a controller action.

        // Loading Roles
        public bool LoadRoles()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // *** Role claims ***
            if (ds.RoleClaims.Count() == 0)
            {
                // Add role claims here
                ds.RoleClaims.Add(new RoleClaim() { Name = "Admin" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Executive" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Clerk" });

                ds.SaveChanges();
                done = true;
            }

            // You may load additional entities here, or you may 
            // choose to create a new method altogether.

            return done;
        }

        // Loading Genres
        public bool LoadGenres()
        {
            // Return if there's existing data
            if (ds.Genres.Count() > 0) { return false; }

            // Otherwise...
            // Create and add objects
            ds.Genres.Add(new Genre { Name = "Action" });
            ds.Genres.Add(new Genre { Name = "Comedy" });
            ds.Genres.Add(new Genre { Name = "Drama" });
            ds.Genres.Add(new Genre { Name = "Thriller" });
            ds.Genres.Add(new Genre { Name = "Adventure" });
            ds.Genres.Add(new Genre { Name = "Romance" });
            ds.Genres.Add(new Genre { Name = "Science Fiction" });
            ds.Genres.Add(new Genre { Name = "Documentry" });
            ds.Genres.Add(new Genre { Name = "Horror" });
            ds.Genres.Add(new Genre { Name = "Family" });

            // Save changes
            ds.SaveChanges();

            return true;
        }

        // Loading Actors
        public bool LoadActors()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Return if there's existing data
            if (ds.Actors.Count() > 0) { return false; }

            // Otherwise...
            // Create and add objects
            ds.Actors.Add(new Actor { 
                Name = "Mark Sinclair", 
                AlternateName = "Vin Diesel", 
                BirthDate = new DateTime(1967, 7, 18), 
                Executive = user, 
                Height = 1.82, 
                ImageUrl = "https://en.wikipedia.org/wiki/File:Vin_Diesel_2013_SDCC_(cropped).jpg"
            });

            ds.Actors.Add(new Actor
            {
                Name = "Kevin Hart",
                AlternateName = "Dell Scott",
                BirthDate = new DateTime(1979, 7, 6),
                Executive = user,
                Height = 1.63,
                ImageUrl = "https://en.wikipedia.org/wiki/File:Kevin_Hart_2014_(cropped_2).jpg"
            });

            ds.Actors.Add(new Actor
            {
                Name = "Cillian Murphy",
                AlternateName = "Thomas Shelby",
                BirthDate = new DateTime(1976, 5, 25),
                Executive = user,
                Height = 1.75,
                ImageUrl = "https://en.wikipedia.org/wiki/File:Cillian_Murphy-2014.jpg"
            });

            ds.Actors.Add(new Actor
            {
               Name = "Alexander Dreymon",
               AlternateName = "Uhtred of Bebbanburg",
               BirthDate = new DateTime(1983, 2, 7),
               Executive = user,
               Height = 1.78,
               ImageUrl = "https://en.wikipedia.org/wiki/File:Stuttgart_-Comic_Con_Germany_2019-_d90_by-RaBoe_143_(cropped).jpg"
            });

         // Save changes
         ds.SaveChanges();

            return true;
        }

        // Loading Shows
        public bool LoadShows()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Return if there's existing data
            if (ds.Shows.Count() > 0) { return false; }

         
         // Fetch the actor object, because we need it
         var vinDiesel = ds.Actors.SingleOrDefault(a => a.Name == "Mark Sinclair");
         var kevinHart = ds.Actors.SingleOrDefault(a => a.Name == "Kevin Hart");
         var cillianMurphy = ds.Actors.SingleOrDefault(a => a.Name == "Cillian Murphy");
         var alexanderDreymon = ds.Actors.SingleOrDefault(a => a.Name == "Alexander Dreymon");
         if (vinDiesel == null || kevinHart == null || cillianMurphy == null || alexanderDreymon == null)
         {
            return false;
         }
         // Shows
         ds.Shows.Add(new Show
         {
            Actors = new Actor[] { vinDiesel },
            Name = "Fast & Furious",
            Coordinator = user,
            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/3/30/Fast_%26_Furious_6_film_poster.jpg",
            Genre = "Action",
            ReleaseDate = new DateTime(2009, 4, 3)
         });

         ds.Shows.Add(new Show
         {
            Actors = new Actor[] { kevinHart },
            Name = "The Upside",
            Coordinator = user,
            ImageUrl = "https://upload.wikimedia.org/wikipedia/en/8/83/The_Upside.png",
            Genre = "Comedy",
            ReleaseDate = new DateTime(2019, 1, 11)
         });

         ds.Shows.Add(new Show
         {
            Actors = new Actor[] { cillianMurphy },
            Name = "Peaky Blinders",
            Coordinator = user,
            ImageUrl = "https://en.wikipedia.org/wiki/File:Peaky_Blinders_titlecard.jpg",
            Genre = "Drama",
            ReleaseDate = new DateTime(2013, 9, 12)
         });

         ds.Shows.Add(new Show
         {
            Actors = new Actor[] { alexanderDreymon },
            Name = "The Last Kingdom",
            Coordinator = user,
            ImageUrl = "https://en.wikipedia.org/wiki/File:The_Last_Kingdom_TV_series_titlecard.jpg",
            Genre = "Action",
            ReleaseDate = new DateTime(2015, 10, 10)
         });

         // Save changes
         ds.SaveChanges();

            return true;
        }

        // Loading Episodes
        public bool LoadEpisodes()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Return if there's existing data
            if (ds.Episodes.Count() > 0) { return false; }

            // Fetch the shows object because we need it.
            // The Last Kingdom
            var TheLastKingdom = ds.Shows.SingleOrDefault(a => a.Name == "The Last Kingdom");
            if (TheLastKingdom == null) { return false; }
            ds.Episodes.Add(new Episode
            {
                Name = "S1E1",
                EpisodeNumber = 1,
                SeasonNumber = 1,
                Clerk = user, 
                AirDate = new DateTime(2015, 10, 10), 
                Genre = "Action", 
                ImageUrl = "https://i.guim.co.uk/img/static/sys-images/Guardian/Pix/pictures/2015/10/20/1445335790019/57752c99-1634-4d75-83ee-fd645c069664-2060x1236.jpeg?width=620&dpr=2&s=none", 
                Show = TheLastKingdom
            });
            ds.Episodes.Add(new Episode
            {
                Name = "S1E2",
                EpisodeNumber = 2,
                SeasonNumber = 1,
                Clerk = user,
                AirDate = new DateTime(2015, 10, 22),
                Genre = "Action",
                ImageUrl = "https://s.yimg.com/fz/api/res/1.2/ugtzSrBSGJ05SW4D6K8oOw--~C/YXBwaWQ9c3JjaGRkO2g9MTMxO3E9ODA7dz0yMDA-/https://www.bing.com/th?id=OIP.Gg-seEO17haO90D-knu_QQHaE4&w=200&h=131&c=8&rs=1&qlt=80&pid=3.1",
                Show = TheLastKingdom
            });
            ds.Episodes.Add(new Episode
            {
                Name = "S1E3",
                EpisodeNumber = 3,
                SeasonNumber = 1,
                Clerk = user,
                AirDate = new DateTime(2023, 10, 29),
                Genre = "Action",
                ImageUrl = "https://s.yimg.com/fz/api/res/1.2/ugtzSrBSGJ05SW4D6K8oOw--~C/YXBwaWQ9c3JjaGRkO2g9MTMxO3E9ODA7dz0yMDA-/https://www.bing.com/th?id=OIP.Gg-seEO17haO90D-knu_QQHaE4&w=200&h=131&c=8&rs=1&qlt=80&pid=3.1",
                Show = TheLastKingdom
            });

            // PeakyBlinders
            var peakyblinders = ds.Shows.SingleOrDefault(a => a.Name == "peakyblinders");
            if (peakyblinders == null) { return false; }
            ds.Episodes.Add(new Episode
            {
                Name = "S1E1",
                EpisodeNumber = 1,
                SeasonNumber = 1,
                Clerk = user,
                AirDate = new DateTime(2014, 9, 30),
                Genre = "Drama",
                ImageUrl = "https://www.imdb.com/title/tt2471500/mediaviewer/rm3650120192/?ref_=tt_ov_i",
                Show = peakyblinders
            });
            ds.Episodes.Add(new Episode
            {
                Name = "S1E2",
                EpisodeNumber = 2,
                SeasonNumber = 1,
                Clerk = user,
                AirDate = new DateTime(2014, 10, 1),
                Genre = "Drama",
                ImageUrl = "https://pbs.twimg.com/media/Dhq3jEJX0AYG16B.jpg",
                Show = peakyblinders
            });
            ds.Episodes.Add(new Episode
            {
                Name = "S1E3",
                EpisodeNumber = 3,
                SeasonNumber = 1,
                Clerk = user,
                AirDate = new DateTime(2014, 10, 5),
                Genre = "Drama",
                ImageUrl = "https://www.imdb.com/title/tt2471506/mediaviewer/rm509729792/?ref_=tt_ov_i",
                Show = peakyblinders
            });

            // Save changes
            ds.SaveChanges();

            return true;
        }

        // Removing Roles
        public bool RemoveRoles()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Removing Genres
        public bool RemoveGenres()
        {
            try
            {
                foreach (var e in ds.Genres)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Removing Actors
        public bool RemoveActors()
        {
            try
            {
                foreach (var e in ds.Actors)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Removing Shows
        public bool RemoveShows()
        {
            try
            {
                foreach (var e in ds.Shows)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Removing Episodes
        public bool RemoveEpisodes()
        {
            try
            {
                foreach (var e in ds.Episodes)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }
                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    #endregion

    #region RequestUser Class

    // This "RequestUser" class includes many convenient members that make it
    // easier work with the authenticated user and render user account info.
    // Study the properties and methods, and think about how you could use this class.

    // How to use...
    // In the Manager class, declare a new property named User:
    //    public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value:
    //    User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }

        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }

        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }

        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

    #endregion

}