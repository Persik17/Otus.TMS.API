﻿using Microsoft.Extensions.Logging;
using TMS.Abstractions.Exceptions;
using TMS.Abstractions.Interfaces.Repositories.BaseRepositories;
using TMS.Abstractions.Interfaces.Services;
using TMS.Application.DTOs.Column;
using TMS.Application.Extensions;
using TMS.Infrastructure.DataAccess.DataModels;

namespace TMS.Application.Services
{
    /// <summary>
    /// Provides a service for managing columns.
    /// Implements operations for creating, retrieving, and updating columns.
    /// </summary>
    public class ColumnService : IColumnService<ColumnDto, ColumnCreateDto>
    {
        private readonly IAuditableCommandRepository<Column> _commandRepository;
        private readonly IAuditableQueryRepository<Column> _queryRepository;
        private readonly ILogger<ColumnService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnService"/> class.
        /// </summary>
        /// <param name="commandRepository">The repository for performing auditable column commands (e.g., insert, update).</param>
        /// <param name="queryRepository">The repository for performing auditable column queries (e.g., get by id).</param>
        /// <param name="logger">The logger for logging column service events.</param>
        public ColumnService(
            IAuditableCommandRepository<Column> commandRepository,
            IAuditableQueryRepository<Column> queryRepository,
            ILogger<ColumnService> logger)
        {
            _commandRepository = commandRepository;
            _queryRepository = queryRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ColumnDto> CreateAsync(ColumnCreateDto createDto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(createDto);

            _logger.LogInformation("Creating new column: {Name}", createDto.Name);

            var newColumn = createDto.ToColumn();
            await _commandRepository.InsertAsync(newColumn, cancellationToken);

            var createdColumn = await _queryRepository.GetByIdAsync(newColumn.Id, cancellationToken)
                ?? throw new NotFoundException(typeof(Column));

            _logger.LogInformation("Column created successfully with id: {Id}", newColumn.Id);

            return createdColumn.ToColumnDto();
        }

        /// <inheritdoc/>
        public async Task<ColumnDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to get column with empty id");
                throw new WrongIdException(typeof(Column));
            }

            var column = await _queryRepository.GetByIdAsync(id, cancellationToken);
            if (column == null)
            {
                _logger.LogWarning("Column with id {Id} not found", id);
                return null; // Consider returning null instead of throwing an exception if the column is not found.
            }

            return column.ToColumnDto();
        }

        /// <inheritdoc/>
        public async Task<ColumnDto> UpdateAsync(ColumnDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            if (dto.Id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to update column with empty id");
                throw new WrongIdException(typeof(Column));
            }

            var existingColumn = await _queryRepository.GetByIdAsync(dto.Id, cancellationToken);
            if (existingColumn == null)
            {
                _logger.LogWarning("Column with id {Id} not found for update", dto.Id);
                throw new NotFoundException(typeof(Column));
            }

            existingColumn.Name = dto.Name;
            existingColumn.Description = dto.Description;
            existingColumn.ColumnType = dto.ColumnType;
            existingColumn.Order = dto.Order;
            existingColumn.Color = dto.Color;
            existingColumn.UpdateDate = DateTime.UtcNow;

            await _commandRepository.UpdateAsync(existingColumn, cancellationToken);

            _logger.LogInformation("Column with id {Id} updated successfully", dto.Id);

            return dto;
        }

        /// <inheritdoc/>
        public async System.Threading.Tasks.Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete column with empty id");
                throw new WrongIdException(typeof(Column));
            }

            await _commandRepository.DeleteAsync(id, cancellationToken);

            _logger.LogInformation("Column with id {Id} deleted", id);
        }
    }
}