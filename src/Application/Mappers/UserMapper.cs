using Application.DTOs.User;
using Domain.Entities;
using Riok.Mapperly.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

[Mapper]
    public partial class UserMapper
    {
        public partial UserDto ToDto(User user);

        /// <summary>
        /// Mappe une collection d'entités vers une collection de DTOs.
        /// </summary>
        public partial IEnumerable<UserDto> ToDtoList(IEnumerable<User> users);

        /// <summary>
        /// Mappe un CreateUserDto vers une entité User.
        /// Id et DateCreation ne sont pas mappés (générés par le repository).
        /// </summary>
        public partial User ToEntity(CreateUserDto dto);

        /// <summary>
        /// Mappe un UpdateUserDto vers une entité User.
        /// DateCreation ne sera pas mappé car absent du DTO (propriété immutable).
        /// </summary>
        public partial User ToEntity(UpdateUserDto dto);

        /// <summary>
        /// Met à jour une entité existante avec les données d'un UpdateUserDto.
        /// Préserve Id et DateCreation de l'entité existante.
        /// </summary>
        [MapperIgnoreTarget(nameof(User.Id))]
        public partial void UpdateEntity(UpdateUserDto dto, User existingEntity);
    }

