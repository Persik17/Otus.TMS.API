﻿using Microsoft.Extensions.Logging;
using TMS.Abstractions.Interfaces.Repositories.BaseRepositories;
using TMS.Abstractions.Interfaces.Services;
using TMS.Abstractions.Models.DTOs.Registration;
using TMS.Contracts.Events;
using TMS.Infrastructure.DataAccess.DataModels;

namespace TMS.Application.Services
{
    /// <summary>
    /// Service for registering new users and initiating verification.
    /// </summary>
    public class RegistrationService : IRegistrationService
    {
        private readonly ICommandRepository<RegistrationVerification> _commandRepository;
        private readonly INotifyService _notifyService;
        private readonly ILogger<RegistrationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationService"/> class.
        /// </summary>
        /// <param name="commandRepository">The repository for performing registration verification commands (e.g., insert).</param>
        /// <param name="notifyService">The service for sending notifications (e.g., email, SMS, Telegram).</param>
        /// <param name="logger">The logger for logging registration service events.</param>
        public RegistrationService(
            ICommandRepository<RegistrationVerification> commandRepository,
            INotifyService notifyService,
            ILogger<RegistrationService> logger)
        {
            _commandRepository = commandRepository;
            _notifyService = notifyService;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<RegistrationResultDto> RegisterAsync(RegistrationDto dto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dto);

            // Генерируем код подтверждения
            var code = new Random().Next(100000, 999999).ToString();
            var verification = new RegistrationVerification
            {
                Id = Guid.NewGuid(),
                Target = dto.Target,
                Type = (int)dto.Type,
                Code = code,
                Expiration = DateTime.UtcNow.AddMinutes(10),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _commandRepository.InsertAsync(verification, cancellationToken);

            await _notifyService.PublishAsync(new RegistrationVerificationCreatedEvent
            {
                VerificationId = verification.Id,
                Target = verification.Target,
                Type = verification.Type,
                Code = verification.Code,
                Expiration = verification.Expiration
            }, cancellationToken);

            _logger.LogInformation("Registration verification created for {Target} ({Type})", dto.Target, dto.Type);

            return new RegistrationResultDto
            {
                VerificationId = verification.Id,
                Expiration = verification.Expiration,
                Success = true
            };
        }
    }
}