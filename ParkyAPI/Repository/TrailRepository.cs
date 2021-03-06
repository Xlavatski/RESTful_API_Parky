using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;
        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool CreateTrail(Trail trail)
        {
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            return _db.Trails.Include(c => c.NationalPark).FirstOrDefault(a => a.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(c => c.NationalPark).OrderBy(a => a.Name).ToList();
        }

        public ICollection<Trail> GetTrailsInNationalParks(int npId)
        {
            return _db.Trails.Include(c => c.NationalPark).Where(c=>c.NationalParkId == npId).ToList();
        }

        public bool Save()
        {
            var values = _db.SaveChanges() > 0 ? true : false;
            return values;
        }

        public bool TrailExist(string name)
        {
            bool value = _db.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExist(int id)
        {
            return _db.Trails.Any(a => a.Id == id);
        }

        public bool UpdataTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }
    }
}
