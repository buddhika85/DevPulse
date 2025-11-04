using TaskService.Application.Common.Mappers;
using UserService.Application.Commands;
using UserService.Application.Common.Models;
using UserService.Application.Dtos;
using UserService.Application.Queries;
using UserService.Domain.Entities;
using UserService.Domain.ValueObjects;
using UserService.Infrastructure.Identity;
using UserService.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UserService.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IExternalIdentityProvider _identityProvider;               // communicates with Micro Az Entra External ID

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IExternalIdentityProvider identityProvider)
        {
            _userRepository = userRepository;
            _logger = logger;
            _identityProvider = identityProvider;
        }

        
           
        public async Task<IReadOnlyList<UserAccountDto>> GetAllUsersAsync(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all users with includeDeleted: {IncludeDeleted} at {Time}", query.IncludeDeleted, DateTime.UtcNow);

                var entities = await _userRepository.GetAllAsync(query.IncludeDeleted, cancellationToken);
                _logger.LogInformation("Retrieved {UserCount} users with includeDeleted: {IncludeDeleted} at {Time}", entities.Count, query.IncludeDeleted, DateTime.UtcNow);

                var dtos = UserAccountMapper.ToDtosList(entities);
                _logger.LogInformation("Mapped {UserCount} users with includeDeleted: {IncludeDeleted} at {Time}", dtos.Count(), query.IncludeDeleted, DateTime.UtcNow);

                return dtos.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving all users with includeDeleted: {IncludeDeleted} at {Time}", query.IncludeDeleted, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<IReadOnlyList<UserAccountDto>> GetAllUsersByRoleAsync(GetAllUsersByRoleQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve users in role {Role} at {Time}", query.Role.Value, DateTime.UtcNow);
                var entitites = await _userRepository.GetAllAsync(query.Role, cancellationToken);
                _logger.LogInformation("Retrieved {UserCount} {Role} users at {Time}", entitites.Count, query.Role.Value, DateTime.UtcNow);

                var dtos = UserAccountMapper.ToDtosList(entitites);
                _logger.LogInformation("Retrieved {UserCount} {Role} users at {Time}", dtos.Count(), query.Role.Value, DateTime.UtcNow);

                return dtos.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving {Role} users at {Time}", query.Role.Value, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<PaginatedResult<UserAccountDto>> GetUserAccountsPaginatedAsync(GetUsersPaginatedQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Attempting to retrieve paginated Users with filters: Email={Email}, DisplayName={DisplayName}, Role={Role}, Page={PageNumber}, Size={PageSize}, SortBy={SortBy}, Desc={SortDescending} at {Time}",
                    query.Email, query.DisplayName, query.Role, query.PageNumber, query.PageSize, query.SortBy, query.SortDescending, DateTime.UtcNow);

                var pagedResult = await _userRepository.GetUserAccountsPaginatedAsync(query, cancellationToken);
                _logger.LogInformation("Successfully retrieved {Count} Users for page {PageNumber} at {Time}", pagedResult.PageItems.Count, pagedResult.PageNumber, DateTime.UtcNow);

                if (!pagedResult.PageItems.Any())
                {
                    _logger.LogInformation("No Users found for page {PageNumber} at {Time}", pagedResult.PageNumber, DateTime.UtcNow);
                    return new PaginatedResult<UserAccountDto>
                    {
                        PageItems = [],
                        PageNumber = pagedResult.PageNumber,
                        PageSize = pagedResult.PageSize,
                        TotalCount = pagedResult.TotalCount
                    };
                }

                var dtos = UserAccountMapper.ToDtosList(pagedResult.PageItems);
                _logger.LogInformation("Successfully mapped {Count} Users for page {PageNumber} at {Time}", dtos.Count(), pagedResult.PageNumber, DateTime.UtcNow);

                return new PaginatedResult<UserAccountDto>
                {
                    PageItems = dtos.ToList(),
                    PageNumber = pagedResult.PageNumber,
                    PageSize = pagedResult.PageSize,
                    TotalCount = pagedResult.TotalCount                    
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving paginated Users at {Time}", DateTime.UtcNow);
                throw;
            }
        }

        public async Task<UserAccountDto?> GetUserByIdAsync(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve a user with Id:{Id} at {Time}", query.Id, DateTime.UtcNow);
            try
            {
                var entity = await _userRepository.GetByIdAsync(query.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("No user with Id: {Id} at {Time}", query.Id, DateTime.UtcNow);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved a user with email '{Email}' with Id: {Id} at {Time}", entity.Email, entity.Id, DateTime.UtcNow);

                var dto = UserAccountMapper.ToDto(entity);
                _logger.LogInformation("Successfully retrieved a user with email '{Email}' with Id: {Id} at {Time}", dto.Email, dto.Id, DateTime.UtcNow);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving a user with Id:{Id} at {Time}", query.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> ExistsByEmailAsync(ExistsByEmailQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Checking if a user exists with email:{Email} at:{Time}", query.Email, DateTime.UtcNow);
            try
            {
                var exists = await _userRepository.ExistsByEmailAsync(query.Email, cancellationToken);
                if (exists)
                    _logger.LogDebug("User with email: {Email} not found", query.Email);
                else
                    _logger.LogDebug("User with email: {Email} found", query.Email);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while checking if a user with email:{Email} exists at {Time}", query.Email, DateTime.UtcNow);
                throw;
            }
        }



        public async Task<Guid?> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Registering a new user with email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);

                // create domain object and raise event
                var userAccount = UserAccount.Create(command.Email, command.DisplayName, command.Role);

                var result = await _userRepository.AddAsync(userAccount, cancellationToken);

                if (result is not null)
                {
                    _logger.LogInformation("User account created successfully email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                    return result.Id;
                }

                _logger.LogWarning("user account creation failed for email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a user account with email: {Email}, display name: {DisplayName} at {Time}", command.Email, command.DisplayName, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Update user with Id: {Id} at {Time}", command.Id, DateTime.UtcNow);
            try
            {
                var entity = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
                if (entity is null)
                {
                    _logger.LogWarning("User not found for update: {Id}", command.Id);
                    return false;
                }

                ApplyChanges(entity, command);  // store domain events
                var result = await _userRepository.UpdateAsync(command.Id, entity, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Successfully updated user with Id: {Id}, Email: {Email} at {Time}", entity.Id, entity.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Update operation for User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", entity.Id, entity.Email, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating User with Id: {Id}, Email: {Email} at {Time}", command.Id, command.Email, DateTime.UtcNow);
                throw;
            }
        }

        // update and store domain events
        private void ApplyChanges(UserAccount existing, UpdateUserCommand updateCommand)
        {
            var timestamp = DateTime.UtcNow;

            if (updateCommand.Email is not null && !existing.Email.Equals(updateCommand.Email, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Updating Email for user Id: {Id} from '{OldEmail}' to '{NewEmail}' at {Time}",
                    existing.Id, existing.Email, updateCommand.Email, timestamp);

                existing.UpdateEmail(updateCommand.Email);
            }

            if (updateCommand.Role is not null && existing.Role != updateCommand.Role)
            {
                _logger.LogInformation("Updating Role for user Id: {Id} from '{OldRole}' to '{NewRole}' at {Time}",
                    existing.Id, existing.Role, updateCommand.Role, timestamp);

                existing.UpdateRole(updateCommand.Role);
            }

            if (updateCommand.DisplayName is not null && existing.DisplayName != updateCommand.DisplayName)
            {
                _logger.LogInformation("Updating DisplayName for user Id: {Id} from '{OldDisplayName}' to '{NewDisplayName}' at {Time}",
                    existing.Id, existing.DisplayName, updateCommand.DisplayName, timestamp);

                existing.UpdateDisplayName(updateCommand.DisplayName);
            }
        }

        public async Task<bool> DeleteUserAsync(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Attempting to soft delete a user with Id:{Id} at {Time}", command.Id, DateTime.UtcNow);

                var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
                if (user is null)
                {
                    _logger.LogError("No user with Id: {Id} at {Time}. Deletion aborted.", command.Id, DateTime.UtcNow);
                    return false;
                }

                if (user.IsDeleted)
                {
                    _logger.LogWarning("User with Id: {Id} is already soft-deleted. No action taken at {Time}.", command.Id, DateTime.UtcNow);
                    return false;
                }

                user.SoftDelete();          // store domain events
                var result = await _userRepository.SoftDeleteUserAsync(user, cancellationToken);
                if (result)
                {
                    _logger.LogInformation("Successfully soft deleted user with Id: {Id}, Email: {Email} at {Time}", user.Id, user.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Soft deleting User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", user.Id, user.Email, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while soft deleting User with Id: {Id} at {Time}", command.Id, DateTime.UtcNow);
                throw;
            }
        }

        public async Task<bool> RestoreUserAsync(RestoreUserCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to restore a user with Id:{Id} at {Time}", command.Id, DateTime.UtcNow);
            try
            {  
                var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
                if (user is null)
                {
                    _logger.LogError("No user with Id: {Id} at {Time}. Restore user aborted.", command.Id, DateTime.UtcNow);
                    return false;
                }

                if (!user.IsDeleted)
                {
                    _logger.LogWarning("User with Id: {Id} is not soft deleted. No action taken at {Time}.", command.Id, DateTime.UtcNow);
                    return false;
                }

                user.RestoreUser();         // restore and store domain events on the entity

                var result = await _userRepository.RestoreUserAsync(user, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Successfully restored user with Id: {Id}, Email: {Email} at {Time}", user.Id, user.Email, DateTime.UtcNow);
                    return true;
                }

                _logger.LogWarning("Restoring User with Id: {Id}, Email: {Email} at {Time} did not affect any records.", user.Id, user.Email, DateTime.UtcNow);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while restoring User with Id: {Id} at {Time}", command.Id, DateTime.UtcNow);
                throw;
            }
        }

        /// <summary>
        /// From a given object id (of entra maintaining guid),
        /// Trying to resolve - find user in DB and return
        /// OR 
        /// Trying to create - Get more user info from Entra and create a new user and return
        /// </summary>
        /// <param name="objectId">entra maintaining guid</param>
        /// <param name="cancellationToken">for termination</param>
        /// <returns></returns>
        public async Task<UserAccountDto?> ResolveOrCreateAsync(string objectId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to Resolve Or CreateAsync a user with object id: {ObjectId} at {Time}", objectId, DateTime.UtcNow);
            if (!Guid.TryParse(objectId, out _))
            {
                _logger.LogWarning("Invalid objectId format: {ObjectId}", objectId);
                return null;
            }


            try
            {
                // Trying to resolve - find user in DB and return
                var entity = await _userRepository.GetByIdAsync(new Guid(objectId), cancellationToken);
                if (entity is not null)
                {
                    _logger.LogInformation("For object id: {ObjectId} - successfully retrieved an existing user with email '{Email}' with Id: {Id} at {Time} from UserAccounts table", objectId, entity.Email, entity.Id, DateTime.UtcNow);
                    var dto = UserAccountMapper.ToDto(entity);
                    return dto;
                }

                _logger.LogInformation("No existing user with object id: {Id} at {Time} in UserAccounts table, next checking entra to make sure", objectId, DateTime.UtcNow);
                var entraUser = await _identityProvider.GetUserByObjectIdAsync(objectId, cancellationToken);
                if (entraUser is null)
                {
                    _logger.LogError("No existing user with object id: {Id} at {Time} in Entra also, possibly a malicious request. retuning null.", objectId, DateTime.UtcNow);
                    return null;
                }

                // Trying to create - Get more user info from Entra and create a new user and return
                _logger.LogInformation("Fetched user from Entra for object id: {ObjectId}. Creating new user with email '{Email}' and display name '{DisplayName}' at {Time} in SQL UserAccounts table.",
                                objectId, entraUser.Email, entraUser.DisplayName, DateTime.UtcNow);

                var userAccount = UserAccount.Create(entraUser.Email, entraUser.DisplayName, entraUser.UserRole, objectId);

                var result = await _userRepository.AddAsync(userAccount, cancellationToken);

                if (result is not null)
                {
                    _logger.LogInformation("User account created successfully with object id: {ObjectID} email: {Email}, display name: {DisplayName} at {Time}", userAccount.Id, userAccount.Email, userAccount.DisplayName, DateTime.UtcNow);
                    return UserAccountMapper.ToDto(userAccount);
                }

                _logger.LogWarning("User account creation failed for object id: {ObjectID} email: {Email}, display name: {DisplayName} at {Time}", userAccount.Id, userAccount.Email, userAccount.DisplayName, DateTime.UtcNow);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resolving or creating a user account with object id: {ObjectID} at {Time}", objectId, DateTime.UtcNow);
                throw;
            }
        }
    }
}
