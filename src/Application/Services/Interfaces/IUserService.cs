using Application.DTOs;
using Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Récupère une utilisateur par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant unique de la utilisateur.</param>
        /// <returns>Le DTO de la utilisateur si trouvée, sinon null.</returns>
        Task<UserDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// Récupère toutes les utilisateurs.
        /// </summary>
        /// <returns>Une collection de DTOs de toutes les utilisateurs.</returns>
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<IEnumerable<UserDto>> SearchQuery(string query);
        Task<UserStatsDto> GetStatsAsync();



        /// <summary>
        /// Crée une nouvelle utilisateur.
        /// </summary>
        /// <param name="dto">Le DTO contenant les données de création.</param>
        /// <returns>Le DTO de la utilisateur créée avec Id et DateCreation générés.</returns>
        Task<UserDto> CreateAsync(CreateUserDto dto);

        /// <summary>
        /// Met à jour une utilisateur existante.
        /// </summary>
        /// <param name="dto">Le DTO contenant les données de mise à jour.</param>
        /// <returns>Le DTO de la utilisateur mise à jour si trouvée, sinon null.</returns>
        Task<UserDto?> UpdateAsync(UpdateUserDto dto);

        /// <summary>
        /// Supprime une utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant de la utilisateur à supprimer.</param>
        /// <returns>True si la suppression a réussi, false si la utilisateur n'existe pas.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Vérifie si une utilisateur existe.
        /// </summary>
        /// <param name="id">L'identifiant de la utilisateur.</param>
        /// <returns>True si la utilisateur existe, sinon false.</returns>
        Task<bool> ExistsAsync(Guid id);
    }
}
