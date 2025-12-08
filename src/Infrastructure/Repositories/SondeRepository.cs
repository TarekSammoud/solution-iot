using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class SondeRepository : ISondeRepository
    {
        private readonly AppDbContext _context;

        public SondeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sonde?> GetByIdAsync(Guid id)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sonde?> GetByIdWithRelationsAsync(Guid id)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Sonde>> GetAllAsync()
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetAllWithRelationsAsync()
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetByLocalisationAsync(Guid localisationId)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .Where(s => s.LocalisationId == localisationId)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetByLocalisationWithRelationsAsync(Guid localisationId)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .Where(s => s.LocalisationId == localisationId)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetByTypeAsync(TypeSonde type)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .Where(s => s.TypeSonde == type)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetByTypeWithRelationsAsync(TypeSonde type)
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .Where(s => s.TypeSonde == type)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sonde>> GetActivesAsync()
        {
            return await _context.Sondes
                .Include(s => s.Localisation)
                .Include(s => s.UniteMesure)
                .Where(s => s.EstActif)
                .OrderBy(s => s.Nom)
                .ToListAsync();
        }

        public async Task<Sonde> AddAsync(Sonde sonde)
        {
            if (sonde.Id == Guid.Empty) sonde.Id = Guid.NewGuid();
            if (sonde.DateCreation == default) sonde.DateCreation = DateTime.UtcNow;
            if (sonde.DateInstallation == default) sonde.DateInstallation = DateTime.UtcNow;

            _context.Sondes.Add(sonde);
            await _context.SaveChangesAsync();
            return sonde;
        }

        public async Task UpdateAsync(Sonde sonde)
        {
            _context.Sondes.Update(sonde);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Sonde sonde)
        {
            _context.Sondes.Remove(sonde);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var sonde = await _context.Sondes.FindAsync(id);
            if (sonde != null)
            {
                _context.Sondes.Remove(sonde);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Sondes.AnyAsync(s => s.Id == id);
        }
    }
}
