using Application.DTOs;
using Application.DTOs.User;
using Application.Mappers;
using Application.Services.Interfaces;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly UserMapper _mapper;

        public UserService(IUserRepository repository, UserMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            // Mapper le DTO vers l'entité
            var user = _mapper.ToEntity(dto);

            // Créer l'entité (Id et DateCreation générés par le repository)
            var created = await _repository.AddAsync(user);

            // Retourner le DTO de l'entité créée
            return _mapper.ToDto(created);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            // Vérifier que l'utilisateur existe
            var exists = await _repository.ExistsAsync(id);

            if (!exists)
            {
                return false;
            }

            // Supprimer l'utilisateur
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var result = await _repository.GetAllAsync()
                .ContinueWith(users => _mapper.ToDtoList(users.Result));
            return result;
        }

        public async Task<IEnumerable<UserDto>> SearchQuery(string query)
        {
            /* var result = await _repository.GetAllAsync().
                 .ContinueWith(users => _mapper.ToDtoList(users.Result));*/

            var result = await _repository.SearchQuery(query)
                .ContinueWith(users => _mapper.ToDtoList(users.Result));

            return result;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return null;

            return _mapper.ToDto(user);
        }

        public async Task<UserDto?> UpdateAsync(UpdateUserDto dto)
        {
            // Load existing user
            var user = await _repository.GetByIdAsync(dto.Id);
            if (user == null)
                return null;

            // UPDATE entity (use your mapper correctly)
            _mapper.UpdateEntity(dto, user);

            // Save
            await _repository.UpdateAsync(user);

            // Return updated value
            return _mapper.ToDto(user);
        }

    }
}
