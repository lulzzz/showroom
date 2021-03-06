﻿using System;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Showroom.Server.Controllers;
using Showroom.Server.Data;
using Showroom.Server.Dtos;
using Showroom.Server.Models;

using Microsoft.EntityFrameworkCore;

namespace Showroom.Server.Services
{
    public class ManagerManager
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IIdentityService identityService;

        public ManagerManager(
            ApplicationDbContext context,
            IMapper mapper,
            IEmailSender emailSender,
            IIdentityService identityService)
        {
            _context = context;
            this.mapper = mapper;
            this.identityService = identityService;
        }

        public async Task<IQueryable<ManagerProfileDto>> GetManagerProfilesAsync(string organizationId = null)
        {
            var user = await identityService.GetUserAsync();

            await _context.Entry(user).Reference(e => e.Profile).LoadAsync();

            var result = _context
                .ManagersProfiles
                .Include(x => x.Organization)
                .Include(x => x.ManagerCompetenceAreas)
                .AsQueryable();

            if (user.Profile.OrganizationId != null)
            {
                result = result.Where(e => e.OrganizationId == user.Profile.OrganizationId || e.OrganizationId == null);
            }

            if (organizationId != null)
            {
                var organization = await _context.Organizations.FindAsync(organizationId);
                if (organization == null)
                {
                    throw new NotFoundException(nameof(Organization), organizationId);
                }

                result = result.Where(e => e.OrganizationId == organizationId);
            }

            return mapper.ProjectTo<ManagerProfileDto>(result);
        }

        public async Task<ManagerProfileDto> GetManagerProfileAsync(Guid id)
        {
            var managerProfile = await _context
                .ManagersProfiles
                .Include(x => x.Organization)
                .Include(x => x.ManagerCompetenceAreas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (managerProfile == null)
            {
                throw new NotFoundException(nameof(ManagerProfile), id);
            }

            return mapper.Map<ManagerProfileDto>(managerProfile);
        }

        public async Task<ManagerProfileDto> CreateManagerProfileAsync(AddManagerProfileDto dto)
        {
            var managerProfile = mapper.Map<ManagerProfile>(dto);

            var user = await identityService.GetUserAsync();

            await _context.Entry(user).Reference(e => e.Profile).LoadAsync();

            if (dto.OrganizationId == null)
            {
                managerProfile.OrganizationId = user.Profile.OrganizationId;
            }

            _context.ManagersProfiles.Add(managerProfile);
            await _context.SaveChangesAsync();

            return mapper.Map<ManagerProfileDto>(managerProfile);
        }

        public async Task<ManagerProfileDto> UpdateManagerProfileAsync(UpdateManagerProfileDto dto)
        {
            var managerProfile = await _context.ManagersProfiles.FindAsync(dto.Id);

            if (managerProfile == null)
            {
                throw new NotFoundException(nameof(ManagerProfile), dto.Id);
            }

            managerProfile = mapper.Map(dto, managerProfile);

            _context.Update(managerProfile);

            await _context.SaveChangesAsync();

            return mapper.Map<ManagerProfileDto>(managerProfile);
        }

        public async Task DeleteManagerProfileAsync(Guid id)
        {
            var managerProfile = await _context.ConsultantProfiles.FindAsync(id);
            if (managerProfile == null)
            {
                throw new NotFoundException(nameof(ManagerProfile), id);
            }

            _context.ConsultantProfiles.Remove(managerProfile);
            await _context.SaveChangesAsync();
        }
    }
}
