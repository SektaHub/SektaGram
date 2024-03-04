﻿using AutoMapper;
using backend.Models;
using backend.Models.Dto;
using backend.Models.Entity;
using backend.Repo;
using backend.Services.Common;
using Microsoft.AspNetCore.Identity;

namespace backend.Services
{
    public class AudioService : BaseFileContentService<Audio, AudioDto>
    {
        public AudioService(IWebHostEnvironment env, IMapper mapper, ApplicationDbContext dbContext, MongoDBRepository mongoRepo, AnyFileRepository anyFileRepository, UserManager<ApplicationUser> userManager) : base(env, mapper, dbContext, mongoRepo, anyFileRepository, userManager)
        {
        }
    }
}
