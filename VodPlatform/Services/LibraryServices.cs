using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VodPlatform.Database;
using VodPlatform.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VodPlatform.Services
{
    public class LibraryServices : ILibraryActions, ILibraryData
    {
        private readonly ILogger<LibraryServices> _logger;
        private readonly DatabaseContext _context;

        public LibraryServices(DatabaseContext context, ILogger<LibraryServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddMovie(int Id_Series, string Title, string Type, string Category)
        {
            try
            {
                _logger.LogInformation("Adding movie: {Title}", Title);
                Movie movie = new Movie()
                {
                    Id_Series = Id_Series,
                    Title = Title,
                    Type = Type,
                    Category = Category,
                };

                await _context.Movie.AddAsync(movie);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Movie {Title} added successfully", Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding movie: {Title}", Title);
                throw;
            }
        }

        public async Task AddSerial(int Seasons, string Title, string Category, List<int> Episodes, int Id_Series)
        {
            try
            {
                _logger.LogInformation("Adding serial: {Title}", Title);
                Title = Title.TrimEnd();
                Series series = await _context.Series.FindAsync(Id_Series);
                List<Movie> movies = new List<Movie>();

                for (int i = 0; i < Seasons; i++)
                {
                    if (i < Episodes.Count)
                    {
                        int numberOfEpisodes = Episodes[i];

                        for (int j = 1; j <= numberOfEpisodes; j++)
                        {
                            Movie movie = new Movie()
                            {
                                Id_Series = Id_Series,
                                Title = $"{Title} {i + 1}-{j}",
                                Type = "Series",
                                Category = Category,
                                Series = series
                            };

                            movies.Add(movie);
                        }
                    }
                }

                await _context.Movie.AddRangeAsync(movies);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Serial {Title} added successfully", Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding serial: {Title}", Title);
                throw;
            }
        }

        public async Task AddSeries(string Title)
        {
            try
            {
                _logger.LogInformation("Adding series: {Title}", Title);
                Series series = new Series() { Title = Title };
                await _context.Series.AddAsync(series);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Series {Title} added successfully", Title);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError(ex, "Database context was disposed prematurely when adding series: {Title}", Title);
                throw new InvalidOperationException("The database context was disposed prematurely. Please ensure proper scope management.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding series: {Title}", Title);
                throw;
            }
        }


        public async Task DataAssociation(int Id_Series, List<int> Id_Movie)
        {
            try
            {
                _logger.LogInformation("Associating movies with series ID {Id_Series}", Id_Series);
                Series series = await _context.Series.Include(s => s.Movies).FirstOrDefaultAsync(s => s.Id == Id_Series);
                if (series == null)
                {
                    _logger.LogWarning("Series not found: {Id_Series}", Id_Series);
                    throw new ArgumentException("Series not found");
                }

                List<Movie> movies = await _context.Movie.Where(movie => Id_Movie.Contains(movie.Id)).ToListAsync();
                if (movies.Count != Id_Movie.Count)
                {
                    _logger.LogWarning("Some movies were not found for association");
                    throw new ArgumentException("Some movies were not found");
                }

                foreach (var movie in movies)
                {
                    if (movie.Id_Series != null && movie.Id_Series != Id_Series)
                    {
                        var oldSeries = await _context.Series.Include(s => s.Movies).FirstOrDefaultAsync(s => s.Id == movie.Id_Series);
                        if (oldSeries != null)
                        {
                            oldSeries.Movies.Remove(movie);
                        }
                    }
                    movie.Id_Series = Id_Series;
                }
                series.Movies = movies;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Movies successfully associated with series ID {Id_Series}", Id_Series);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error associating movies with series ID {Id_Series}", Id_Series);
                throw;
            }
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            try
            {
                _logger.LogInformation("Retrieving all movies");
                return await _context.Movie.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving movies");
                throw;
            }
        }

        public async Task<List<Series>> GetAllSeries()
        {
            try
            {
                _logger.LogInformation("Retrieving all series");
                return await _context.Series.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving series");
                throw;
            }
        }
    }
}
